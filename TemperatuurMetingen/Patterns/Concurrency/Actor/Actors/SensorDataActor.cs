﻿using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Concurrency.Actor.Messages;

namespace TemperatuurMetingen.Patterns.Concurrency.Actor.Actors
{
    public class SensorDataActor : ReceiveActor
    {
        private readonly Dictionary<string, List<SensorData>> _sensorDataStore = new Dictionary<string, List<SensorData>>();
        private int _totalProcessedData = 0;
        
        public SensorDataActor()
        {
            // Handle incoming sensor data
            Receive<SensorDataMessage>(message => {
                ProcessSensorData(message.Data);
            });
            
            // Handle analysis requests
            Receive<AnalyzeDataMessage>(message => {
                var result = AnalyzeSensorData(message.SensorType);
                Sender.Tell(new DataAnalysisResult(message.SensorType, result));
            });
            
            // Handle status requests
            Receive<GetStatusMessage>(message => {
                Sender.Tell(new SystemStatusResult(_totalProcessedData, _sensorDataStore.Count));
            });
        }
        
        private void ProcessSensorData(SensorData data)
        {
            if (string.IsNullOrEmpty(data.SerialNumber))
                return;
                
            if (!_sensorDataStore.TryGetValue(data.SerialNumber, out var dataList))
            {
                dataList = new List<SensorData>();
                _sensorDataStore[data.SerialNumber] = dataList;
            }
            
            dataList.Add(data);
            _totalProcessedData++;
            
            Console.WriteLine($"Actor: Processed data from sensor {data.SerialNumber} (Total: {_totalProcessedData})");
        }
        
        private Dictionary<string, double> AnalyzeSensorData(string sensorType)
        {
            var result = new Dictionary<string, double>
            {
                { "Count", 0 },
                { "Temperature", 0 },
                { "Humidity", 0 },
                { "BatteryLevel", 0 }
            };
            
            // Filter sensors by type
            var sensorsOfType = _sensorDataStore
                .Where(kv => kv.Value.Any(d => d.Type == sensorType))
                .Select(kv => kv.Value)
                .ToList();
                
            if (sensorsOfType.Count == 0)
                return result;
                
            result["Count"] = sensorsOfType.Count;
            
            // Calculate averages
            double tempSum = 0;
            double humSum = 0;
            double batSum = 0;
            int tempCount = 0;
            int humCount = 0;
            int batCount = 0;
            
            foreach (var sensorData in sensorsOfType)
            {
                foreach (var data in sensorData)
                {
                    if (data.Temperature > 0)
                    {
                        tempSum += data.Temperature;
                        tempCount++;
                    }
                    
                    if (data.Humidity > 0)
                    {
                        humSum += data.Humidity;
                        humCount++;
                    }
                    
                    if (data.BatteryLevel > 0 && data.BatteryMax > 0)
                    {
                        batSum += (data.BatteryLevel / data.BatteryMax) * 100;
                        batCount++;
                    }
                }
            }
            
            if (tempCount > 0)
                result["Temperature"] = tempSum / tempCount;
                
            if (humCount > 0)
                result["Humidity"] = humSum / humCount;
                
            if (batCount > 0)
                result["BatteryLevel"] = batSum / batCount;
                
            return result;
        }
    }
}