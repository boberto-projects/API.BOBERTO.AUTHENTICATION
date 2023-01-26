namespace api_authentication_boberto
{
    public class ApiCicloDeVida
    {
        public DateTime StartAt { get; set; }
        public TimeSpan LastDeploy { get; set; }
        public string Environment { get; set; }

        public ApiCicloDeVida()
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
    }
}
