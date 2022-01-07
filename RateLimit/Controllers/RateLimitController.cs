using Microsoft.AspNetCore.Mvc;
using System;

namespace RateLimit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateLimitController : ControllerBase
    {
        [HttpGet("")]
        public string Get()
        {
            return $"Consumido em: {DateTime.UtcNow:o}";
        }
    }
}
