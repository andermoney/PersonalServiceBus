using System.Configuration;
using PersonalServiceBus.RSS.Core.Domain.Interface;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public class WebConfiguration : IConfiguration
    {
        public string RavenDBUrl
        {
            get { return ConfigurationManager.AppSettings["RavenDBUrl"]; }
        }

        public string PasswordRegex
        {
            get { return ConfigurationManager.AppSettings["PasswordRegex"]; }
        }

        public string PasswordMessage
        {
            get { return ConfigurationManager.AppSettings["PasswordMessage"]; }
        }
    }
}