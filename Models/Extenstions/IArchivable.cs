using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public static class IArchivableExtensions
    {
        public static bool IsArchived(this IArchivable obj)
        {
            return obj.DateArchived.HasValue;
        }

        public static void Archive(this IArchivable obj)
        {
            obj.DateArchived = DateTimeService.UtcNow;
        }

        public static IEnumerable<T> NotArchived<T>(this IEnumerable<T> objects) where T : IArchivable
        {
            return objects.Where(o => o.DateArchived == null);
        }

        public static IQueryable<T> NotArchived<T>(this IQueryable<T> objects) where T : IArchivable
        {
            return objects.Where(o => o.DateArchived == null);
        }
    }
}
