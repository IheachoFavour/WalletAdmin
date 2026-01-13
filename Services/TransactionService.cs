using Microsoft.EntityFrameworkCore;
using WalletAdmin.Data;
using WalletAdmin.Models;
using WalletAdmin.Models.Enums;

namespace WalletAdmin.Services
{
    public class TransactionService
    {
         private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .ThenInclude(w => w.Customer)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Transaction>> GetByWalletIdAsync(int walletId)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        public async Task<Transaction> CreditAsync (int walletId, decimal amount, string? description = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");
            
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");
            var balanceBefore = wallet.Balance;
            var balanceAfter = balanceBefore+amount;
            var transaction = new Transaction
            {
                WalletId = wallet.WalletId,
                Amount = amount,
                TransactionType = TransactionType.Credit,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                CreatedAt = DateTime.UtcNow,
                Description = description,
                Currency = wallet.Currency
            };
            wallet.Balance = balanceAfter;
            wallet.UpdatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return transaction;
        }

        public async Task<Transaction> DebitAsync(int walletId, decimal amount, string? description = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.WalletId == walletId);
            if(wallet == null)
                throw new KeyNotFoundException("Wallet not Found.");
            if (amount> wallet.Balance)
                throw new InvalidOperationException("Insufficient balance.");
            var balanceBefore = wallet.Balance;
            var balanceAfter = balanceBefore - amount;
            var transaction = new Transaction
            {
                WalletId = wallet.WalletId,
                Amount = amount,
                TransactionType = TransactionType.Debit,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                Currency = wallet.Currency

            };
            wallet.Balance = balanceAfter;
            wallet.UpdatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return transaction;
        }
    }
}