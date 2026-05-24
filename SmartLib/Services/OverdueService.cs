using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SmartLib.Data;
using SmartLib.Models.Entities;

namespace SmartLib.Services
{
    public class OverdueService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OverdueService> _logger;
        public OverdueService(IServiceScopeFactory serviceScopeFactory, ILogger<OverdueService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // While your service is running, you can perform your background tasks here.
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("Checking for overdue items...");
                try
                {
                    await CheckAndProcessOverdueBooksAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking overdue books.");
                }
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        private async Task CheckAndProcessOverdueBooksAsync()
        {
            _logger.LogInformation("Checking for overdue books at {Time}", DateTime.UtcNow);
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            var Overdue = context.BorrowRecords.Where(br => br.Status == BorrowRecordStatus.BORROWED && DateTime.UtcNow > br.DueDate).ToList();

            if (Overdue.Count == 0)
            {
                _logger.LogInformation("No overdue books found.");
            }

            _logger.LogInformation("Found {Count} overdue books. Processing fines.", Overdue.Count);


            foreach (var record in Overdue)
            {
                record.Status = BorrowRecordStatus.OVERDUE;

                var OverdueDays = (DateTime.UtcNow - record.DueDate).Days;
                int rate = 3;
                int max = 30;
                int days = Math.Min(OverdueDays, max);
                int total = days * rate;

                var fineEntity = await context.Fines.FirstOrDefaultAsync(f => f.RecordId == record.BorrowRecordId);

                if (fineEntity != null)
                {
                    if (fineEntity.Status == FineStatus.PENDING)
                    {
                        fineEntity.Amount = total;
                    }

                }
                else
                {
                    var fine = new Fine
                    {
                        UserId = record.UserId,
                        RecordId = record.BorrowRecordId,
                        Amount = total,
                        Status = FineStatus.PENDING,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Fines.Add(fine);
                }
            }

            await context.SaveChangesAsync();
            _logger.LogInformation("Successfully processed overdue books.");
        }
    }
}
