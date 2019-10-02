using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class EMailService
    {
        readonly MailAddress _fromAddress = new MailAddress("QTU100@gmail.com", "Administration");
        readonly SmtpClient _smtp;

        public EMailService()
        {
            const string fromPassword = "Oa@QGkA#p6Qwm2WA";

            _smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromAddress.Address, fromPassword)
            };
        }

        public async Task<bool> TrySendMessageAsync(User target, string subject, string title, string body)
        {
            var toAddress = new MailAddress(target.Email, target.UserName);
            var message = new MailMessage(_fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            using (message)
            {
                try
                {
                    await _smtp.SendMailAsync(message);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
