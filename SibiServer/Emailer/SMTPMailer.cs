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
        private static List<Models.RequestApproval> messageQueue = new List<Models.RequestApproval>();
        private static bool mailSent = false;
        private static bool sendingMessage = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String approvalId = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", approvalId);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", approvalId, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
                DBFunctions.SetNotifySent(approvalId);
            }
            mailSent = true;
            sendingMessage = false;
            RemoveFromQueue(approvalId);
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
        public static void SendNewSMTP(Models.RequestApproval approval)
        {
            var fromAddress = "noreply@fairfieldcountyohio.gov";

            var host = "smtp.mail.fairfieldcountyohio.gov";
            var client = new SmtpClient(host);

            MailAddress from = new MailAddress(fromAddress, "Sibi Acquisition Mailer");
            MailAddress to = new MailAddress(approval.Approver.Email);

            MailMessage message = new MailMessage(from, to);
            message.Body = NotifyBody(approval.GUID);
            message.IsBodyHtml = true;
            message.Subject = "New Approval Request - " + approval.SibiRequest.Description;

            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            string userState = approval.GUID;
            sendingMessage = true;
            client.SendAsync(message, userState);

            //  message.Dispose();




        }

        private static string NotifyBody(string id)
        {

            string bodyHTML = "";

            bodyHTML += "<p>You have recieved a new Sibi change approval request.</p>";
            bodyHTML += "<p><a href='http://localhost:57457/Home/Approval?id=" + id + "' target='_blank' rel='noopener'>Please click here to view the request.</a></p>";

            return bodyHTML;


        }

        public static void SendNewApproval(Models.RequestApproval approval)
        {

            AddToQueue(approval);

            ProcessQueue();
        }

        private static void ProcessQueue()
        {
            if (!sendingMessage && messageQueue.Count > 0)
            {
                SendNewSMTP(messageQueue.First());
            }
            
        }

        private static void RemoveFromQueue(string approvalId)
        {
            var removeItem = messageQueue.Find(m => m.GUID == approvalId);
            messageQueue.Remove(removeItem);
        }

        private static void AddToQueue(Models.RequestApproval approval)
        {
            if (!messageQueue.Contains(approval))
            {
                messageQueue.Add(approval);
            }
        }

    }
}
