using AptitudeWebApp.Entities;

namespace AptitudeWebApp.Repository
{
    public interface IPasswordHasher
    {
        bool Verify(string passwordHash, string inputPassword);
        string Hash(string password);
    }   
}
