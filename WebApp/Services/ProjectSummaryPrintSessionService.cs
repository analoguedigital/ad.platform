using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Models;

namespace WebApi.Services
{
    public class ProjectSummaryPrintSessionService
    {
        private static Dictionary<Guid, ProjectSummaryPrintSessionDTO> Storage = new Dictionary<Guid, ProjectSummaryPrintSessionDTO>();

        public static ProjectSummaryPrintSessionDTO AddOrUpdateSession(ProjectSummaryPrintSessionDTO session)
        {
            if (session.Id == Guid.Empty)
                session.Id = Guid.NewGuid();

            Storage[session.Id] = session;
            return session;
        }

        public static ProjectSummaryPrintSessionDTO UpdateSession(Guid id, ProjectSummaryPrintSessionDTO session)
        {
            Storage[id] = session;
            return session;
        }

        public static ProjectSummaryPrintSessionDTO GetSession(Guid id)
        {
            if (Storage.ContainsKey(id))
                return Storage[id];

            return null;
        }
    }
}