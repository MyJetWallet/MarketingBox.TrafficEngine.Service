using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.TrafficEngine.Service.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxTrafficEngineService.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxTrafficEngineService.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MarketingBoxTrafficEngineService.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("MarketingBoxTrafficEngineService.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("MarketingBoxTrafficEngineService.MarketingBoxServiceBusHostPort")]
        public string MarketingBoxServiceBusHostPort { get; set; }

        [YamlProperty("MarketingBoxTrafficEngineService.AffiliateServiceUrl")]
        public string AffiliateServiceUrl { get; set; }
    }
}
