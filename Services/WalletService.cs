using Microsoft.EntityFrameworkCore;
using WalletAdmin.Data;
using WalletAdmin.Models;

namespace WalletAdmin.Services
{
    public class WalletService
    {
         private readonly ApplicationDbContext _context;

        public WalletService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Wallet>> GetAllAsync()
        {
            var applicationDbContext = _context.Wallets.Include(w => w.Customer);
            return await applicationDbContext.ToListAsync();
        }
        public async Task<Wallet?> GetByIdAsync(int id)
        {
            return await _context.Wallets
                .Include(w => w.Customer)
                .FirstOrDefaultAsync(m => m.WalletId == id);
        }
        public async Task CreateAsync(Wallet wallet)
        {
            var exists = await _context.Wallets.AnyAsync(w =>
                w.CustomerId == wallet.CustomerId &&
                w.Currency == wallet.Currency);

            if (exists)
                throw new InvalidOperationException("Customer already has a wallet for this currency.");
            wallet.Balance = 0;
            wallet.CreatedAt = DateTime.UtcNow;
            wallet.UpdatedAt = DateTime.UtcNow;
            _context.Add(wallet);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Wallets.AnyAsync(e => e.WalletId == id);
        }
        public async Task DeleteAsync(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null) return;
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }


    }
}
