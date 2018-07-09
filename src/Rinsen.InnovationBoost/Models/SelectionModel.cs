using System.Collections.Generic;

namespace Rinsen.InnovationBoost.Models
{
    public class SelectionModel
    {
        public IEnumerable<int> LogLevels { get; set; }

        public IEnumerable<int> LogEnvironments { get; set; }

        public IEnumerable<int> LogApplications { get; set; }

        public IEnumerable<int> LogSources { get; set; }
    }
}
