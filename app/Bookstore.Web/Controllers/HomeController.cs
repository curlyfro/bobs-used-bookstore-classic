﻿using System.Diagnostics;
using Bookstore.Domain.Books;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bookstore.Web.Models;
using Bookstore.Web.Models.Home;

namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IBookService bookService;

        public HomeController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        public async Task<ActionResult> Index()
        {
            var books = await bookService.ListBestSellingBooksAsync(4);

            return View(new HomeIndexViewModel(books));
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Search()
        {
            return RedirectToAction("Index", "Search");
        }

        public ActionResult Cart()
        {
            return RedirectToAction("Index", "ShoppingCart");
        }

        public ActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id });
        }
    }
}
