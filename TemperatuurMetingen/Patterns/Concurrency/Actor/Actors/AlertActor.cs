using System;
using System.Collections.Generic;
using Akka.Actor;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Concurrency.Actor.Messages;

namespace TemperatuurMetingen.Patterns.Concurrency.Actor.Actors
{
    public class AlertActor : ReceiveActor
    {
        private readonly Dictionary<string, AlertThresholds> _thresholds = new Dictionary<string, AlertThresholds>();
        private readonly List<string> _alerts = new List<string>();

        private class AlertThresholds
        {
            public double TemperatureHigh { get; set; } = 30.0;
            public double TemperatureLow { get; set; } = 10.0;
            public double HumidityHigh { get; set; } = 80.0;
            public double HumidityLow { get; set; } = 20.0;
            public double BatteryLow { get; set; } = 30.0;
        }

        public AlertActor()
        {
            // Default thresholds
            _thresholds["temp"] = new AlertThresholds();
            _thresholds["humidity"] = new AlertThresholds();

            // Handle sensor data for alerts
            Receive<SensorDataMessage>(message => { CheckForAlerts(message.Data); });

            // Handle getting all alerts
            Receive<GetStatusMessage>(message => { Sender.Tell(string.Join("\n", _alerts)); });
        }

        private void CheckForAlerts(SensorData data)
        {
            if (string.IsNullOrEmpty(data.SerialNumber) || string.IsNullOrEmpty(data.Type))
                return;

            if (!_thresholds.TryGetValue(data.Type, out var threshold))
                threshold = new AlertThresholds(); // Use default threshold

            string alert = null;

            // Check temperature
            if (data.Temperature > threshold.TemperatureHigh)
            {
                alert =
                    $"HIGH TEMP ALERT: Sensor {data.SerialNumber} reported {data.Temperature}°C (threshold: {threshold.TemperatureHigh}°C)";
            }
            else if (data.Temperature < threshold.TemperatureLow && data.Temperature > 0)
            {
                alert =
                    $"LOW TEMP ALERT: Sensor {data.SerialNumber} reported {data.Temperature}°C (threshold: {threshold.TemperatureLow}°C)";
            }

            // Check humidity
            if (data.Humidity > threshold.HumidityHigh)
            {
                alert =
                    $"HIGH HUMIDITY ALERT: Sensor {data.SerialNumber} reported {data.Humidity}% (threshold: {threshold.HumidityHigh}%)";
            }
            else if (data.Humidity < threshold.HumidityLow && data.Humidity > 0)
            {
                alert =
                    $"LOW HUMIDITY ALERT: Sensor {data.SerialNumber} reported {data.Humidity}% (threshold: {threshold.HumidityLow}%)";
            }

            // Check battery
            if (data.BatteryLevel > 0 && data.BatteryMax > 0)
            {
                double batteryPercentage = (data.BatteryLevel / data.BatteryMax) * 100;
                if (batteryPercentage < threshold.BatteryLow)
                {
                    alert =
                        $"LOW BATTERY ALERT: Sensor {data.SerialNumber} battery at {batteryPercentage:F1}% (threshold: {threshold.BatteryLow}%)";
                }
            }

            if (alert != null)
            {
                Console.WriteLine($"ALERT: {alert}");
                _alerts.Add($"[{DateTime.Now:HH:mm:ss}] {alert}");
            }
        }
    }
}

