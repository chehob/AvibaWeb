﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvibaWeb.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ViewResult Index()
        {
            return View();
        }
    }
}