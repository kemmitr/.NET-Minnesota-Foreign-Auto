using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarsByRuss.Models;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CarsByRuss.ViewModels
{
    public class CarAndServicesViewModel
    {
        public Car CarObj { get; set; }

        
        public int carId { get; set; }

        public string VIN { get; set; }
    
        public string Make { get; set; }

        public string Model { get; set; }
        public string Style { get; set; }

        public int Year { get; set; }
        public string UserId { get; set; }

  





        //this will contain records for the new services a user wishes to add
        public Service NewServiceObj { get; set; }
        //this will list all past services, IEnumerbles can hold whole tables and their associated data
        public IEnumerable<Service> PastServicesObj { get; set; }
        //list will hold a specific variable
        public List<ServiceType> ServiceTypesObj { get; set; }


    }
}
