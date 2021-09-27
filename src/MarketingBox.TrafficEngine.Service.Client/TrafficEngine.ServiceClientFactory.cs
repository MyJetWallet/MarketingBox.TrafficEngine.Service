using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.TrafficEngine.Service.Client
{
    [UsedImplicitly]
    public class TrafficEngineServiceClientFactory: MyGrpcClientFactory
    {
        public TrafficEngineServiceClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }
    }
}
