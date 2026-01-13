using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletAdmin.Data;
using WalletAdmin.Models;
using Microsoft.AspNetCore.Authorization;


namespace WalletAdmin.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.CustomerCount = await _context.Customers.CountAsync();
            ViewBag.WalletCount = await _context.Wallets.CountAsync();
            ViewBag.TransactionCount = await _context.Transactions.CountAsync();

            return View();
        }
    }
}