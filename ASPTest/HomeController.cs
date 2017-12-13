﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPTest
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {


            return View();
        }

        public IActionResult Welcome(string name)
        {
            ViewData["Message"] = name;


            return View();
        }
        // [HttpGet]
        public IActionResult Approval(string id)
        {


            ViewData["id"] = id;
            ViewData["state"] = "prepost";
            Models.RequestApproval approval = new Models.RequestApproval(id);
            //MySQLCommsOLD sqlContext = HttpContext.RequestServices.GetService(typeof(MySQLCommsOLD)) as MySQLCommsOLD;
            //sqlContext.PopRequestData(approval);
            DBFunctions.PopRequestData(ref approval);

            return View(approval);
        }

        [HttpPost]
        public ActionResult Approval(Models.RequestApproval r, string accept)
        {
            Console.WriteLine("Approve CLicked!  " + accept);
            ViewData["state"] = "posted";
            //Do the actual approval logic.
            // MySQLCommsOLD sqlContext = HttpContext.RequestServices.GetService(typeof(MySQLCommsOLD)) as MySQLCommsOLD;

            DBFunctions.PopRequestData(ref r);
            bool success = DBFunctions.ApproveRequest(r);//sqlContext.ApproveRequest(r.GUID);
            r.PostSuccess = success;


            return View(r);

        }


        //[HttpPost]
        ////public ActionResult Approved(string submit)
        //public void Approved(Models.RequestApproval r)
        //{
        //    ModelState.Remove("Success");
        //    r.Success = true;

        //    Console.WriteLine("Approve CLicked!  ");
        //    //return View();
        //}
    }
}