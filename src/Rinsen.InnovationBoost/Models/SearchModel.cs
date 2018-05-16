using System;
using System.Collections.Generic;

namespace Rinsen.InnovationBoost.Models
{
    public class SearchModel
    {
        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }

        public IEnumerable<int> LogLevels { get; set; }

        public IEnumerable<int> LogEnvironments { get; set; }

        public IEnumerable<int> LogApplications { get; set; }

    }
}
