using Cryptalk_Provisioner.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Cryptalk_Provisioner.Services
{
    public sealed class CloudflareDnsProvider : IDnsProvider
    {
        private readonly HttpClient _http;
        private readonly CloudflareOptions _opt;

        public CloudflareDnsProvider(HttpClient http, IOptions<CloudflareOptions> opt)
        {
            _http = http;
            _opt = opt.Value;

            if (_http.DefaultRequestHeaders.Authorization is null && !string.IsNullOrWhiteSpace(_opt.ApiToken))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _opt.ApiToken);
        }

        public Task CreateAAsync(string nameOrLabel, string ipv4, int ttl = 120, CancellationToken ct = default)
            => CreateRecordAsync("A", nameOrLabel, ipv4, ttl, ct);

        public Task CreateAAAAAsync(string nameOrLabel, string ipv6, int ttl = 120, CancellationToken ct = default)
            => CreateRecordAsync("AAAA", nameOrLabel, ipv6, ttl, ct);

        public async Task DeleteAllAsync(string fqdn, CancellationToken ct = default)
        {
            var list = await _http.GetFromJsonAsync<CfList>($"zones/{_opt.ZoneId}/dns_records?name={fqdn}", ct);
            if (list?.result is null) return;
            foreach (var r in list.result)
                await _http.DeleteAsync($"zones/{_opt.ZoneId}/dns_records/{r.id}", ct);
        }

        private async Task CreateRecordAsync(string type, string name, string content, int ttl, CancellationToken ct)
        {
            // name should be label within zone (e.g., "srv-abc123")
            var body = new { type, name, content, ttl, proxied = false };
            var resp = await _http.PostAsync(
                $"zones/{_opt.ZoneId}/dns_records",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"),
                ct
            );
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"CF create failed: {await resp.Content.ReadAsStringAsync(ct)}");
        }
    }
}