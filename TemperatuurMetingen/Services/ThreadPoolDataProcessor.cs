using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Concurrency.ThreadPool;
using TemperatuurMetingen.Patterns.Structural.Facade;

namespace TemperatuurMetingen.Services
{
    public class ThreadPoolDataProcessor
    {
        private readonly SensorSystemFacade _sensorSystem;
        private readonly ThreadPoolManager _threadPool;

        public ThreadPoolDataProcessor(SensorSystemFacade sensorSystem)
        {
            _sensorSystem = sensorSystem;
            _threadPool = ThreadPoolManager.Instance;
        }

        public async Task ProcessDataLinesAsync(IEnumerable<string> dataLines)
        {
            Console.WriteLine("Starting parallel processing of sensor data...");

            await _threadPool.ProcessBatchAsync(dataLines, async (line) =>
            {
                // Simulate some processing time
                await Task.Delay(50);

                // Process the data
                _sensorSystem.ProcessSensorData(line);
            });

            Console.WriteLine("Parallel processing complete.");
        }

        public async Task<Dictionary<string, SensorData>> ProcessAndGroupDataAsync(IEnumerable<string> dataLines)
        {
            var result = new Dictionary<string, SensorData>();
            var tasks = new List<Task<KeyValuePair<string, SensorData>>>();

            foreach (var line in dataLines)
            {
                tasks.Add(_threadPool.QueueTaskAsync(() =>
                {
                    // Process data and return serial number with data
                    var data = _sensorSystem.ProcessSensorDataAndReturn(line);
                    return new KeyValuePair<string, SensorData>(data.SerialNumber, data);
                }));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var pair in results)
            {
                if (!string.IsNullOrEmpty(pair.Key))
                {
                    result[pair.Key] = pair.Value;
                }
            }

            return result;
        }
    }
}