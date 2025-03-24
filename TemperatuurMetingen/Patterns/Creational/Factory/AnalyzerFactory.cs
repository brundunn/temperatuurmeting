using System;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

    public abstract class AnalyzerFactory
    {
        public abstract ISensorDataAnalyzer CreateAnalyzer();
    }
        
    public class TemperatureAnalyzerFactory : AnalyzerFactory
    {
        private readonly double _warningThreshold;
        private readonly double _criticalThreshold;
            
        public TemperatureAnalyzerFactory(double warningThreshold = 25.0, double criticalThreshold = 30.0)
        {
            _warningThreshold = warningThreshold;
            _criticalThreshold = criticalThreshold;
        }
            
        public override ISensorDataAnalyzer CreateAnalyzer()
        {
            return new TemperatureAnalyzer(_warningThreshold, _criticalThreshold);
        }
    }
        
    public class HumidityAnalyzerFactory : AnalyzerFactory
    {
        private readonly double _lowHumidityThreshold;
        private readonly double _highHumidityThreshold;
            
        public HumidityAnalyzerFactory(double lowHumidityThreshold = 30.0, double highHumidityThreshold = 70.0)
        {
            _lowHumidityThreshold = lowHumidityThreshold;
            _highHumidityThreshold = highHumidityThreshold;
        }
            
        public override ISensorDataAnalyzer CreateAnalyzer()
        {
            return new HumidityAnalyzer(_lowHumidityThreshold, _highHumidityThreshold);
        }
    }
        
    public class BatteryAnalyzerFactory : AnalyzerFactory
    {
        private readonly double _lowBatteryThreshold;
            
        public BatteryAnalyzerFactory(double lowBatteryThreshold = 0.2)
        {
            _lowBatteryThreshold = lowBatteryThreshold;
        }
            
        public override ISensorDataAnalyzer CreateAnalyzer()
        {
            return new BatteryAnalyzer(_lowBatteryThreshold);
        }
}