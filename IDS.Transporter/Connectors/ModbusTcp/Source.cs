using IDS.Transporter.Configuration.ModbusTcp;
using System.Net.Sockets;
using NModbus;

namespace IDS.Transporter.Connectors.ModbusTcp;

public class Source: SourceConnector<ConnectorConfiguration, ConnectorItem>
{
    private IModbusMaster _client = null;

    public Source(ConnectorConfiguration configuration, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor) : base(configuration, disruptor)
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
        var tcpClient = new TcpClient();
        var task = tcpClient.ConnectAsync(
            Configuration.IpAddress,
            Configuration.Port
        );
        task.Wait(Configuration.TimeoutMs);
        if (!tcpClient.Connected)
        {
            return false;
        }
        _client = new ModbusFactory().CreateMaster(tcpClient);
        _client.Transport.ReadTimeout = Configuration.TimeoutMs;
        _client.Transport.WriteTimeout = Configuration.TimeoutMs;
        
        return true;
    }

    protected override bool ReadImplementation()
    {
        foreach (var item in Configuration.Items)
        {
            object response = null;
            
            switch (item.Type)
            {
                case 1:
                    response = _client
                        .ReadCoils(Configuration.Slave, 
                            item.Address, 
                            item.Count);
                    break;
                case 2:
                    response = _client
                        .ReadInputs(Configuration.Slave, 
                            item.Address, 
                            item.Count);
                    break;
                case 3:
                    response = _client
                        .ReadHoldingRegisters(Configuration.Slave, 
                            item.Address, 
                            item.Count);
                    break;
                case 4:
                    response = _client
                        .ReadInputRegisters(Configuration.Slave, 
                            item.Address, 
                            item.Count);
                    break;
                default:
                    response = null;
                    break;
            }
            
            
            Samples.Add(new MessageBoxMessage()
            {
                Path = $"{Configuration.Name}/{item.Name}",
                Data = response,
                Timestamp = DateTime.UtcNow.ToEpochMilliseconds()
            });
        }
        
        return true;
    }

    protected override bool DisconnectImplementation()
    {
        return true;
    }
}