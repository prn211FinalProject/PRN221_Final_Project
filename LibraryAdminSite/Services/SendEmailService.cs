using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAdminSite.Services
{
    public class SendEmailService
    {
        public void SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = "trantuanminhbg2003@gmail.com";
            var fromPassword = "oaxd geaa qpph skvj"; 

            // Thiết lập thông tin email
            var mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true; 

            var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, fromPassword)
            };

            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Email đã được gửi thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gửi email thất bại: " + ex.Message);
            }
        }
    }
}
