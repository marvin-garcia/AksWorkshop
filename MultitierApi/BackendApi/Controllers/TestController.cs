using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/Backend/ServerName
        [HttpGet("ServerName")]
        public string GetServerName()
        {
            return Environment.MachineName;
        }

        // GET: api/Backend/DateTime
        [HttpGet("DateTime/{timezone}")]
        public string GetDateTime(string timezone = null)
        {
            if (string.IsNullOrEmpty(timezone))
                return DateTime.UtcNow.ToString();
            else
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezone)).ToString();
        }
    }
}
