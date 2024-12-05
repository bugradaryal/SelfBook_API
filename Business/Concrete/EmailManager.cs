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
        private readonly UserManager<User> _userManager;
        public EmailManager(IOptions<EmailSender> emailSender, UserManager<User> userManager)
        {
            _userManager = userManager;
            _emailsender = emailSender.Value; 
        }


        public async Task SendingEmail(string email, string url)    //callback url required
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Email Verification<No-Reply>", _emailsender.Email));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Email Verification by SelfBookAPI";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<b>Email verification url: </b>" + "<a href = " + url + "> link text </a> <br>" +
                "<br> <p>If link doesn't work : " + url + "</p>";

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
        public async Task ConfirmEmail(string userid, string token)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
                throw new Exception("User not found!!");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new Exception("Email confirmation failed!");
        }







    }
}
