using IDS.Transporter.Connectors;
using IDS.Transporter.Configuration.ModbusTcp;

namespace IDS.Transporter.Configurator.ModbusTcp;

public static class Source
{
    public static IConnector Create(Dictionary<object, object> section, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor)
    {
        ConnectorConfiguration config = new();
        config.ConnectorType = section.ContainsKey("connector") ? Convert.ToString(section["connector"]) : "EthernetIP";
        config.Direction = Configuration.ConnectorDirectionEnum.Source;
        config.Enabled = section.ContainsKey("enabled") ? Convert.ToBoolean(section["enabled"]) : true;
        config.ScanIntervalMs = section.ContainsKey("scan_interval") ? Convert.ToInt32(section["scan_interval"]) : 1000;
        config.ReportByException = section.ContainsKey("rbe") ? Convert.ToBoolean(section["rbe"]) : true;
        config.Name = section.ContainsKey("name") ? Convert.ToString(section["name"]) : Guid.NewGuid().ToString();
        config.IpAddress = section.ContainsKey("address") ? Convert.ToString(section["address"]) : "0.0.0.0";
        config.Port = section.ContainsKey("port") ? Convert.ToInt32(section["port"]) : 502;
        config.Slave = section.ContainsKey("slave") ? Convert.ToByte(section["slave"]) : (byte)1;
        config.TimeoutMs = section.ContainsKey("timeout") ? Convert.ToInt32(section["timeout"]) : 1000;
        config.Items = new List<ConnectorItem>();

        var items = section["items"] as List<object>;
        if (items != null)
        {
            foreach (var item in items)
            {
                var itemDictionary = item as Dictionary<object, object>;
                if (itemDictionary != null)
                {
                    config.Items.Add(new ConnectorItem()
                    {
                        Enabled = itemDictionary.ContainsKey("enabled") ? Convert.ToBoolean(itemDictionary["enabled"]) : true,
                        Name = itemDictionary.ContainsKey("name") ? Convert.ToString(itemDictionary["name"]) : Guid.NewGuid().ToString(),
                        Type = itemDictionary.ContainsKey("type") ? Convert.ToInt32(itemDictionary["type"]) : 1,
                        Address = itemDictionary.ContainsKey("address") ? Convert.ToUInt16(itemDictionary["address"]) : (ushort)1,
                        Count = itemDictionary.ContainsKey("count") ? Convert.ToUInt16(itemDictionary["count"]) : (ushort)1,
                    });
                }
            }
        }

        var connector = new Connectors.ModbusTcp.Source(config, disruptor);

        return connector;
    }
}