using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API.BOBERTO.AUTHENTICATION.WEB
{
    public class HealthCheck
    {
        public DateTime StartAt { get; set; }
        public TimeSpan LastDeploy { get; set; }
        public string Environment { get; set; }
        public HealthCheck()
        {
            StartAt = DateTime.Now;
            Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "PRODUCTION";
        }
        public TimeSpan GetUpTime()
        {
            var upTime = DateTime.Now.Subtract(StartAt);
            LastDeploy = upTime;
            return upTime;
        }
        public string GetMessage()
        {
            return "Last deploy " + StartAt.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
