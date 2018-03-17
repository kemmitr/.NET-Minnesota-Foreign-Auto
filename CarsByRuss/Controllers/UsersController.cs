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
    public class UsersController : Controller
    {
        private ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }
        //here we pass option and search from the view
        public IActionResult Index(string option=null, string search=null)
        {
            var users = _db.Users.ToList();
            //if the user selects radiobutton email and the search field is not empty then line 26 will run
            if (option == "email" && search != null)
            {
      //now we go into the database and fetch the data associated with the email entered, we also call on the to lower method to make all text enetered into lowercase
                users = _db.Users.Where(u => u.Email.ToLower().Contains(search.ToLower())).ToList();
            }
            else
            {
                
                if (option == "name" && search != null)
                {
                    users = _db.Users.Where(u => u.FirstName.ToLower().Contains(search.ToLower())
                                                 || u.LastName.ToLower().Contains(search.ToLower())).ToList();
                }

                else
                {
                    if (option == "phone" && search != null)
                    {
                        users = _db.Users.Where(u => u.PhoneNumber.Contains(search)).ToList();
                    }
                }
            }

            return View(users);
        }

        public IActionResult Edit(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var user = _db.Users.Find(Id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user )
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", user);
            }
            else
            {
                var userInDb = await _db.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
                userInDb.FirstName = user.FirstName;
                userInDb.LastName = user.LastName;
                userInDb.City = user.City;
                userInDb.Address = user.Address;
                userInDb.PostalCode = user.PostalCode;
                userInDb.Email = user.Email;
                userInDb.PhoneNumber = user.PhoneNumber;

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }


        }

        public async Task<IActionResult> Details(string Id)
        {
            if (Id == null)
            {
                return NotFound();

            }

            ApplicationUser user = await _db.Users.SingleOrDefaultAsync( u => u.Id == Id);
            return View(user);
        }

        
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var user = await  _db.Users.SingleOrDefaultAsync(u=> u.Id == Id);
            var cars = _db.Cars.Where(u => u.UserId == user.Id);
            List<Car> carList = cars.ToList();

            foreach (var car in carList)
            {
                var services = _db.Services.Where(s => s.CarId == car.Id);
                _db.Services.RemoveRange(services);
            }
            _db.Cars.RemoveRange();
             _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
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