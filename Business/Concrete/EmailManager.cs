using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Entities;
using Entities.DTOs;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Business.Concrete
{
    public class EmailManager : IEmailService
    {
        private readonly Entities.DTOs.EmailSender _emailsender;

        public EmailManager(IOptions<EmailSender> emailSender)
        {
            _emailsender = emailSender.Value; 
        }


        public void SendingEmail(string email)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Email Verification<No-Reply>", _emailsender.Email));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Email Verification by SelfBookAPI";


            Random rnd = new Random();
            string number = rnd.Next(0, 9999).ToString();
            number = String.Format("{0:0000}", number);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<b>Email code: </b>" + number;

            message.Body = bodyBuilder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate(_emailsender.Email, _emailsender.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }

    }
}
