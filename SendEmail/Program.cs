using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace SendHTMLEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            SendMail();
            Environment.Exit(0);
        }

        public static void SendMail()
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["email"]);
            message.To.Add(new MailAddress(ConfigurationManager.AppSettings["destinationEmail"]));

            message.Subject = ConfigurationManager.AppSettings["emailSubject"];

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailTemplate("HTMLToEmail.html"));
            htmlView.ContentType = new System.Net.Mime.ContentType("text/html");

            message.AlternateViews.Add(htmlView);
            string isGmail = ConfigurationManager.AppSettings["isGmailAccount"];

            SmtpClient client = new SmtpClient();

            if (isGmail == "true")
            {
                client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"])
                };
            }
            else
            {
                client.Host = ConfigurationManager.AppSettings["host"];
                client.Port = Int32.Parse(ConfigurationManager.AppSettings["port"]);
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"]);
            }

            client.Send(message);
        }

        private static string GetEmailTemplate(string name)
        {
#if DEBUG
            //            From Visual Studio:
            var path = Path.GetFullPath(@"..\..\" + name);

            //            From Another IDE:
            //var path = Path.GetFullPath(@".\" + name);
#else
            var path = Path.GetFullPath(@"" + name);
#endif
            StreamReader htmlFile = new StreamReader(path);


            string emailString = htmlFile.ReadToEnd();
            htmlFile.Close();

            return emailString;
        }
    }
}
