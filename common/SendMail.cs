using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace Project.common
{
    public static class Mail
    {
        public static int SendMail(string sub,string body , string to)
        {
            try
            {
                // how to enable less secure app.
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;                 // setup Smtp authentication    
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(" ", "ff4@1234");
                client.UseDefaultCredentials = true;

                client.Credentials = credentials;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("chatpatadabba@gmail.com");
                msg.IsBodyHtml = true;
                msg.To.Add(new MailAddress(to));

                msg.Subject = sub;
                msg.IsBodyHtml = true;
                msg.Body = body;
                client.Send(msg);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
    }
}