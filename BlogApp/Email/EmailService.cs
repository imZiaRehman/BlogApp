using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;


namespace BlogApp.Email
{
    public class EmailService
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587; 
        private readonly string smtpUsername = "mziarehman98@gmail.com";
        private readonly string smtpAppPassword = "yeqtkgyyjxutolno";

        public void SendConfirmationEmail(string toEmail, string confirmationLink)
        {
            using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(smtpUsername, smtpAppPassword);
                smtp.EnableSsl = true;

                MailMessage message = new MailMessage();
                message.From = new MailAddress("m.ziaurrehman@devsinc.com");
                message.To.Add(toEmail);
                message.Subject = "Confirmation Email";
                message.Body = $"Click the link to confirm your email: {confirmationLink}";

                smtp.Send(message);
            }
        }

    }

}