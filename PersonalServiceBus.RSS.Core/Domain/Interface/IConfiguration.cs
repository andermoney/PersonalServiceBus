namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IConfiguration
    {
        string RavenDBUrl { get; }
        string PasswordRegex { get; }
        string PasswordMessage { get; }
    }
}