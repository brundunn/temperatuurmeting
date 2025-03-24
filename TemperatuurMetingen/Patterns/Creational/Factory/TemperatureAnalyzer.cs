using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

public class TemperatureAnalyzer : ISensorDataAnalyzer
{
    private readonly List<double> _temperatures = new List<double>();
    private readonly double _warningThreshold;
    private readonly double _criticalThreshold;

    public string AnalyzerType => "Temperature";

    public TemperatureAnalyzer(double warningThreshold = 25.0, double criticalThreshold = 30.0)
    {
        _warningThreshold = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }

    public void Analyze(SensorData data)
    {
        if (data.Temperature > 0)
        {
            _temperatures.Add(data.Temperature);
        }
    }

    public string GetAnalysisResult()
    {
        if (_temperatures.Count == 0)
        {
            return "No temperature data available for analysis.";
        }

        double average = _temperatures.Average();
        double max = _temperatures.Max();
        double min = _temperatures.Min();

        string status = "Normal";
        if (max > _criticalThreshold)
        {
            status = "CRITICAL";
        }
        else if (max > _warningThreshold)
        {
            status = "Warning";
        }

        return $"Temperature Analysis:\n" +
               $"  Average: {average:F2}°C\n" +
               $"  Maximum: {max:F2}°C\n" +
               $"  Minimum: {min:F2}°C\n" +
               $"  Status: {status}\n" +
               $"  Data points: {_temperatures.Count}";
    }
}