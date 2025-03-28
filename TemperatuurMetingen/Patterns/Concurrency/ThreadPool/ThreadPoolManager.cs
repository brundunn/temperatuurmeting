using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TemperatuurMetingen.Patterns.Concurrency.ThreadPool
{
    /// <summary>
    /// Manages thread pool resources for concurrent operations with controlled parallelism.
    /// Implements the Singleton pattern to provide a single system-wide thread pool manager.
    /// </summary>
    public class ThreadPoolManager
    {
        /// <summary>
        /// Lazy-initialized singleton instance of the ThreadPoolManager class.
        /// </summary>
        private static readonly Lazy<ThreadPoolManager> _instance =
            new Lazy<ThreadPoolManager>(() => new ThreadPoolManager());

        /// <summary>
        /// Gets the singleton instance of the ThreadPoolManager.
        /// </summary>
        public static ThreadPoolManager Instance => _instance.Value;

        /// <summary>
        /// The maximum number of operations that can execute concurrently.
        /// </summary>
        private readonly int _maxDegreeOfParallelism;
        
        /// <summary>
        /// Semaphore to control access to the thread pool resources.
        /// </summary>
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolManager"/> class.
        /// Sets the maximum parallelism based on the number of processor cores.
        /// </summary>
        private ThreadPoolManager()
        {
            // Use number of processor cores as default parallelism
            _maxDegreeOfParallelism = Environment.ProcessorCount;
            _semaphore = new SemaphoreSlim(_maxDegreeOfParallelism, _maxDegreeOfParallelism);

            Console.WriteLine($"Thread Pool initialized with {_maxDegreeOfParallelism} worker threads.");
        }

        /// <summary>
        /// Queues a function for execution on the thread pool and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by the function.</typeparam>
        /// <param name="function">The function to execute on the thread pool.</param>
        /// <returns>A Task representing the asynchronous operation, containing the result of the function.</returns>
        public async Task<TResult> QueueTaskAsync<TResult>(Func<TResult> function)
        {
            try
            {
                // Wait until a thread is available
                await _semaphore.WaitAsync();

                // Execute the task using the thread pool
                return await Task.Run(function);
            }
            finally
            {
                // Release the thread back to the pool
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Queues an action for execution on the thread pool.
        /// </summary>
        /// <param name="action">The action to execute on the thread pool.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task QueueTaskAsync(Action action)
        {
            try
            {
                // Wait until a thread is available
                await _semaphore.WaitAsync();

                // Execute the task using the thread pool
                await Task.Run(action);
            }
            finally
            {
                // Release the thread back to the pool
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Processes a collection of items in parallel using the thread pool.
        /// </summary>
        /// <typeparam name="T">The type of items to process.</typeparam>
        /// <param name="items">The collection of items to process.</param>
        /// <param name="processFunction">The function to apply to each item.</param>
        /// <returns>A Task representing the asynchronous batch operation.</returns>
        public async Task ProcessBatchAsync<T>(IEnumerable<T> items, Func<T, Task> processFunction)
        {
            var tasks = new List<Task>();

            foreach (var item in items)
            {
                tasks.Add(QueueTaskAsync(async () => await processFunction(item)));
            }

            await Task.WhenAll(tasks);
        }
    }
}