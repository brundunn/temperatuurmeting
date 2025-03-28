using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

/// <summary>
/// Analyzes temperature data from sensors to determine if conditions are normal, warning, or critical.
/// Implements the ISensorDataAnalyzer interface to work with the Factory pattern.
/// </summary>
public class TemperatureAnalyzer : ISensorDataAnalyzer
{
    /// <summary>
    /// Collection of temperature values from sensor readings.
    /// </summary>
    private readonly List<double> _temperatures = new List<double>();

    /// <summary>
    /// Threshold value (in °C) above which temperature is considered a warning.
    /// </summary>
    private readonly double _warningThreshold;

    /// <summary>
    /// Threshold value (in °C) above which temperature is considered critical.
    /// </summary>
    private readonly double _criticalThreshold;

    /// <summary>
    /// Gets the type identifier for this analyzer.
    /// </summary>
    public string AnalyzerType => "Temperature";

    /// <summary>
    /// Initializes a new instance of the <see cref="TemperatureAnalyzer"/> class.
    /// </summary>
    /// <param name="warningThreshold">The threshold value above which temperature is considered a warning. Defaults to 25.0°C.</param>
    /// <param name="criticalThreshold">The threshold value above which temperature is considered critical. Defaults to 30.0°C.</param>
    public TemperatureAnalyzer(double warningThreshold = 25.0, double criticalThreshold = 30.0)
    {
        _warningThreshold = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }

    /// <summary>
    /// Analyzes sensor data to extract temperature information.
    /// Only processes data with valid temperature values greater than zero.
    /// </summary>
    /// <param name="data">The sensor data containing temperature information.</param>
    public void Analyze(SensorData data)
    {
        if (data.Temperature > 0)
        {
            _temperatures.Add(data.Temperature);
        }
    }

    /// <summary>
    /// Generates a summary report of temperature analysis including average, minimum, and maximum values.
    /// Also determines the temperature status based on configured thresholds.
    /// </summary>
    /// <returns>A formatted string with temperature analysis results.</returns>
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