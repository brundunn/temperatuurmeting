using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Concurrency.ThreadPool;
using TemperatuurMetingen.Patterns.Structural.Facade;

namespace TemperatuurMetingen.Services
{
    /// <summary>
    /// Processes sensor data in parallel using a managed thread pool.
    /// Coordinates between the thread pool and sensor system facade for efficient data processing.
    /// </summary>
    public class ThreadPoolDataProcessor
    {
        /// <summary>
        /// The facade that handles sensor data processing and system coordination.
        /// </summary>
        private readonly SensorSystemFacade _sensorSystem;

        /// <summary>
        /// The thread pool manager that handles concurrent execution of tasks.
        /// </summary>
        private readonly ThreadPoolManager _threadPool;

        /// <summary>
        /// Initializes a new instance of the ThreadPoolDataProcessor class.
        /// </summary>
        /// <param name="sensorSystem">The sensor system facade used to process sensor data.</param>
        public ThreadPoolDataProcessor(SensorSystemFacade sensorSystem)
        {
            _sensorSystem = sensorSystem;
            _threadPool = ThreadPoolManager.Instance;
        }

        /// <summary>
        /// Processes a collection of sensor data lines in parallel using the thread pool.
        /// Each line is processed individually through the sensor system facade.
        /// </summary>
        /// <param name="dataLines">A collection of raw sensor data strings to process.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Processes sensor data and groups the results by sensor serial number.
        /// Returns a dictionary mapping sensor serial numbers to their processed data objects.
        /// </summary>
        /// <param name="dataLines">A collection of raw sensor data strings to process.</param>
        /// <returns>A Task that resolves to a dictionary mapping sensor serial numbers to their processed data.</returns>
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