using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WalletAdmin.Data;
using WalletAdmin.Models;
using WalletAdmin.Services;
using WalletAdmin.Models.Enums;
using Microsoft.AspNetCore.Authorization;
namespace WalletAdmin.Controllers
{
    [Authorize]
    public class WalletsController : Controller
    {
        private readonly WalletService _walletService;
        private readonly ApplicationDbContext _context;

        public WalletsController(WalletService walletService, ApplicationDbContext context)
        {
            _walletService = walletService;
            _context = context;
        }

        // GET: Wallets
        public async Task<IActionResult> Index()
        {
            return View(await _walletService.GetAllAsync());
        }

        // GET: Wallets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var wallet = await _walletService.GetByIdAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return View(wallet);
        }

        // GET: Wallets/Create
        public IActionResult Create(int? customerId)
        {
            ViewData["CustomerId"] = new SelectList(
                _context.Customers,
                "CustomerId",
                "Email",
                customerId
            );

            ViewBag.Currencies = Enum.GetValues<Currency>()
                .Select(c => new SelectListItem
                {
                    Value = c.ToString(),
                    Text = c.ToString()
                });

            return View();
        }


        // POST: Wallets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Wallet wallet)
        {
            try
            {
                await _walletService.CreateAsync(wallet);
                return RedirectToAction(
                    "ByCustomer",
                    "Wallets",
                    new { customerId = wallet.CustomerId }
                );
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewData["CustomerId"] = new SelectList(
                    _context.Customers,
                    "CustomerId",
                    "Email",
                    wallet.CustomerId);

                ViewBag.Currencies = Enum.GetValues<Currency>()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ToString(),
                        Text = c.ToString()
                    });

                return View(wallet);
            }
        }


        // GET: Wallets/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var wallet = await _walletService.GetByIdAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return View(wallet);
        }

        // POST: Wallets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _walletService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ByCustomer(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Wallets)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return NotFound();

            ViewBag.Customer = customer;
            return View(customer.Wallets);
        }
    }
}
