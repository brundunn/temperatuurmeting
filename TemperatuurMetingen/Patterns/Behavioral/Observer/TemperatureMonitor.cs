using System;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Behavioral.Observer;

public class TemperatureMonitor : ISensorDataObserver
{
    private readonly double _warningThreshold;
    private readonly double _criticalThreshold;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemperatureMonitor"/> class.
    /// </summary>
    /// <param name="warningThreshold">The temperature threshold for warnings.</param>
    /// <param name="criticalThreshold">The temperature threshold for critical alerts.</param>
    public TemperatureMonitor(double warningThreshold = 25.0, double criticalThreshold = 30.0)
    {
        _warningThreshold = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }

    /// <summary>
    /// Updates the temperature monitor with new sensor data.
    /// </summary>
    /// <param name="data">The sensor data containing temperature information.</param>
    public void Update(SensorData data)
    {
        if (data.Type != "temp")
        {
            return; // Only process temperature sensors
        }

        Console.WriteLine($"Temperature reading: {data.Temperature}°C from sensor {data.SerialNumber}");

        if (data.Temperature > _criticalThreshold)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"CRITICAL: Temperature exceeds {_criticalThreshold}°C!");
            Console.ResetColor();
        }
        else if (data.Temperature > _warningThreshold)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: Temperature exceeds {_warningThreshold}°C!");
            Console.ResetColor();
        }
    }
}