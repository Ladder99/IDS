using DIME.Configuration.MtConnectAgent;
using MQTTnet;
using MTConnect.Clients;

namespace DIME.Connectors.MtConnectAgent;

public class Source: QueuingSourceConnector<ConnectorConfiguration, ConnectorItem>
{
    private MTConnectHttpClient _client = null;
    
    public Source(ConnectorConfiguration configuration, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor) : base(configuration, disruptor)
    {
    }

    protected override bool InitializeImplementation()
    {
        return true;
    }

    protected override bool CreateImplementation()
    {
        _client = new MTConnectHttpClient(Configuration.Address, Configuration.Port);
        
        _client.ObservationReceived += (s, observation) =>
        {
            _incomingBuffer.Add(new IncomingMessage()
            {
                Key = observation.DataItemId,
                Value = observation.Values.ToList(),
                Timestamp = DateTime.UtcNow.ToEpochMilliseconds()
            });
        };
            
        return true;
    }

    protected override bool ConnectImplementation()
    {
        _client.Start();
        return true;
    }
    
    protected override bool DisconnectImplementation()
    {
        _client.Stop();
        return true;
    }
    
    protected override bool DeinitializeImplementation()
    {
        return true;
    }
}