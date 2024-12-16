using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class FaceComparison
    {
        public string MatchStatus { get; set; }
        public float accuracy { get; set; }

        public string userId { get; set; }
        public string? ImageName { get; set; }
    }
}
