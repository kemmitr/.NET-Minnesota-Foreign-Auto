using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarsByRuss.Data;
using CarsByRuss.Models;
using CarsByRuss.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarsByRuss.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class ServiceTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ServiceTypesController( ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.ServiceTypes.ToList());
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var service = await _db.ServiceTypes.SingleOrDefaultAsync(m => m.Id == Id);
            return View(service);
        }
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var service = await _db.ServiceTypes.SingleOrDefaultAsync(m => m.Id == Id);
            return View(service);
        }

        public async Task<IActionResult> Delete(int Id)
        {
           

            var service = _db.ServiceTypes.Find(Id);
            _db.ServiceTypes.Remove(service);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, ServiceType serviceType)
        {
            if (Id != serviceType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(serviceType);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(serviceType);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceType ServiceType)
        {
            if (ModelState.IsValid)
            {
                _db.Add(ServiceType);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ServiceType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }
    }
}