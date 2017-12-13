using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ASPTest.Emailer
{
    public class DBPoller
    {

        public void RequestMonitor()
        {

            int maxloops = 10;
            int loops = 0;

            do
            {
                loops++;

                List<Models.RequestApproval> requestList; //= new List<Models.RequestApproval>();


                using (var results = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM sibi_request_items_approvals"))
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



                    }


                }









                    Task.Delay(5000).Wait();

            } while (loops < maxloops);


                    
        }

        private void ProcessRequest(List<Models.RequestApproval> requests)
        {

            foreach (var request in requests)
            {

                if (!request.ApprovalSent && request.ApprovalStatus == "pending")
                {
                    // Send email and set Sent flag.
                } 



            }

        }
    }
}
