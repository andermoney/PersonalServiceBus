namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface ICryptography
    {
        string CreateHash(string unhash);
        bool MatchHash(string hashdata, string unhash);
    }
}