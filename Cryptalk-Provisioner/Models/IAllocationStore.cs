namespace Cryptalk_Provisioner.Models
{
    public interface IAllocationStore
    {
        Task SaveAsync(Allocation a, CancellationToken ct = default);
        Task<Allocation?> GetByInstallIdAsync(string installId, CancellationToken ct = default);
    }
}