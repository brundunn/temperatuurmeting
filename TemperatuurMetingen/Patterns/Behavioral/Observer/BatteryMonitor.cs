using System;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Behavioral.Observer;

public class BatteryMonitor : ISensorDataObserver
{
    private readonly double _lowBatteryThreshold;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatteryMonitor"/> class.
    /// </summary>
    /// <param name="lowBatteryThreshold">The threshold below which the battery is considered low.</param>
    public BatteryMonitor(double lowBatteryThreshold = 0.2)
    {
        _lowBatteryThreshold = lowBatteryThreshold;
    }

    /// <summary>
    /// Updates the battery monitor with new sensor data.
    /// </summary>
    /// <param name="data">The sensor data containing battery information.</param>
    public void Update(SensorData data)
    {
        if (data.BatteryLevel <= 0 || data.BatteryMax <= 0)
        {
            return; // Skip if battery data is not available
        }

        double batteryPercentage = data.BatteryLevel / data.BatteryMax;

        Console.WriteLine($"Battery level: {batteryPercentage:P} for sensor {data.SerialNumber}");

        if (batteryPercentage < _lowBatteryThreshold)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"WARNING: Low battery for sensor {data.SerialNumber}! ({batteryPercentage:P})");
            Console.ResetColor();
        }
    }
}