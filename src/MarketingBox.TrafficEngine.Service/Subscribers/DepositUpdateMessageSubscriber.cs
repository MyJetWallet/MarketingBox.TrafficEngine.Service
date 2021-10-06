using System;
using System.Globalization;
using DotNetCoreDecorators;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.TrafficEngine.Service.Messages.Traffic;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests;

namespace MarketingBox.TrafficEngine.Service.Subscribers
{
    public class DepositUpdateMessageSubscriber
    {
        private readonly IPublisher<CalculatedTrafficMessage> _calculateTrafficPublisher;
        private readonly ILogger<DepositUpdateMessageSubscriber> _logger;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campDataReader;
        private readonly ICampaignService _campaignService;

        public DepositUpdateMessageSubscriber(
            ISubscriber<MarketingBox.Registration.Service.Messages.Deposits.DepositUpdateMessage> subscriber,
            IPublisher<CalculatedTrafficMessage> calculateTrafficPublisher,
            ILogger<DepositUpdateMessageSubscriber> logger,
            IMyNoSqlServerDataReader<CampaignNoSql> campDataReader,
            ICampaignService campaignService)
        {
            _calculateTrafficPublisher = calculateTrafficPublisher;
            _logger = logger;
            _campDataReader = campDataReader;
            _campaignService = campaignService;
            subscriber.Subscribe(Consume);
        }

        private async ValueTask Consume(MarketingBox.Registration.Service.Messages.Deposits.DepositUpdateMessage message)
        {
            _logger.LogInformation("Consuming message {@context}", message);
            var campaignNoSql = _campDataReader.Get(
                CampaignNoSql.GeneratePartitionKey(message.TenantId),
                CampaignNoSql.GenerateRowKey(message.CampaignId));

            decimal payoutAmount;
            decimal revenueAmount;

            if (campaignNoSql != null)
            {
                payoutAmount = campaignNoSql.Payout.Plan == Plan.CPA ? campaignNoSql.Payout.Amount : 0;
                revenueAmount = campaignNoSql.Revenue.Plan == Plan.CPA ? campaignNoSql.Revenue.Amount : 0;
            }
            else
            {
                var campaign = await _campaignService.GetAsync(new CampaignGetRequest() { CampaignId = message.CampaignId });

                if (campaign?.Campaign == null)
                {
                    _logger.LogError("There is no campaign! Skipping message: {@context}", message);
                    return;
                }

                if (campaign.Error != null)
                {
                    _logger.LogError("Error from affiliate service while processing message: {@context}", message);

                    throw new Exception("Retry!");
                }

                payoutAmount = campaign.Campaign.Payout.Plan == Plan.CPA ? campaign.Campaign.Payout.Amount : 0;
                revenueAmount = campaign.Campaign.Revenue.Plan == Plan.CPA ? campaign.Campaign.Revenue.Amount : 0;
            }

            var calculatedTraffic = new CalculatedTrafficMessage()
            {
                AffiliateId = message.AffiliateId,
                LeadId = message.LeadId,
                PayoutAmount = payoutAmount.ToString(CultureInfo.InvariantCulture),
                RevenueAmount = revenueAmount.ToString(CultureInfo.InvariantCulture),
                TenantId = message.TenantId,
                BoxId = message.BoxId,
                CampaignId = message.CampaignId,
                CreatedAt = message.CreatedAt,
                BrandId = message.BrandId
            };

            await _calculateTrafficPublisher.PublishAsync(calculatedTraffic);

            _logger.LogInformation("Has been consumed {@context}", message);
        }
    }
}
