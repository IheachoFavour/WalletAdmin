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
    public class TransactionsController: Controller
    {
        private readonly TransactionService _transactionService;
        private readonly WalletService _walletService;

        private readonly ApplicationDbContext _context;
        public TransactionsController(TransactionService transactionService, ApplicationDbContext context, WalletService walletService)
        {
            _transactionService = transactionService;
            _context = context;
            _walletService = walletService;
        }
        [HttpGet]
        public async Task<IActionResult> Credit(int walletId)
        {
            var wallet = await _walletService.GetByIdAsync(walletId);
            if (wallet == null)
                return NotFound();

            ViewBag.WalletId = wallet.WalletId;
            ViewBag.CustomerEmail = wallet.Customer.Email;
            ViewBag.Currency = wallet.Currency;
            ViewBag.Balance = wallet.Balance;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Credit(int walletId, decimal amount, string? description)
        {
            try
            {
                await _transactionService.CreditAsync(walletId, amount, description);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var wallet = await _walletService.GetByIdAsync(walletId);
                if (wallet == null)
                    return NotFound();

                ViewBag.WalletId = wallet.WalletId;
                ViewBag.CustomerEmail = wallet.Customer.Email;
                ViewBag.Currency = wallet.Currency;
                ViewBag.Balance = wallet.Balance;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Debit(int walletId)
        {
            var wallet = await _walletService.GetByIdAsync(walletId);
            if (wallet == null)
                return NotFound();

            ViewBag.WalletId = wallet.WalletId;
            ViewBag.CustomerEmail = wallet.Customer.Email;
            ViewBag.Currency = wallet.Currency;
            ViewBag.Balance = wallet.Balance;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Debit(int walletId, decimal amount, string? description)
        {
            try
            {
                await _transactionService.DebitAsync(walletId, amount, description);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Show the error clearly to the ops team
                ModelState.AddModelError(string.Empty, ex.Message);
                var wallet = await _walletService.GetByIdAsync(walletId);
                if (wallet == null)
                    return NotFound();

                ViewBag.WalletId = wallet.WalletId;
                ViewBag.CustomerEmail = wallet.Customer.Email;
                ViewBag.Currency = wallet.Currency;
                ViewBag.Balance = wallet.Balance;
                return View();
            }
        }
        public async Task<IActionResult> Index()
        {
            return View(await _transactionService.GetAllAsync());
        }
        public async Task<IActionResult> Wallet(int walletId)
        {
            return View(await _transactionService.GetByWalletIdAsync(walletId));
        }

        public async Task<IActionResult> ByWallet(int walletId)
        {
            var wallet = await _walletService.GetByIdAsync(walletId);
            if (wallet == null)
                return NotFound();

            var transactions = await _transactionService.GetByWalletIdAsync(walletId);

            ViewBag.Wallet = wallet;
            ViewBag.CustomerEmail = wallet.Customer.Email;
            ViewBag.Currency = wallet.Currency;
            ViewBag.CustomerId = wallet.CustomerId;
            ViewBag.WalletId = walletId;
            ViewBag.Balance = wallet.Balance;

            return View(transactions);
        }

    }
}