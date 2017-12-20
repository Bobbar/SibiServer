using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.ComponentModel;


namespace SibiServer.Emailer
{
    public static class SMTPMailer
    {
        //private static List<Models.RequestApproval> messageQueue = new List<Models.RequestApproval>();
        private static List<Notification> messageQueue = new List<Notification>();
        private static bool mailSent = false;
        private static bool sendingMessage = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String notificationId = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", notificationId);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", notificationId, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
                DBFunctions.SetNotifySent(notificationId);
            }
            mailSent = true;
            sendingMessage = false;
            RemoveFromQueue(notificationId);
            ProcessQueue();
        }

        //public static void SendNewSMTP()
        //{
        //    var testAddress = "rblovell@fairfielddd.com";
        //    var host = "smtp.mail.fairfieldcountyohio.gov";

        //    var client = new SmtpClient(host);

        //    MailAddress from = new MailAddress(testAddress, "Bobby Lovell");

        //    MailAddress to = new MailAddress(testAddress);

        //    MailMessage message = new MailMessage(from, to);
        //    message.Body = "This is a test message from SMTP...";
        //    message.Subject = "SMTP Test";

        //    client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

        //    string userState = "1234";
        //    client.SendAsync(message, userState);

        //    //  message.Dispose();




        //}
        public static void SendNewSMTP(Notification notification)
        {
            var fromAddress = "noreply@fairfieldcountyohio.gov";

            var host = "smtp.mail.fairfieldcountyohio.gov";
            var client = new SmtpClient(host);

            MailAddress from = new MailAddress(fromAddress, "Sibi Acquisition Mailer");
            //MailAddress to = new MailAddress(notification.Approval.Approver.Email);
            // MailAddressCollection to = SetRecipients(notification);

            MailMessage message = new MailMessage();// (from.ToString(), to);
            message.From = from;
            SetRecipients(notification, message);

            message.Body = GetMessageBody(notification);// ApprovalRequestBody(notification.Approval.GUID);
            message.IsBodyHtml = true;
            message.Subject = GetSubjectText(notification);//"New Approval Request - " + notification.Approval.SibiRequest.Description;

            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            string userState = notification.GUID;
            sendingMessage = true;
            client.SendAsync(message, userState);

            //  message.Dispose();




        }

        private static void SetRecipients(Notification notification, MailMessage message)
        {
            // MailAddressCollection recipients = new MailAddressCollection();

            switch (notification.Type)
            {
                case NotificationType.APPROVAL:
                    message.To.Add(notification.Approver.Email);

                    break;

                case NotificationType.ACCEPTED:
                    message.To.Add(notification.Requestor.Email);
                    message.To.Add(notification.Approver.Email);

                    break;

                case NotificationType.REJECTED:
                    message.To.Add(notification.Requestor.Email);
                    message.To.Add(notification.Approver.Email);


                    break;

                case NotificationType.CHANGE:
                    message.To.Add(notification.Approver.Email);



                    break;

            }
            // return recipients;


        }

        private static string GetSubjectText(Notification notification)
        {
            switch (notification.Type)
            {
                case NotificationType.APPROVAL:
                    return "New Sibi Request Change Approval - " + notification.Approval.SibiRequest.Description;

                case NotificationType.ACCEPTED:
                    return "Approved Sibi Request Change - " + notification.Approval.SibiRequest.Description;

                case NotificationType.REJECTED:
                    return "Rejected Sibi Request Change - " + notification.Approval.SibiRequest.Description;

                case NotificationType.CHANGE:
                    return "Sibi Request Item Status Change";
                    //return string.Empty;

                    //TODO: Create a change only/new status message.

            }
            return string.Empty;

        }

        private static string GetMessageBody(Notification notification)
        {
            switch (notification.Type)
            {
                case NotificationType.APPROVAL:
                    return NewApprovalBody(notification.Approval);

                case NotificationType.ACCEPTED:
                    return AcceptBody(notification.Approval);

                case NotificationType.REJECTED:
                    return RejectBody(notification.Approval);

                case NotificationType.CHANGE:
                    return NewStatusBody(notification.RequestItem);
                    //return string.Empty;

                    //TODO: Create a change only/new status message.

            }
            return string.Empty;
        }

        private static string AcceptBody(Models.RequestApproval approval)
        {

            string bodyHTML = "";

            bodyHTML += "<p>" + approval.Approver.FullName + " has accepted the change request for " + approval.SibiRequest.Description + "</p>";
            bodyHTML += "<p><a href='http://localhost:57456/Sibi/Approval?id=" + approval.GUID + "' target='_blank' rel='noopener'>Click here to view the request.</a></p>";

            return bodyHTML;


        }

        private static string RejectBody(Models.RequestApproval approval)
        {

            string bodyHTML = "";

            bodyHTML += "<p>" + approval.Approver.FullName + " has REJECTED the change request for " + approval.SibiRequest.Description + "</p>";
            bodyHTML += "<p><a href='http://localhost:57456/Sibi/Approval?id=" + approval.GUID + "' target='_blank' rel='noopener'>Click here to view the request.</a></p>";

            return bodyHTML;

        }



        private static string NewApprovalBody(Models.RequestApproval approval)
        {

            string bodyHTML = "";

            bodyHTML += "<p>You have recieved a new Sibi change approval request.</p>";
            bodyHTML += "<p><a href='http://localhost:57456/Sibi/Approval?id=" + approval.GUID + "' target='_blank' rel='noopener'>Please click here to view the request.</a></p>";

            return bodyHTML;


        }

        private static string NewStatusBody(Models.SibiRequestItem requestItem)
        {

            string bodyHTML = "";

            bodyHTML += "<p>The status of the following Sibi Request Item has changed.</p>";
            bodyHTML += "<p>ItemID: " + requestItem.GUID + "  User: " + requestItem.User + "  Description: " + requestItem.Description + "  Status: " + requestItem.Status + " </p>";

            return bodyHTML;


        }

        public static void SendNewNotifications(List<Notification> notifications)
        {
            foreach (var item in notifications)
            {
                AddToQueue(item);
            }

            ProcessQueue();


        }


        //public static void SendNewApproval(Models.RequestApproval approval)
        //{

        //    AddToQueue(approval);

        //    ProcessQueue();
        //}

        private static void ProcessQueue()
        {
            if (!sendingMessage && messageQueue.Count > 0)
            {
                SendNewSMTP(messageQueue.First());
            }
            Task.Delay(500).Wait();
        }

        private static void RemoveFromQueue(string notificationId)
        {
            var removeItem = messageQueue.Find(m => m.GUID == notificationId);
            messageQueue.Remove(removeItem);
        }

        private static void AddToQueue(Notification notification)
        {
            if (!messageQueue.Contains(notification))
            {
                messageQueue.Add(notification);
            }
        }

    }
}
