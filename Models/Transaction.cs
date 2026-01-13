using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalletAdmin.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace WalletAdmin.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet {get; set;} = null!;
        public decimal Amount {get; set;}
        public string? Description {get; set;}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal BalanceBefore {get; set;} 
        public decimal BalanceAfter { get; set; }
        public TransactionType TransactionType { get; set; }


        // Denormalized for reporting and quick querying.
        // Must always match Wallet.Currency
        public Currency Currency { get; set; }

    }
}