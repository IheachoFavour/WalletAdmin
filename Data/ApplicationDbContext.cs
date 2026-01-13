using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WalletAdmin.Models;

namespace WalletAdmin.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Customer> Customers {get; set;} = null!;
    public DbSet<Wallet> Wallets {get; set;} = null!;
    public DbSet<Transaction> Transactions {get; set;} = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Customer -> Wallets
        builder.Entity<Wallet>()
            .HasOne(w => w.Customer)
            .WithMany(c => c.Wallets)
            .HasForeignKey(w => w.CustomerId)
            .OnDelete(DeleteBehavior.Restrict); //“If someone tries to delete a Customer that has Wallets → throw an error.”
        
        //Wallet -> Transactions
        builder.Entity<Transaction>()
            .HasOne(t => t.Wallet)
            .WithMany (w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        // One wallet per customer per currency
        builder.Entity<Wallet>()
            .HasIndex(w => new { w.CustomerId, w.Currency })
            .IsUnique(); //A customer cannot have two wallets with the same currency.

    }

}
