using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services
{
    public static class DateTimeService
    {
        public static DateTime Now
        {
            get { return DateTime.Now; }
        }

        public static DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
