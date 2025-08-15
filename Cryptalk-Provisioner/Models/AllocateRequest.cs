namespace Cryptalk_Provisioner.Models
{
    public record AllocateRequest(string installId, string publicIpV4, string? publicIpV6)
    {
        public string InstallId = installId;
        public string PublicIpV4 = publicIpV4;
        public string? PublicIpV6 = publicIpV6;
    }
}
