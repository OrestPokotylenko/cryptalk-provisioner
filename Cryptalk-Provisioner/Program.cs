using Cryptalk_Provisioner.Models;
using Cryptalk_Provisioner.Services;
using Cryptalk_Provisioner.Database;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var cfg = builder.Configuration;

        builder.Services.AddControllers();
        builder.Configuration.AddEnvironmentVariables();

        // Rate limiting (basic)
        builder.Services.AddRateLimiter(o =>
            o.AddFixedWindowLimiter("api", x =>
            {
                x.Window = TimeSpan.FromSeconds(1);
                x.PermitLimit = 5;
                x.QueueLimit = 20;
            }));

        builder.Services.AddSwaggerGen();

        // ---- Required config (sanitize BaseDomain) ----
        var baseDomain = cfg["BaseDomain"].Trim().Trim('.');
        if (string.IsNullOrWhiteSpace(baseDomain))
            throw new Exception("BaseDomain missing or empty");

        var subPrefix = cfg["SubPrefix"].Trim();
        var authKey = builder.Configuration["AuthKey"] ?? throw new Exception("AuthKey missing");

        // ---- EF Core + PostgreSQL ----
        builder.Services.AddDbContext<ProvisionerDbContext>(opt =>
            opt.UseNpgsql(cfg["ConnectionStrings:Default"]));

        // IAllocationStore via EF
        builder.Services.AddScoped<IAllocationStore, AllocationStore>();

        // ---- Cloudflare typed client + options ----
        builder.Services.Configure<CloudflareOptions>(cfg.GetSection("Cloudflare"));

        builder.Services.AddHttpClient<IDnsProvider, CloudflareDnsProvider>(c =>
        {
            c.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = cfg["Cloudflare:ApiToken"];
            if (!string.IsNullOrWhiteSpace(token))
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        });

        // ---- Use case handler (scoped) ----
        builder.Services.AddScoped<AllocateSubdomainHandler>(sp =>
            new AllocateSubdomainHandler(
                sp.GetRequiredService<IDnsProvider>(),
                sp.GetRequiredService<IAllocationStore>(),
                baseDomain,
                subPrefix
            )
        );

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRateLimiter();
        app.MapControllers();
        app.Run();
    }
}