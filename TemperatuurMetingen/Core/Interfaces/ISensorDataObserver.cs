using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Core.Interfaces;

public interface ISensorDataObserver
{
    void Update(SensorData sensorData);
}