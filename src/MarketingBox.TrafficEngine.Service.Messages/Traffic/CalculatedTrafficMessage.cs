using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketingBox.TrafficEngine.Service.Messages.Traffic
{
    [DataContract]
    public class CalculatedTrafficMessage
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public long LeadId { get; set; }

        [DataMember(Order = 3)]
        public string PayoutAmount { get; set; }

        [DataMember(Order = 4)]
        public string RevenueAmount { get; set; }

        [DataMember(Order = 5)]
        public long BoxId { get; set; }

        [DataMember(Order = 6)]
        public long CampaignId { get; set; }
    }
}
