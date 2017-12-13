using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.ComponentModel;


namespace ASPTest.Emailer
{
    public static class SMTPMailer
    {
        private static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        public static void SendNewSMTP()
        {
            var testAddress = "rblovell@fairfielddd.com";
            var host = "smtp.mail.fairfieldcountyohio.gov";

            var client = new SmtpClient(host);

            MailAddress from = new MailAddress(testAddress, "Bobby Lovell");

            MailAddress to = new MailAddress(testAddress);

            MailMessage message = new MailMessage(from, to);
            message.Body = "This is a test message from SMTP...";
            message.Subject = "SMTP Test";

            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            string userState = "1234";
            client.SendAsync(message, userState);

            //  message.Dispose();




        }



    }
}
