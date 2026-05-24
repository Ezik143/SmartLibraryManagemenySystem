using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Models.Entities;

namespace SmartLib.Services;

/// <summary>
/// Background service that runs daily to check for overdue books and create fines automatically.
/// </summary>
public class OverdueCheckService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueCheckService> _logger;

    public OverdueCheckService(
        IServiceScopeFactory scopeFactory,
        ILogger<OverdueCheckService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Overdue check service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndProcessOverdueBooksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking overdue books.");
            }

            // Wait for 24 hours before running again
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    /// <summary>
    /// Checks for overdue borrow records and creates fines automatically.
    /// </summary>
    private async Task CheckAndProcessOverdueBooksAsync()
    {
        _logger.LogInformation("Checking for overdue books at {Time}", DateTime.UtcNow);

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Find all BORROWED records where DueDate has passed
        var overdueRecords = await context.BorrowRecords
            .Where(br => br.Status == BorrowRecordStatus.BORROWED
                      && DateTime.UtcNow > br.DueDate)
            .ToListAsync();

        if (overdueRecords.Count == 0)
        {
            _logger.LogInformation("No overdue books found.");
            return;
        }

        _logger.LogInformation("Found {Count} overdue books. Processing fines.", overdueRecords.Count);

        foreach (var record in overdueRecords)
        {
            // Change status to OVERDUE
            record.Status = BorrowRecordStatus.OVERDUE;

            // Calculate overdue days
            var overdueDays = (DateTime.UtcNow - record.DueDate).Days;
            int fineRate = 3;          // Daily fine rate
            int maxDays = 30;         // Maximum days for fine calculation
            int daysToCharge = Math.Min(overdueDays, maxDays);
            decimal fineAmount = fineRate * daysToCharge;

            // Check if a fine already exists for this record
            var existingFine = await context.Fines
                .FirstOrDefaultAsync(f => f.RecordId == record.BorrowRecordId);

            if (existingFine != null)
            {
                // Update existing fine amount if it's still PENDING
                if (existingFine.Status == FineStatus.PENDING)
                {
                    existingFine.Amount = fineAmount;
                    existingFine.CreatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Updated fine for RecordId {RecordId}: {Amount}",
                        record.BorrowRecordId, fineAmount);
                }
            }
            else
            {
                // Create new fine
                var fine = new Fine
                {
                    UserId = record.UserId,
                    RecordId = record.BorrowRecordId,
                    Amount = fineAmount,
                    Status = FineStatus.PENDING,
                    CreatedAt = DateTime.UtcNow
                };

                context.Fines.Add(fine);
                _logger.LogInformation("Created new fine for RecordId {RecordId}: {Amount}",
                    record.BorrowRecordId, fineAmount);
            }
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Successfully processed overdue books.");
    }
}
