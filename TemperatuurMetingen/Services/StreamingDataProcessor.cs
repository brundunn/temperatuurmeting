using System;
using System.IO;
using System.Threading.Tasks;
using TemperatuurMetingen.Patterns.Concurrency.ProducerConsumer;
using TemperatuurMetingen.Patterns.Structural.Facade;

namespace TemperatuurMetingen.Services
{
    /// <summary>
    /// Processes streaming sensor data using a producer-consumer pattern.
    /// Coordinates between the data queue and sensor system facade.
    /// </summary>
    public class StreamingDataProcessor
    {
        /// <summary>
        /// The facade that handles sensor data processing and system coordination.
        /// </summary>
        private readonly SensorSystemFacade _sensorSystem;
        
        /// <summary>
        /// Queue that implements the producer-consumer pattern for sensor data processing.
        /// </summary>
        private readonly SensorDataQueue _dataQueue;

        /// <summary>
        /// Initializes a new instance of the StreamingDataProcessor class.
        /// Sets up event handlers for the data queue.
        /// </summary>
        /// <param name="sensorSystem">The sensor system facade used to process sensor data.</param>
        public StreamingDataProcessor(SensorSystemFacade sensorSystem)
        {
            _sensorSystem = sensorSystem;
            _dataQueue = new SensorDataQueue();

            // Set up event handlers
            _dataQueue.RawDataReceived += (sender, data) =>
            {
                Console.WriteLine($"Data received: {data.Substring(0, Math.Min(20, data.Length))}...");
            };
        }

        /// <summary>
        /// Starts the consumer process that reads from the data queue and processes items.
        /// </summary>
        public void Start()
        {
            // Start the consumer that processes data
            _dataQueue.Start(data => _sensorSystem.ProcessSensorData(data));
            Console.WriteLine("Streaming data processor started.");
        }

        /// <summary>
        /// Stops the consumer process and halts data processing.
        /// </summary>
        public void Stop()
        {
            _dataQueue.Stop();
            Console.WriteLine("Streaming data processor stopped.");
        }

        /// <summary>
        /// Adds a raw data string to the processing queue.
        /// </summary>
        /// <param name="rawData">The raw sensor data to be processed.</param>
        public void AddData(string rawData)
        {
            _dataQueue.Produce(rawData);
        }

        /// <summary>
        /// Asynchronously reads a file line by line and adds each line to the processing queue.
        /// Simulates real-time data by adding a configurable delay between lines.
        /// </summary>
        /// <param name="filePath">The path to the file containing sensor data.</param>
        /// <param name="delayMs">The delay in milliseconds between processing each line. Defaults to 100ms.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task ProcessFileStreamAsync(string filePath, int delayMs = 100)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        // Add the data to the queue
                        _dataQueue.Produce(line);

                        // Simulate real-time data by adding a delay
                        await Task.Delay(delayMs);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
    }
}