using LightMethods.Survey.Models.DAL;
using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class EmailService
    {
        private UnitOfWork UnitOfWork { get; set; }

        public EmailService()
        {
            this.UnitOfWork = new UnitOfWork(new SurveyContext(autoDetectChangesEnabled: false));
        }

        public async Task ProcessEmails()
        {
            var emails = this.UnitOfWork.EmailsRepository.AllAsNoTracking
                .Where(e => !e.IsSent)
                .OrderByDescending(e => e.DateCreated)
                .ToList();

            foreach (var email in emails)
            {
                var message = new MailMessage();
                message.To.Add(email.To);
                message.Subject = email.Subject;
                message.Body = email.Content;
                message.IsBodyHtml = true;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.None;
                message.Priority = MailPriority.High;

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
}