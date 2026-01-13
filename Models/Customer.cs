using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalletAdmin.Models.Enums;
namespace WalletAdmin.Models
{
   public class Customer
{
    public int CustomerId { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}

}
