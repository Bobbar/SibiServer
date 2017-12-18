using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace SibiServer.Emailer
{
    public static class DBPoller
    {

        public static void RequestMonitor()
        {

            int maxloops = 50;
            int loops = 0;

            do
            {
                loops++;

                List<Models.RequestApproval> requestList; //= new List<Models.RequestApproval>();


                using (var results = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM sibi_request_items_approvals WHERE approval_sent = '0' AND approval_status = 'new'"))
                {
                    if (results.Rows.Count > 0)
                    {
                        requestList = new List<Models.RequestApproval>();

                        foreach (DataRow row in results.Rows)
                        {
                            var newRequst = new Models.RequestApproval(row);
                            DBFunctions.PopRequestData(ref newRequst);
                            requestList.Add(newRequst);


                        }

                        ProcessRequests(requestList);


                    }


                }

                Task.Delay(5000).Wait();

            } while (loops < maxloops);



        }

        public static async void StartPoller()
        {
          await Task.Run(() =>
                {
                    RequestMonitor();
                });
        }

        private static void ProcessRequests(List<Models.RequestApproval> approvals)
        {

            foreach (var approval in approvals)
            {

                SMTPMailer.SendNewApproval(approval);

                //if (!request.ApprovalSent && request.ApprovalStatus == "pending")
                //{
                //    // Send email and set Sent flag.



                //} 



            }

        }
    }
}
