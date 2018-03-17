using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarsByRuss.Data;
using CarsByRuss.Models;
using CarsByRuss.Utility;
using CarsByRuss.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;

namespace CarsByRuss.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ServicesController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Index(int carId)
        {
            var car = _db.Cars.FirstOrDefault(c => c.Id == carId);
            var model = new CarAndServicesViewModel
            {
                carId = car.Id,
                Make = car.Make,
                Model = car.Model,
                Style = car.Style,
                VIN = car.VIN,
                Year = car.Year,
                UserId = car.UserId,
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                //here we will grab the services and order by descending in the date they were added, and we will only
                //display 5 at a time. 
                PastServicesObj = _db.Services.Where(s => s.CarId == carId)
                    .OrderByDescending(s => s.DateAdded)
            };
            return View(model);
        }
        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult Create(int carId)
        {

            var car = _db.Cars.FirstOrDefault(c => c.Id == carId);
            var model = new CarAndServicesViewModel
            {
                carId = car.Id,
                Make =car.Make,
                Model=car.Model,
                Style = car.Style,
                VIN =car.VIN,
                Year = car.Year,
                UserId = car.UserId,
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                //here we will grab the services and order by descending in the date they were added, and we will only
                //display 5 at a time. 
                PastServicesObj = _db.Services.Where(s=>s.CarId ==carId)
                .OrderByDescending(s=> s.DateAdded).Take(5)
            };
            return View(model);
        }
        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarAndServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                //the carid and date will not passed with the view model so we need to add them here
                model.NewServiceObj.CarId = model.carId;
                model.NewServiceObj.DateAdded = DateTime.Now;
                //comment end
                _db.Add(model.NewServiceObj);
                await _db.SaveChangesAsync();
                return RedirectToAction("Create", new {carId = model.carId });
            }

            var car = _db.Cars.FirstOrDefault(c => c.Id == model.carId);
            var newModel = new CarAndServicesViewModel
            {
                
                carId = car.Id,
                Make = car.Make,
                Model = car.Model,
                Style = car.Style,
                VIN = car.VIN,
                Year = car.Year,
                UserId = car.UserId,
                //this will grab all the service types so they will be displayed in the drop down
                ServiceTypesObj = _db.ServiceTypes.ToList(),

                //here we will grab the services and order by descending in the date they were added, and we will only
                //display 5 at a time. 
                PastServicesObj = _db.Services.Where(s => s.CarId == model.carId)
                    .OrderByDescending(s => s.DateAdded).Take(5)
            };

            return View(newModel);
        }
        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _db.Services.Include(s => s.Car).Include(s => s.ServiceType)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }
        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Service model)
        {
            var serviceId = model.Id;
            var carId = model.CarId;
            var service = await _db.Services.SingleOrDefaultAsync(m => m.Id == serviceId);
            if (service == null)
            {
                return NotFound();
            }

            _db.Services.Remove(service);
            await _db.SaveChangesAsync();
            return RedirectToAction("Create",  new { carId = model.CarId });

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