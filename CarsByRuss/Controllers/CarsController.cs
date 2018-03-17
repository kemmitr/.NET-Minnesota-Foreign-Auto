using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CarsByRuss.Data;
using CarsByRuss.Models;
using CarsByRuss.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace CarsByRuss.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CarsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index( string userId = null)
        {
            if (userId == null)
            {
                //this will only be called if user is a guest user
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var model = new CarAndCustomerViewModel
            {
                Cars = _db.Cars.Where(c => c.UserId == userId),
                UserObj = _db.Users.FirstOrDefault(u=>u.Id==userId)
                
            };
            return View(model);
        }

        //we will be creating a new car for a specific user so we will recieve the userid passed from the view
        //so we must create a string od userId
        public IActionResult Create(string userId)
        {
            //our foreign key in the Car table is UserId and we will set that equal to the userId data we recieved
            //through the param fields
            Car carObj = new Car
            {
                Year = DateTime.Now.Year,
                UserId = userId
            };
            //we will pass the car obj so so we are able to pass the user Id again when we are done adding the new car
            return View(carObj);
        }

        //create post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                _db.Add(car);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", new {userId = car.UserId});
            }

            return View(car);

        }

        //carid
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _db.Cars
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            return View(car);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _db.Cars
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            return View(car);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(car);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", new {userId = car.UserId});
            }

            return View(car);
        }

        public async Task<IActionResult> Delete(int id)
        {


            var car = await _db.Cars.SingleOrDefaultAsync(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", new {userId = car.UserId});

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