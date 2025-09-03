using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_payments.Controllers
{
    public class DebtController : Controller
    {
        // GET: DebtController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DebtController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DebtController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DebtController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DebtController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DebtController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DebtController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DebtController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
