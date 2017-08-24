using Hangfire;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace WebApi.Services
{
    public class EmailService
    {
        private UnitOfWork UnitOfWork { get; set; }

        public EmailService()
        {
            this.UnitOfWork = new UnitOfWork(new SurveyContext(autoDetectChangesEnabled: false));
        }

        public void ProcessEmails()
        {
            var emails = this.UnitOfWork.EmailsRepository.AllAsNoTracking
                .Where(e => !e.IsSent)
                .OrderByDescending(e => e.DateCreated)
                .ToList();

            foreach (var email in emails)
                BackgroundJob.Enqueue(() => this.SendEmail(email));
        }

        public async Task SendEmail(Email email)
        {
            var message = new MailMessage();
            message.To.Add(email.To);
            message.Subject = email.Subject;
            message.Body = email.Content;
            message.IsBodyHtml = true;

            try
            {
                var client = new SmtpClient();
                await client.SendMailAsync(message);

                email.IsSent = true;
                this.UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                this.UnitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}