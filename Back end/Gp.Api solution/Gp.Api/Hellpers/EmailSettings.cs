using GP.Core.Entities;
using System.Net;
using System.Net.Mail;

namespace Gp.Api.Hellpers
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email) 
        {
            var client = new SmtpClient("smtp.gmail.com", 587);

            client.EnableSsl = true;

            client.Credentials = new NetworkCredential("jilanhisham02@gmail.com", "pupstdqyhghzfkqu");
            // Da el email elly haye3mel Send

            client.Send("jilanhisham02@gmail.com", email.To, email.Title, email.Body);
            }
        }
    }
