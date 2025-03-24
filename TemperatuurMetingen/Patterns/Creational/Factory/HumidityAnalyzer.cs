using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

public class HumidityAnalyzer : ISensorDataAnalyzer
{
    private readonly List<double> _humidityValues = new List<double>();
    private readonly double _highHumidityThreshold;
    private readonly double _lowHumidityThreshold;

    public string AnalyzerType => "Humidity";

    public HumidityAnalyzer(double lowHumidityThreshold = 30.0, double highHumidityThreshold = 70.0)
    {
        _lowHumidityThreshold = lowHumidityThreshold;
        _highHumidityThreshold = highHumidityThreshold;
    }

    public void Analyze(SensorData data)
    {
        if (data.Humidity > 0)
        {
            _humidityValues.Add(data.Humidity);
        }
    }

    public string GetAnalysisResult()
    {
        if (_humidityValues.Count == 0)
        {
            return "No humidity data available for analysis.";
        }

        double average = _humidityValues.Average();
        double max = _humidityValues.Max();
        double min = _humidityValues.Min();

        string status = "Normal";
        if (min < _lowHumidityThreshold)
        {
            status = "Too Dry";
        }
        else if (max > _highHumidityThreshold)
        {
            status = "Too Humid";
        }

        return $"Humidity Analysis:\n" +
               $"  Average: {average:F2}%\n" +
               $"  Maximum: {max:F2}%\n" +
               $"  Minimum: {min:F2}%\n" +
               $"  Status: {status}\n" +
               $"  Data points: {_humidityValues.Count}";
    }
}