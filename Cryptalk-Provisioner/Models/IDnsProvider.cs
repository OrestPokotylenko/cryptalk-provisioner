namespace Cryptalk_Provisioner.Models
{
    public interface IDnsProvider
    {
        Task CreateAAsync(string fqdn, string ipv4, int ttl = 120, CancellationToken ct = default);
        Task CreateAAAAAsync(string fqdn, string ipv6, int ttl = 120, CancellationToken ct = default);
        Task DeleteAllAsync(string fqdn, CancellationToken ct = default);
    }
}