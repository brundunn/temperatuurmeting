using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

/// <summary>
/// Analyzes humidity data from sensors to determine if conditions are too dry, too humid, or normal.
/// Implements the ISensorDataAnalyzer interface to work with the Factory pattern.
/// </summary>
public class HumidityAnalyzer : ISensorDataAnalyzer
{
    /// <summary>
    /// Collection of humidity values from sensor readings.
    /// </summary>
    private readonly List<double> _humidityValues = new List<double>();
    
    /// <summary>
    /// Threshold value (percentage) above which humidity is considered too high.
    /// </summary>
    private readonly double _highHumidityThreshold;
    
    /// <summary>
    /// Threshold value (percentage) below which humidity is considered too low.
    /// </summary>
    private readonly double _lowHumidityThreshold;

    /// <summary>
    /// Gets the type identifier for this analyzer.
    /// </summary>
    public string AnalyzerType => "Humidity";

    /// <summary>
    /// Initializes a new instance of the <see cref="HumidityAnalyzer"/> class.
    /// </summary>
    /// <param name="lowHumidityThreshold">The threshold value below which humidity is considered too low. Defaults to 30.0%.</param>
    /// <param name="highHumidityThreshold">The threshold value above which humidity is considered too high. Defaults to 70.0%.</param>
    public HumidityAnalyzer(double lowHumidityThreshold = 30.0, double highHumidityThreshold = 70.0)
    {
        _lowHumidityThreshold = lowHumidityThreshold;
        _highHumidityThreshold = highHumidityThreshold;
    }

    /// <summary>
    /// Analyzes sensor data to extract humidity information.
    /// Only processes data with valid humidity values greater than zero.
    /// </summary>
    /// <param name="data">The sensor data containing humidity information.</param>
    public void Analyze(SensorData data)
    {
        if (data.Humidity > 0)
        {
            _humidityValues.Add(data.Humidity);
        }
    }

    /// <summary>
    /// Generates a summary report of humidity analysis including average, minimum, and maximum values.
    /// Also determines the overall humidity status based on configured thresholds.
    /// </summary>
    /// <returns>A formatted string with humidity analysis results.</returns>
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