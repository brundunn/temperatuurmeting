using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

/// <summary>
/// Analyzes battery data from sensors to detect low battery levels and provide statistics.
/// Implements the ISensorDataAnalyzer interface to work with the Factory pattern.
/// </summary>
public class BatteryAnalyzer : ISensorDataAnalyzer
{
    /// <summary>
    /// Collection of sensor data containing battery information.
    /// </summary>
    private readonly List<SensorData> _batteryData = new List<SensorData>();
    
    /// <summary>
    /// Threshold value (as a decimal percentage) for determining low battery status.
    /// </summary>
    private readonly double _lowBatteryThreshold;

    /// <summary>
    /// Gets the type identifier for this analyzer.
    /// </summary>
    public string AnalyzerType => "Battery";

    /// <summary>
    /// Initializes a new instance of the <see cref="BatteryAnalyzer"/> class.
    /// </summary>
    /// <param name="lowBatteryThreshold">The threshold value (as a decimal percentage) below which a battery is considered low. Defaults to 0.2 (20%).</param>
    public BatteryAnalyzer(double lowBatteryThreshold = 0.2)
    {
        _lowBatteryThreshold = lowBatteryThreshold;
    }

    /// <summary>
    /// Analyzes sensor data to extract battery information.
    /// Only processes data with valid battery level and maximum values.
    /// </summary>
    /// <param name="data">The sensor data containing battery information.</param>
    public void Analyze(SensorData data)
    {
        if (data.BatteryLevel > 0 && data.BatteryMax > 0)
        {
            _batteryData.Add(data);
        }
    }

    /// <summary>
    /// Generates a summary report of battery analysis including average levels and low battery alerts.
    /// </summary>
    /// <returns>A formatted string with battery analysis results.</returns>
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