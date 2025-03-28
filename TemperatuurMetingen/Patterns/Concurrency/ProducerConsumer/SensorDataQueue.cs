using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Concurrency.ProducerConsumer
{
    /// <summary>
    /// Implements the Producer-Consumer pattern for processing sensor data asynchronously.
    /// Provides a thread-safe queue system for handling sensor data with controlled throughput.
    /// </summary>
    public class SensorDataQueue
    {
        /// <summary>
        /// Thread-safe collection for storing raw sensor data entries.
        /// </summary>
        private readonly BlockingCollection<string> _dataQueue;

        /// <summary>
        /// Token source for cancellation of consumer operations.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Background task that processes queued data.
        /// </summary>
        private Task _consumerTask;

        /// <summary>
        /// Event raised when raw sensor data is added to the queue.
        /// </summary>
        public event EventHandler<string> RawDataReceived;

        /// <summary>
        /// Event raised when sensor data has been successfully processed.
        /// </summary>
        public event EventHandler<SensorData> DataProcessed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorDataQueue"/> class.
        /// </summary>
        /// <param name="boundedCapacity">The maximum capacity of the queue. Defaults to 100 items.</param>
        public SensorDataQueue(int boundedCapacity = 100)
        {
            _dataQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), boundedCapacity);
        }

        /// <summary>
        /// Starts the consumer task that processes queued sensor data.
        /// </summary>
        /// <param name="processAction">The action to perform on each data item.</param>
        /// <exception cref="InvalidOperationException">Thrown if consumer task is already running.</exception>
        public void Start(Action<string> processAction)
        {
            if (_consumerTask != null && !_consumerTask.IsCompleted)
            {
                throw new InvalidOperationException("Consumer is already running.");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            // Start the consumer task
            _consumerTask = Task.Run(() => ConsumeData(processAction, token), token);

            Console.WriteLine("Producer-Consumer queue started.");
        }

        /// <summary>
        /// Stops the consumer task and completes adding to the queue.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _dataQueue.CompleteAdding();

            try
            {
                _consumerTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
            {
                // Task was canceled, which is expected
            }

            Console.WriteLine("Producer-Consumer queue stopped.");
        }

        /// <summary>
        /// Adds raw sensor data to the processing queue.
        /// </summary>
        /// <param name="rawData">The raw sensor data string to be processed.</param>
        /// <exception cref="InvalidOperationException">Thrown if the queue has been marked as complete.</exception>
        public void Produce(string rawData)
        {
            if (_dataQueue.IsAddingCompleted)
            {
                throw new InvalidOperationException("Cannot add to a completed queue.");
            }

            _dataQueue.Add(rawData);
            RawDataReceived?.Invoke(this, rawData);
        }

        /// <summary>
        /// Consumer method that continuously processes data from the queue.
        /// </summary>
        /// <param name="processAction">The action to perform on each data item.</param>
        /// <param name="token">Cancellation token to stop processing when requested.</param>
        private void ConsumeData(Action<string> processAction, CancellationToken token)
        {
            try
            {
                // Process items until cancellation is requested or queue is marked as complete
                foreach (var rawData in _dataQueue.GetConsumingEnumerable(token))
                {
                    // Process the data
                    try
                    {
                        processAction(rawData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing data: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Cancellation was requested, which is expected
            }
        }
    }
}