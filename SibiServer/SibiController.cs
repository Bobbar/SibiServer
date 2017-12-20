using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SibiServer
{
    public class SibiController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(string userid)
        {

            Console.WriteLine(userid);
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
        public ActionResult Approval(Models.RequestApproval r, string response)
        {
            Console.WriteLine("Approve CLicked!  " + response);
            ViewData["state"] = "posted";
            ViewData["response"] = response;
          //  ViewBag.Approval = r;

            if (response == "accept")
            {
                DBFunctions.PopRequestData(ref r);
                bool success = DBFunctions.ApproveRequest(r);//sqlContext.ApproveRequest(r.GUID);
                r.PostSuccess = success;
                return View(r);
            }
            else if (response == "reject")
            {
                DBFunctions.PopRequestData(ref r);
                bool success = DBFunctions.RejectRequest(r);//sqlContext.ApproveRequest(r.GUID);
                r.PostSuccess = success;
                return View(r);
            }
            return View(r);

           
        }

        public IActionResult MyApprovals(Models.RequestApproval r, string approverId)
        {
            //var m = (Models.RequestApproval)ViewBag.Approval;
            Console.WriteLine(r.ApproverID);
            ViewBag.ApprovalsTable = DBFunctions.GetApprovalsTable(approverId);
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
