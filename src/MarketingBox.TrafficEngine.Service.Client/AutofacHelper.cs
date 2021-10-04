using Autofac;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.TrafficEngine.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAssetsDictionaryClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new TrafficEngineServiceClientFactory(grpcServiceUrl);
        }
    }
}
