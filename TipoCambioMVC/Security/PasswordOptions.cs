namespace TipoCambioMVC.Security
{
    public sealed class PasswordOptions
    {
        public string Algorithm { get; set; } = "Argon2id";
        public BCryptOptions BCrypt { get; set; } = new();
        public Argon2Options Argon2id { get; set; } = new();

        public sealed class BCryptOptions { public int WorkFactor { get; set; } = 12; }
        public sealed class Argon2Options
        {
            public int Iterations { get; set; } = 3;
            public int MemoryKb { get; set; } = 65536; // 64MB
            public int Parallelism { get; set; } = 2;
            public int SaltSize { get; set; } = 16;
        }
    }
}
