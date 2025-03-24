namespace TemperatuurMetingen.Core.Models;

public class SensorData
{
    public string SerialNumber { get; set; } = "";
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double BatteryLevel { get; set; }
    public double BatteryMax { get; set; }
    public double BatteryMin { get; set; }
    public string State { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string Type { get; set; } = "";
    public string Error { get; set; } = "";
    public double Voltage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $"Sensor: {SerialNumber} | Type: {Type} | Temp: {Temperature} | Humidity: {Humidity} | Battery: {BatteryLevel}/{BatteryMax} | State: {State}";
    }
}