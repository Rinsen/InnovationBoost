﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Rinsen.InnovationBoost.Controllers
{
    public class IdentitiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}