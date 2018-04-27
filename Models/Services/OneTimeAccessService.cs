using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.Services
{
    public static class OneTimeAccessService
    {
        /// <summary>
        /// One time tokens live for a max of 60 seconds
        /// </summary>
        private static double _timeToLive = 60.0;
        private static object lockObject = new object();

        private static List<OneTimeAccessTicket> _tickets = new List<OneTimeAccessTicket>();

        public static Guid GetFileIdForTicket(string accessId)
        {
            var attachmentId = Guid.Empty;

            lock (lockObject)
            {
                // Max 60 seconds to start download after requesting one time token.
                _tickets.RemoveAll(t => t.CreatedAt < DateTime.UtcNow.AddSeconds(-_timeToLive));

                var item = _tickets.FirstOrDefault(t => t.AccessId == accessId);
                if (item != null)
                {
                    attachmentId = item.AttachmentId;
                    //_tickets.Remove(item);    // not necessary to remove the ticket. it will time out automatically.
                }
            }

            return attachmentId;
        }

        public static string AddFileIdForTicket(Guid attachmentId)
        {
            var useOnceAccessId = new OneTimeAccessTicket(attachmentId);

            lock (lockObject)
            {
                _tickets.Add(useOnceAccessId);
            }

            return useOnceAccessId.AccessId;
        }
    }

    internal class OneTimeAccessTicket
    {
        public DateTime CreatedAt { get; set; }

        public Guid AttachmentId { get; set; }

        public string AccessId { get; set; }

        public OneTimeAccessTicket(Guid attachmentId)
        {
            this.CreatedAt = DateTime.UtcNow;
            this.AccessId = CreateAccessId();
            this.AttachmentId = attachmentId;
        }

        private string CreateAccessId()
        {
            var random = new Random();
            return random.Next() + Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return this.AccessId;
        }
    }
}
