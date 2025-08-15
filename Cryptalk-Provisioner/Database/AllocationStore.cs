using Cryptalk_Provisioner.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cryptalk_Provisioner.Database;

public sealed class AllocationStore : IAllocationStore
{
    private readonly ProvisionerDbContext _db;
    public AllocationStore(ProvisionerDbContext db) => _db = db;

    public async Task SaveAsync(Allocation a, CancellationToken ct = default)
    {
        var existing = await _db.Allocations.AsTracking()
            .SingleOrDefaultAsync(x => x.InstallId == a.InstallId, ct);

        if (existing is null)
            await _db.Allocations.AddAsync(new Allocation
            {
                InstallId = a.InstallId,
                Fqdn = a.Fqdn,
                IPv4 = a.IPv4,
                IPv6 = a.IPv6
            }, ct);
        else
        {
            existing.Fqdn = a.Fqdn;
            existing.IPv4 = a.IPv4;
            existing.IPv6 = a.IPv6;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<Allocation?> GetByInstallIdAsync(string installId, CancellationToken ct = default)
    {
        var e = await _db.Allocations.AsNoTracking()
            .SingleOrDefaultAsync(x => x.InstallId == installId, ct);
        if (e is null) return null;

        return new Allocation
        {
            InstallId = e.InstallId,
            Fqdn = e.Fqdn,
            CreatedAt = e.CreatedAt,
            IPv4 = e.IPv4,
            IPv6 = e.IPv6
        };
    }
}