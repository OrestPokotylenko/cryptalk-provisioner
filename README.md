# Cryptalk Provisioner

A lightweight **.NET 8 Minimal API** that automatically provisions unique subdomains for Cryptalk servers.  
When a new Cryptalk server is installed, the provisioner:

- Generates a unique subdomain (e.g., `srv-a1b2c3.cryptalk.live`)
- Creates a DNS A record in Cloudflare pointing to the server's public IP
- Returns the subdomain so the installer can configure automatic HTTPS via ACME DNS-01

---

## Features
- Simple .NET 8 Minimal API
- Cloudflare API integration for automated DNS record creation
- DNS-only mode for full decentralization (no traffic proxying)