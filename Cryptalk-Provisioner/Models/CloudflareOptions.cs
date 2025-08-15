namespace Cryptalk_Provisioner.Models
{
    public sealed class CloudflareOptions
    {
        public string ZoneId { get; set; } = default!;
        public string ApiToken { get; set; } = default!;
    }
}