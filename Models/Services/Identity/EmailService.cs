using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services.Identity
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var email = new Email
            {
                To = message.Destination,
                Subject = message.Subject,
                Content = message.Body
            };

            var uow = new UnitOfWork(new SurveyContext());

            try
            {
                uow.EmailsRepository.InsertOrUpdate(email);
                uow.Save();

                return Task.FromResult(200);
            }
            catch (Exception)
            {
                return Task.FromResult(503);
            }
        }
    }
}
