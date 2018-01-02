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
            DBFunctions.PopApprovalData(ref approval);
            approval.ApprovalResponse = "none";

            return View(approval);
        }

        [HttpPost]
        public ActionResult Approval(Models.RequestApproval r)
        {

            var response = r.ApprovalResponse;
            Console.WriteLine("Approve CLicked!  " + response);
            ViewData["state"] = "posted";
            ViewData["response"] = response;
            //  ViewBag.Approval = r;


            if (response == "accept")
            {
                DBFunctions.PopApprovalData(ref r);
                bool success = DBFunctions.ApproveRequest(r);//sqlContext.ApproveRequest(r.GUID);
                r.PostSuccess = success;
                return View(r);
            }
            else if (response == "reject")
            {
                DBFunctions.PopApprovalData(ref r);
                bool success = DBFunctions.RejectRequest(r);//sqlContext.ApproveRequest(r.GUID);
                r.PostSuccess = success;
                return View(r);
            }
            return View(r);


        }

        [HttpPost]
        public IActionResult Approve([FromBody] Models.RequestApproval r)
        {
            //TODO: Create method within DataMapObject to update database with current object values.
            ViewData["state"] = "posted";
            ViewData["response"] = "accept";

            var postApproval = new Models.RequestApproval(r.GUID);
            DBFunctions.PopApprovalData(ref postApproval);
            postApproval.Note = r.Note;
            bool success = DBFunctions.ApproveRequest(postApproval);
            postApproval.PostSuccess = success;
            return Json(postApproval);

        }


        [HttpPost]
        public IActionResult Reject([FromBody] Models.RequestApproval r)
        {

            ViewData["state"] = "posted";
            ViewData["response"] = "reject";

            var postApproval = new Models.RequestApproval(r.GUID);
            DBFunctions.PopApprovalData(ref postApproval);
            postApproval.Note = r.Note;
            bool success = DBFunctions.RejectRequest(postApproval);
            postApproval.PostSuccess = success;
            return Json(postApproval);
                     

        }

        [HttpGet]
        public PartialViewResult GetApprovalData(string approvalId)
        {
            var approval = new Models.RequestApproval(approvalId);
            DBFunctions.PopApprovalData(ref approval);

            return PartialView("RequestPartial", approval);

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
