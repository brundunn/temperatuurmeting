using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Core.Interfaces;

public interface ISensorDataParser
{
    bool CanParse(string rawData);
    SensorData Parse(string rawData);
}