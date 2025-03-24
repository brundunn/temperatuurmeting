using System;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Behavioral.Observer;

public class BatteryMonitor : ISensorDataObserver
{
    private readonly double _lowBatteryThreshold;
        
    public BatteryMonitor(double lowBatteryThreshold = 0.2)
    {
        _lowBatteryThreshold = lowBatteryThreshold;
    }
        
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