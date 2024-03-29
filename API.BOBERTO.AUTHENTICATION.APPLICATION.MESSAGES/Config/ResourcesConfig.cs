﻿namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config
{
    public class ResourcesConfig
    {
        public IEnumerable<ResourceOptionConfig> Resources { get; set; }
    }
    public class ResourceOptionConfig
    {
        public string Key { get; set; }
        public bool Enabled { get; set; }
    }
}
