using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Enums;
using System;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class EmailRecipientsRepository : Repository<EmailRecipient>
    {
        public EmailRecipientsRepository(UnitOfWork uow) : base(uow) { }

        private EmailRecipient FlagEmailNotification(EmailRecipient recipient, EmailNotificationType accessLevel, bool enabled)
        {
            switch (accessLevel)
            {
                case EmailNotificationType.Feedbacks:
                    {
                        recipient.Feedbacks = enabled;
                        break;
                    }
                case EmailNotificationType.NewMobileUsers:
                    {
                        recipient.NewMobileUsers = enabled;
                        break;
                    }
                case EmailNotificationType.OrgRequests:
                    {
                        recipient.OrgRequests = enabled;
                        break;
                    }
                case EmailNotificationType.OrgConnectionRequests:
                    {
                        recipient.OrgConnectionRequests = enabled;
                        break;
                    }
                default:
                    break;
            }

            return recipient;
        }

        public EmailRecipient AssignEmailNotification(Guid userId, EmailNotificationType flags, bool enabled)
        {
            var recipient = this.AllAsNoTracking.SingleOrDefault(x => x.OrgUserId == userId);

            if (recipient != null)
            {
                recipient = FlagEmailNotification(recipient, flags, enabled);
                this.InsertOrUpdate(recipient);
            }
            else
            {
                recipient = new EmailRecipient() { OrgUserId = userId };
                recipient = FlagEmailNotification(recipient, flags, enabled);
                this.InsertOrUpdate(recipient);
            }

            this.CurrentUOW.Save();

            return this.AllAsNoTracking.SingleOrDefault(x => x.OrgUserId == userId);
        }

        public override void InsertOrUpdate(EmailRecipient entity)
        {
            if (entity.Feedbacks || entity.NewMobileUsers || entity.OrgRequests || entity.OrgConnectionRequests)
                base.InsertOrUpdate(entity);
            else
            {
                var entry = this.Context.Entry(entity);
                if (entry.State == System.Data.Entity.EntityState.Modified || entry.State == System.Data.Entity.EntityState.Detached)
                    this.Delete(entity);
            }
        }
    }
}
