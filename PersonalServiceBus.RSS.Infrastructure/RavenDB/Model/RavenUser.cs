namespace PersonalServiceBus.RSS.Infrastructure.RavenDB.Model
{
    public class RavenUser
    {
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }
    }
}