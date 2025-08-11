using System.Collections.Generic;

namespace SS_API.Reporting
{
    public class SegmentSummary
    {
        public int ClusterId { get; set; }
        public int MemberCount { get; set; } 
        public Dictionary<string, object> FeatureSummary { get; set; } 

      
        public string Description { get; set; }
    }
}
