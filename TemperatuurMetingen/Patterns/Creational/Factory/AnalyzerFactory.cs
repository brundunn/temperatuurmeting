using System;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

    /// <summary>
    /// Abstract factory that defines the interface for creating sensor data analyzers.
    /// Implements the Factory Method pattern for creating different types of analyzers.
    /// </summary>
    public abstract class AnalyzerFactory
    {
        /// <summary>
        /// Creates a new sensor data analyzer instance.
        /// </summary>
        /// <returns>An implementation of the ISensorDataAnalyzer interface.</returns>
        public abstract ISensorDataAnalyzer CreateAnalyzer();
    }

    /// <summary>
    /// Factory for creating temperature analyzers with customizable thresholds.
    /// </summary>
    public class TemperatureAnalyzerFactory : AnalyzerFactory
    {
        /// <summary>
        /// The temperature threshold for warning alerts.
        /// </summary>
        private readonly double _warningThreshold;
        
        /// <summary>
        /// The temperature threshold for critical alerts.
        /// </summary>
        private readonly double _criticalThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureAnalyzerFactory"/> class.
        /// </summary>
        /// <param name="warningThreshold">The warning temperature threshold. Defaults to 25.0°C.</param>
        /// <param name="criticalThreshold">The critical temperature threshold. Defaults to 30.0°C.</param>
        public TemperatureAnalyzerFactory(double warningThreshold = 25.0, double criticalThreshold = 30.0)
        {
            _warningThreshold = warningThreshold;
            _criticalThreshold = criticalThreshold;
        }

        /// <summary>
        /// Creates a new temperature analyzer with configured thresholds.
        /// </summary>
        /// <returns>A temperature analyzer configured with the specified thresholds.</returns>
        public override ISensorDataAnalyzer CreateAnalyzer()
        {
            return new TemperatureAnalyzer(_warningThreshold, _criticalThreshold);
        }
    }

    /// <summary>
    /// Factory for creating humidity analyzers with customizable thresholds.
    /// </summary>
    public class HumidityAnalyzerFactory : AnalyzerFactory
    {
        /// <summary>
        /// The threshold for low humidity alerts.
        /// </summary>
        private readonly double _lowHumidityThreshold;
        
        /// <summary>
        /// The threshold for high humidity alerts.
        /// </summary>
        private readonly double _highHumidityThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="HumidityAnalyzerFactory"/> class.
        /// </summary>
        /// <param name="lowHumidityThreshold">The low humidity threshold. Defaults to 30.0%.</param>
        /// <param name="highHumidityThreshold">The high humidity threshold. Defaults to 70.0%.</param>
        public HumidityAnalyzerFactory(double lowHumidityThreshold = 30.0, double highHumidityThreshold = 70.0)
        {
            _lowHumidityThreshold = lowHumidityThreshold;
            _highHumidityThreshold = highHumidityThreshold;
        }

        /// <summary>
        /// Creates a new humidity analyzer with configured thresholds.
        /// </summary>
        /// <returns>A humidity analyzer configured with the specified thresholds.</returns>
        public override ISensorDataAnalyzer CreateAnalyzer()
        {
            return new HumidityAnalyzer(_lowHumidityThreshold, _highHumidityThreshold);
        }
    }

    /// <summary>
    /// Factory for creating battery analyzers with customizable threshold.
    /// </summary>
    public class BatteryAnalyzerFactory : AnalyzerFactory
    {
        /// <summary>
        /// The threshold for low battery alerts, as a percentage (0.0-1.0).
        /// </summary>
        private readonly double _lowBatteryThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatteryAnalyzerFactory"/> class.
        /// </summary>
        /// <param name="lowBatteryThreshold">The low battery threshold as a decimal percentage. Defaults to 0.2 (20%).</param>
        public BatteryAnalyzerFactory(double lowBatteryThreshold = 0.2)
        {
            _lowBatteryThreshold = lowBatteryThreshold;
        }

        /// <summary>
        /// Creates a new battery analyzer with configured threshold.
        /// </summary>
        /// <returns>A battery analyzer configured with the specified threshold.</returns>
        public override ISensorDataAnalyzer CreateAnalyzer()
        {
            return new BatteryAnalyzer(_lowBatteryThreshold);
        }
}