using System.Text;
using DIME.Configuration.SplunkEhSdk;
using Google.Protobuf.Collections;
using Grpc.Net.Client;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using ProtoBuf.WellKnownTypes;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace DIME.Connectors.SplunkEhSdk;

public class Sink: SinkConnector<ConnectorConfiguration, ConnectorItem>
{
    public Sink(ConnectorConfiguration configuration, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor) : base(configuration, disruptor)
    {
    }

    protected override bool InitializeImplementation()
    {
        return true;
    }

    protected override bool CreateImplementation()
    {
        return true;
    }

    protected override bool ConnectImplementation()
    {
        return true;
    }

    protected override bool WriteImplementation()
    {
        using (var channel = GrpcChannel.ForAddress($"{Configuration.Address}:{Configuration.Port}"))
        {
            foreach (var message in Outbox)
            {
                var client = new EdgeHubService.EdgeHubServiceClient(channel);
                var @event = new SendEventDataRequest()
                {
                    Id = JsonConvert.SerializeObject(message),
                    CreateTime = Timestamp.FromDateTime(DateTime.UtcNow),
                    Fields =
                    {
                        { JsonConvert.SerializeObject(message), "" },
                        { "name", "edge-hub-sdk" },
                        { "type", "single-event" },
                    }
                };
                var reply = client.SendEventData(@event);
            }
        }

        return true;
    }

    protected override bool DisconnectImplementation()
    {
        return true;
    }
    
    protected override bool DeinitializeImplementation()
    {
        return true;
    }
}