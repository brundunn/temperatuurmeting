using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

public class BatteryAnalyzer : ISensorDataAnalyzer
{
    private readonly List<SensorData> _batteryData = new List<SensorData>();
    private readonly double _lowBatteryThreshold;
        
    public string AnalyzerType => "Battery";
        
    public BatteryAnalyzer(double lowBatteryThreshold = 0.2)
    {
        _lowBatteryThreshold = lowBatteryThreshold;
    }
        
    public void Analyze(SensorData data)
    {
        if (data.BatteryLevel > 0 && data.BatteryMax > 0)
        {
            _batteryData.Add(data);
        }
    }
        
    public string GetAnalysisResult()
    {
        if (_batteryData.Count == 0)
        {
            return "No battery data available for analysis.";
        }
            
        int totalSensors = _batteryData.Select(d => d.SerialNumber).Distinct().Count();
        var lowBatterySensors = _batteryData
            .Where(d => d.BatteryLevel / d.BatteryMax < _lowBatteryThreshold)
            .Select(d => d.SerialNumber)
            .Distinct()
            .ToList();
            
        double averageBatteryPercentage = _batteryData.Average(d => d.BatteryLevel / d.BatteryMax);
            
        return $"Battery Analysis:\n" +
               $"  Average battery level: {averageBatteryPercentage:P}\n" +
               $"  Total sensors: {totalSensors}\n" +
               $"  Sensors with low battery: {lowBatterySensors.Count}\n" +
               $"  Low battery sensors: {(lowBatterySensors.Count > 0 ? string.Join(", ", lowBatterySensors) : "None")}";
    }
}