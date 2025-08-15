using Cryptalk_Provisioner.Models;
using Cryptalk_Provisioner.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptalk_Provisioner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly AllocateSubdomainHandler _handler;
        private readonly string _authKey;

        public RegisterController(AllocateSubdomainHandler handler, IConfiguration config)
        {
            _handler = handler;
            _authKey = config["AuthKey"] ?? throw new Exception("AuthKey missing");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AllocateRequest req, CancellationToken ct)
        {
            if (!Request.Headers.TryGetValue("X-Auth-Key", out var key) || key != _authKey)
                return Unauthorized(new { error = "unauthorized" });

            if (string.IsNullOrWhiteSpace(req.InstallId))
                return BadRequest(new { error = "installId required" });

            var res = await _handler.HandleAsync(req, ct);
            Console.WriteLine($"[register] returning: {res.Subdomain}");
            return Ok(new { subdomain = res.Subdomain });
        }
    }
}