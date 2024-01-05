using AptitudeWebApp.Repository;
using System.Security.Cryptography;

namespace AptitudeWebApp.Service
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly ILogger<PasswordHasher> _logger;

        public PasswordHasher (ILogger<PasswordHasher> logger)
        {
            _logger = logger;
        }
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private static char Delimiter = ';';
        public string Hash(string password)
        {
            try
            {
                var salt = RandomNumberGenerator.GetBytes(SaltSize);
                var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
                return String.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PasswordHasher: {ex.Message}");
            }

            return "";
        }
        public bool Verify(string passwordHash, string inputPassword)
        {
            try
            {
                var elements = passwordHash.Split(Delimiter);
                var salt = Convert.FromBase64String(elements[0]);
                var hash = Convert.FromBase64String(elements[1]);

                var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iterations, _hashAlgorithmName, KeySize);

                return CryptographicOperations.FixedTimeEquals(hash, hashInput);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PasswordVerify: {ex.Message}");
            }

            return false;
        }
    }
}
