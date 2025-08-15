using Cryptalk_Provisioner.Models;

namespace Cryptalk_Provisioner.Services
{
    public sealed class AllocateSubdomainHandler
    {
        private readonly IDnsProvider _dns;
        private readonly IAllocationStore _store;
        private readonly string _baseDomain;
        private readonly string _prefix;

        public AllocateSubdomainHandler(IDnsProvider dns, IAllocationStore store, string baseDomain, string prefix)
        {
            _dns = dns; _store = store; _baseDomain = baseDomain; _prefix = prefix;
        }

        public async Task<AllocateResponse> HandleAsync(AllocateRequest req, CancellationToken ct = default)
        {
            // idempotent by InstallId
            var existing = await _store.GetByInstallIdAsync(req.InstallId, ct);
            if (existing is not null) return new AllocateResponse(existing.Fqdn);

            var id = Guid.NewGuid().ToString("N")[..6];
            var sub = $"{_prefix}-{id}".Trim().ToLowerInvariant();
            var fqdn = $"{sub}.{_baseDomain}";

            if (!string.IsNullOrWhiteSpace(req.PublicIpV4))
                await _dns.CreateAAsync(sub, req.PublicIpV4!, ct: ct);
            if (!string.IsNullOrWhiteSpace(req.PublicIpV6))
                await _dns.CreateAAAAAsync(sub, req.PublicIpV6!, ct: ct);

            await _store.SaveAsync(new Allocation
            {
                InstallId = req.InstallId,
                Fqdn = fqdn,
                IPv4 = req.PublicIpV4,
                IPv6 = req.PublicIpV6
            }, ct);

            return new AllocateResponse(fqdn);
        }
    }
}