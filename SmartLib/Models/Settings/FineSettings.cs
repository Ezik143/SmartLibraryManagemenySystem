using System.ComponentModel.DataAnnotations;

namespace SmartLib.Models.Settings
{
    public sealed class FineSettings
    {
        [Range(typeof(decimal), "0.0", "1000000")]
        public decimal RatePerDay { get; init; } = 3m;

        [Range(0, 3650)]
        public int MaxDays { get; init; } = 30;
    }
}
