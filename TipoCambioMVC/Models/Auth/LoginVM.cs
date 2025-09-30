﻿using System.ComponentModel.DataAnnotations;

namespace TipoCambioMVC.Models.Auth
{
    public class LoginVM
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, DataType(DataType.Password)] public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
