using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LightMethods.Survey.Models.Services.Identity
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var sid = ConfigurationManager.AppSettings["TwilioAccountSID"];
            var token = ConfigurationManager.AppSettings["TwilioAuthToken"];
            var phoneNumber = ConfigurationManager.AppSettings["TwilioPhoneNumber"];

            TwilioClient.Init(sid, token);

            var to = new PhoneNumber(message.Destination);
            var twilioMessage = MessageResource.Create(
                to,
                from: new PhoneNumber(phoneNumber),
                body: message.Body);

            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
