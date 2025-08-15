namespace Cryptalk_Provisioner.Models
{
    public sealed class Allocation
    {
        public string InstallId { get; set; } = default!;
        public string Fqdn { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public string? IPv4 { get; set; }
        public string? IPv6 { get; set; }
    }
}