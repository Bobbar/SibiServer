using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AssetManager;

namespace SibiServer.Emailer
{
    public static class DBPoller
    {

        public static void RequestMonitor()
        {

            int maxloops = 1000;
            int loops = 0;

            do
            {
                loops++;

                //List<Models.RequestApproval> notifyList; //= new List<Models.RequestApproval>();
                List<Notification> notifyList = GetNotifications();

                //AddNewApprovals(ref notifyList);
                //AddNewAccepts(ref notifyList);
                //AddNewRejects(ref notifyList);
                //AddNewNotifys(ref notifyList);



                SMTPMailer.SendNewNotifications(notifyList);

                //using (var results = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiApprovalColumns.TableName + " WHERE " + SibiApprovalColumns.NotifySent +  " = '0' AND " + SibiApprovalColumns.Status + " = 'new'"))
                //{
                //    if (results.Rows.Count > 0)
                //    {
                //        //notifyList = new List<Models.RequestApproval>();
                //        notifyList = new List<Notification>();

                //        foreach (DataRow row in results.Rows)
                //        {
                //            var newRequst = new Models.RequestApproval(row);
                //            DBFunctions.PopRequestData(ref newRequst);
                //            // notifyList.Add(newRequst);
                //            notifyList.Add(new Notification(NotificationType.NewApproval, newRequst));


                //        }

                //        ProcessRequests(notifyList);


                //    }


                //}

                Task.Delay(5000).Wait();

            } while (loops < maxloops);



        }

        private static List<Notification> GetNotifications()
        {
            var tempList = new List<Notification>();
            var notificationsQuery = "SELECT * FROM " + NotificationColumns.TableName + " WHERE " + NotificationColumns.Sent + " = '0'";
            using (var notificationsTable = DBFactory.GetDatabase().DataTableFromQueryString(notificationsQuery))
            {
                foreach (DataRow notifRow in notificationsTable.Rows)
                {
                    tempList.Add(new Notification(notifRow));
                }
            }
            return tempList;
        }

        //private static void AddNewApprovals(ref List<Notification> notifications)
        //{
        //    using (var results = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiApprovalColumns.TableName + " WHERE " + SibiApprovalColumns.NotifySent + " = '0' AND " + SibiApprovalColumns.Status + " = 'pending'"))
        //    {
        //        if (results.Rows.Count > 0)
        //        {
        //            foreach (DataRow row in results.Rows)
        //            {
        //                var newRequst = new Models.RequestApproval(row);
        //                DBFunctions.PopRequestData(ref newRequst);
        //                notifications.Add(new Notification(NotificationType.NewApproval, newRequst));
        //            }
        //        }
        //    }
        //}

        //private static void AddNewAccepts(ref List<Notification> notifications)
        //{
        //    using (var results = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiApprovalColumns.TableName + " WHERE " + SibiApprovalColumns.NotifySent + " = '0' AND " + SibiApprovalColumns.Status + " = 'accept'"))
        //    {
        //        if (results.Rows.Count > 0)
        //        {
        //            foreach (DataRow row in results.Rows)
        //            {
        //                var newRequst = new Models.RequestApproval(row);
        //                DBFunctions.PopRequestData(ref newRequst);
        //                notifications.Add(new Notification(NotificationType.Accept, newRequst));
        //            }
        //        }
        //    }
        //}

        //private static void AddNewRejects(ref List<Notification> notifications)
        //{
        //    using (var results = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiApprovalColumns.TableName + " WHERE " + SibiApprovalColumns.NotifySent + " = '0' AND " + SibiApprovalColumns.Status + " = 'reject'"))
        //    {
        //        if (results.Rows.Count > 0)
        //        {
        //            foreach (DataRow row in results.Rows)
        //            {
        //                var newRequst = new Models.RequestApproval(row);
        //                DBFunctions.PopRequestData(ref newRequst);
        //                notifications.Add(new Notification(NotificationType.Reject, newRequst));
        //            }
        //        }
        //    }
        //}

        //private static void AddNewNotifys(ref List<Notification> notifications)
        //{
        //    using (var results = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiApprovalColumns.TableName + " WHERE " + SibiApprovalColumns.NotifySent + " = '0' AND " + SibiApprovalColumns.Status + " = 'notify'"))
        //    {
        //        if (results.Rows.Count > 0)
        //        {
        //            foreach (DataRow row in results.Rows)
        //            {
        //                var newRequst = new Models.RequestApproval(row);
        //                DBFunctions.PopRequestData(ref newRequst);
        //                notifications.Add(new Notification(NotificationType.NotifyOnly, newRequst));
        //            }
        //        }
        //    }
        //}



        public static async void StartPoller()
        {
            await Task.Run(() =>
                  {
                      RequestMonitor();
                  });
        }




        //private static void ProcessRequests(List<Models.RequestApproval> approvals)
        //{

        //    foreach (var approval in approvals)
        //    {

        //        SMTPMailer.SendNewApproval(approval);

        //        //if (!request.ApprovalSent && request.ApprovalStatus == "pending")
        //        //{
        //        //    // Send email and set Sent flag.



        //        //} 



        //    }

        //}
    }
}
