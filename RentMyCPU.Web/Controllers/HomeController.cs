﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
 

namespace RentMyCPU.Backend.Controllers
{
    public class HomeController : Controller
    { 
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
