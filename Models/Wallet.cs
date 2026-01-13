using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalletAdmin.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace WalletAdmin.Models
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }
        public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        [Required]
        public Currency Currency {get; set;}
        public ICollection<Transaction> Transactions {get; set;} = new List<Transaction>();
    }
}