namespace TipoCambioMVC.Security
{
    using BCrypt.Net;
    using Isopoh.Cryptography.Argon2;
    using Microsoft.Extensions.Options;

    // Aliases para evitar choques de nombres
    using Argon2Class = Isopoh.Cryptography.Argon2.Argon2;
    using Argon2TypeEnum = Isopoh.Cryptography.Argon2.Argon2Type;
    public sealed class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordOptions _opt;
        public PasswordHasherService(IOptions<PasswordOptions> opt) => _opt = opt.Value;

        public string Hash(string password) =>
            _opt.Algorithm.Equals("BCrypt", StringComparison.OrdinalIgnoreCase)
                ? BCrypt.HashPassword(password, _opt.BCrypt.WorkFactor)
                : HashArgon2id_Static(password);

        public bool Verify(string password, string encodedHash)
        {
            if (encodedHash.StartsWith("$2"))        // BCrypt
                return BCrypt.Verify(password, encodedHash);

            if (encodedHash.StartsWith("$argon2"))   // Argon2 (Isopoh)
                return Argon2Class.Verify(encodedHash, password);

            // Fallback según configuración
            return _opt.Algorithm.Equals("BCrypt", StringComparison.OrdinalIgnoreCase)
                ? BCrypt.Verify(password, encodedHash)
                : Argon2Class.Verify(encodedHash, password);
        }

        // Usa la sobrecarga ESTÁTICA de Isopoh:
        // Hash(string password, int timeCost, int memoryCost, int parallelism, Argon2Type type, int saltLength, SecureArrayCall? = null)
        private string HashArgon2id_Static(string password)
        {
            var a = _opt.Argon2id;
            return Argon2Class.Hash(
                password,
                a.Iterations,
                a.MemoryKb,  
                a.Parallelism,
                Argon2TypeEnum.HybridAddressing, 
                a.SaltSize                     
            );
        }
    }
}




