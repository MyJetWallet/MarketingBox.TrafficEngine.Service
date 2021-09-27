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
        [DataMember(Order = 1)] public string TenantId { get; set; }

        [DataMember(Order = 2)] public long LeadId { get; set; }

        [DataMember(Order = 3)] public decimal PayoutAmount { get; set; }

        [DataMember(Order = 4)] public decimal RevenueAmount { get; set; }
    }
}
