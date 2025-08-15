using Microsoft.EntityFrameworkCore;
using Cryptalk_Provisioner.Models;

public class ProvisionerDbContext : DbContext
{
    public ProvisionerDbContext(DbContextOptions<ProvisionerDbContext> options) : base(options) { }

    public DbSet<Allocation> Allocations => Set<Allocation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Allocation>(e =>
        {
            e.ToTable("allocations");
            e.HasKey(x => x.InstallId);
            e.Property(x => x.InstallId).HasColumnName("install_id").IsRequired();
            e.Property(x => x.Fqdn).HasColumnName("fqdn").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now() at time zone 'utc'");
            e.Property(x => x.IPv4).HasColumnName("ipv4");
            e.Property(x => x.IPv6).HasColumnName("ipv6");
            e.HasIndex(x => x.Fqdn).IsUnique();
        });
    }
}