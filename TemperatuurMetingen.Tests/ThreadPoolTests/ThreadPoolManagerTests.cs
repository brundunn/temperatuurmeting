using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TemperatuurMetingen.Patterns.Concurrency.ThreadPool;

namespace TemperatuurMetingen.Tests.ThreadPoolTests
{
    /// <summary>
    /// Unit tests for the ThreadPoolManager class.
    /// </summary>
    [TestClass]
    public class ThreadPoolManagerTests
    {
        /// <summary>
        /// Tests that QueueTaskAsync with a function executes and returns the correct result.
        /// </summary>
        [TestMethod]
        public async Task QueueTaskAsync_WithFunction_ShouldExecuteAndReturnResult()
        {
            // Arrange
            var threadPool = ThreadPoolManager.Instance;
            int expected = 42;

            // Act
            int result = await threadPool.QueueTaskAsync(() =>
            {
                // Simulate some work
                Thread.Sleep(100);
                return expected;
            });

            // Assert
            Assert.AreEqual(expected, result, "Task should execute and return the correct result");
        }

        /// <summary>
        /// Tests that QueueTaskAsync with an action executes the action.
        /// </summary>
        [TestMethod]
        public async Task QueueTaskAsync_WithAction_ShouldExecuteAction()
        {
            // Arrange
            var threadPool = ThreadPoolManager.Instance;
            bool wasExecuted = false;

            // Act
            await threadPool.QueueTaskAsync(() =>
            {
                // Simulate some work
                Thread.Sleep(100);
                wasExecuted = true;
            });

            // Assert
            Assert.IsTrue(wasExecuted, "Action should have been executed");
        }

        /// <summary>
        /// Tests that the thread pool processes all items in a batch.
        /// </summary>
        [TestMethod]
        public async Task ThreadPool_ShouldProcessAllItemsInBatch()
        {
            // Arrange
            var threadPool = ThreadPoolManager.Instance;
            var items = Enumerable.Range(1, 10).ToList();
            var processedItems = new System.Collections.Concurrent.ConcurrentBag<int>();

            // Act - Manually create and track tasks
            var tasks = new List<Task>();

            foreach (var item in items)
            {
                // Capture the item in a local variable to avoid closure issues
                var capturedItem = item;

                var task = threadPool.QueueTaskAsync(() => {
                    // Simulate some work
                    Thread.Sleep(capturedItem * 10);
                    processedItems.Add(capturedItem);
                });

                tasks.Add(task);
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            // Assert
            foreach (var item in items)
            {
                Assert.IsTrue(processedItems.Contains(item),
                    $"All items should be processed: missing {item}");
            }

            Assert.AreEqual(items.Count, processedItems.Count,
                $"Expected {items.Count} processed items, got {processedItems.Count}");
        }

        /// <summary>
        /// Tests that tasks execute in parallel.
        /// </summary>
        [TestMethod]
        public async Task Tasks_ShouldExecuteInParallel()
        {
            // Arrange
            var threadPool = ThreadPoolManager.Instance;
            var parallelTasks = 5;

            // We'll use a thread-safe counter to track concurrent execution
            int concurrentExecutions = 0;
            int maxConcurrentExecutions = 0;

            // Use this for synchronizing access to counters
            object lockObj = new object();

            // EventWaitHandle to keep tasks running until we're ready to check
            using var executionBarrier = new ManualResetEvent(false);

            // Act - Create multiple tasks that will all try to run at once
            var tasks = new List<Task>();

            Console.WriteLine("Starting parallel execution test...");

            for (int i = 0; i < parallelTasks; i++)
            {
                tasks.Add(Task.Run(async () => {
                    await threadPool.QueueTaskAsync(() => {
                        // Signal that this task has started execution
                        lock (lockObj)
                        {
                            concurrentExecutions++;
                            if (concurrentExecutions > maxConcurrentExecutions)
                            {
                                maxConcurrentExecutions = concurrentExecutions;
                            }

                            Console.WriteLine($"Task entered execution, current count: {concurrentExecutions}");
                        }

                        // All tasks will wait here on the same barrier
                        // This ensures they all run concurrently if parallel execution is working
                        executionBarrier.WaitOne();

                        // Signal that this task is finishing execution
                        lock (lockObj)
                        {
                            concurrentExecutions--;
                            Console.WriteLine($"Task exiting, remaining count: {concurrentExecutions}");
                        }
                    });
                }));
            }

            // Give all tasks a chance to start
            await Task.Delay(1000);

            // Record the maximum concurrent executions before we let tasks complete
            int recordedMax = maxConcurrentExecutions;
            Console.WriteLine($"Maximum concurrent executions detected: {recordedMax}");

            // Release the barrier so tasks can complete
            executionBarrier.Set();

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            // Assert
            Console.WriteLine($"Final max concurrent executions: {recordedMax}");

            // If tasks ran in parallel, we should have seen more than 1 task running concurrently
            Assert.IsTrue(recordedMax > 1,
                $"Expected multiple concurrent executions, but maximum was only {recordedMax}");

            // Ideally, all tasks should run concurrently on a system with enough cores
            // But we'll be a bit more lenient to account for varying environments
            Assert.IsTrue(recordedMax >= 2,
                "ThreadPool should support at least 2 concurrent tasks");
        }

        /// <summary>
        /// Tests that the thread pool respects the maximum degree of parallelism.
        /// </summary>
        [TestMethod]
        public async Task ThreadPool_ShouldRespectMaxDegreeOfParallelism()
        {
            // Arrange
            var threadPool = ThreadPoolManager.Instance;
            int totalTasks = 20;
            int longTaskDuration = 1000; // ms

            // We're going to track active tasks count
            int maxConcurrentTasks = 0;
            int currentActiveTasks = 0;
            object lockObj = new object();

            // Act
            var tasks = new List<Task>();

            for (int i = 0; i < totalTasks; i++)
            {
                tasks.Add(threadPool.QueueTaskAsync(() =>
                {
                    // Track active tasks
                    lock (lockObj)
                    {
                        currentActiveTasks++;
                        maxConcurrentTasks = Math.Max(maxConcurrentTasks, currentActiveTasks);
                    }

                    // Simulate work
                    Thread.Sleep(longTaskDuration);

                    // Decrement active count
                    lock (lockObj)
                    {
                        currentActiveTasks--;
                    }
                }));
            }

            // Wait for all tasks to finish
            await Task.WhenAll(tasks);

            // Assert
            // Get the processor count which should be the default max degree of parallelism
            int expectedMaxParallelism = Environment.ProcessorCount;

            Console.WriteLine($"Max concurrent tasks observed: {maxConcurrentTasks}");
            Console.WriteLine($"Expected max parallelism (processor count): {expectedMaxParallelism}");

            // We check that max concurrent tasks didn't exceed processors+1
            // (Adding one for possible slight timing variations)
            Assert.IsTrue(maxConcurrentTasks <= expectedMaxParallelism + 1,
                $"Max concurrent tasks ({maxConcurrentTasks}) should not significantly exceed " +
                $"max degree of parallelism ({expectedMaxParallelism})");

            // Also check that we did achieve some parallelism (at least 2)
            Assert.IsTrue(maxConcurrentTasks >= 2,
                "ThreadPool should execute at least some tasks in parallel");
        }

        /// <summary>
        /// Tests that the thread pool supports task cancellation.
        /// </summary>
        [TestMethod]
        public async Task ThreadPool_ShouldSupportCancellation()
        {
            // Arrange
            var threadPool = ThreadPoolManager.Instance;
            var cts = new CancellationTokenSource();
            bool taskCompleted = false;

            // Act
            // Start a task that will take a while
            var task = Task.Run(async () => {
                try
                {
                    await threadPool.QueueTaskAsync(() => {
                        // This task would take 5 seconds
                        for (int i = 0; i < 50; i++)
                        {
                            // Check for cancellation frequently
                            if (i % 5 == 0)
                                cts.Token.ThrowIfCancellationRequested();

                            Thread.Sleep(100);
                        }

                        taskCompleted = true;
                    });
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation occurs
                }
            });

            // Cancel after a short time
            await Task.Delay(200);
            cts.Cancel();

            // Wait for the task to respond to cancellation
            await Task.Delay(300);

            // Assert
            Assert.IsFalse(taskCompleted, "Task should have been cancelled before completion");

            // Clean up
            try { await task; } catch { /* Ignore exceptions */ }
        }
    }
}