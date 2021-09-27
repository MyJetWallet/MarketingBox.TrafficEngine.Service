using DotNetCoreDecorators;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.TrafficEngine.Service.Messages.Traffic;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System.Threading.Tasks;

namespace MarketingBox.TrafficEngine.Service.Subscribers
{
    public class LeadBusUpdatedMessageSubscriber
    {
        private readonly ILogger<LeadBusUpdatedMessageSubscriber> _logger;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campDataReader;
        private readonly IPublisher<CalculatedTrafficMessage> _calcPublisher;

        public LeadBusUpdatedMessageSubscriber(
            ISubscriber<MarketingBox.Registration.Service.Messages.Leads.LeadBusUpdateMessage> subscriber,
            ILogger<LeadBusUpdatedMessageSubscriber> logger,
            IMyNoSqlServerDataReader<CampaignNoSql> campDataReader,
            IPublisher<CalculatedTrafficMessage> calcPublisher)
        {
            _logger = logger;
            _campDataReader = campDataReader;
            _calcPublisher = calcPublisher;
            subscriber.Subscribe(Consume);
        }

        private async ValueTask Consume(MarketingBox.Registration.Service.Messages.Leads.LeadBusUpdateMessage message)
        {
            _logger.LogInformation("Consuming message {@context}", message);
            var campaign = _campDataReader.Get(
                CampaignNoSql.GeneratePartitionKey(message.TenantId),
                CampaignNoSql.GenerateRowKey(message.RouteInfo.CampaignId));

            var payoutAmount = campaign.Payout.Amount;
            var revenueAmount = campaign.Revenue.Amount;

            await _calcPublisher.PublishAsync(new CalculatedTrafficMessage()
            {
                RevenueAmount = revenueAmount,
                PayoutAmount = payoutAmount,
                LeadId = message.LeadId,
                TenantId = message.TenantId
            });

            _logger.LogInformation("Has been consumed {@context}", message);
        }
    }
}
