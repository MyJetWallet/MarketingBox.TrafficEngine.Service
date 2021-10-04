using Autofac;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.TrafficEngine.Service.Subscribers;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using MyServiceBus.Abstractions;

namespace MarketingBox.TrafficEngine.Service.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAffiliateServiceClient(Program.Settings.AffiliateServiceUrl);

            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    ApplicationEnvironment.HostName, Program.LogFactory);

            var subs = new MyNoSqlReadRepository<CampaignNoSql>(noSqlClient, CampaignNoSql.TableName);
            builder.RegisterInstance(subs)
                .As<IMyNoSqlServerDataReader<CampaignNoSql>>();

            #region MarketingBox.Registration.Service.Messages.Leads.LeadBusUpdatedMessage

            // subscriber (ISubscriber<MarketingBox.Registration.Service.Messages.Leads.LeadBusUpdatedMessage>)
            builder.RegisterMyServiceBusSubscriberSingle<MarketingBox.Registration.Service.Messages.Deposits.DepositUpdateMessage>(
                serviceBusClient, 
                MarketingBox.Registration.Service.Messages.Topics.LeadUpdateTopic, 
                "marketingbox-trafficengine-service", 
                TopicQueueType.Permanent);

            #endregion

            // publisher (IPublisher<MarketingBox.TrafficEngine.Service.Messages.Traffic.CalculatedTrafficMessage>)
            builder.RegisterMyServiceBusPublisher<MarketingBox.TrafficEngine.Service.Messages.Traffic.CalculatedTrafficMessage>(serviceBusClient,
                MarketingBox.TrafficEngine.Service.Messages.Topics.CalculatedTrafficTopic, 
                false);

            builder.RegisterType<MarketingBox.Registration.Service.Messages.Deposits.DepositUpdateMessage>()
                .SingleInstance()
                .AutoActivate();
        }
    }
}
