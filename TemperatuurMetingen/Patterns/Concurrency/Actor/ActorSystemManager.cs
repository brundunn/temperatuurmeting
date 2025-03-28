using System;
using System.Threading.Tasks;
using Akka.Actor;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Concurrency.Actor.Actors;
using TemperatuurMetingen.Patterns.Concurrency.Actor.Messages;

namespace TemperatuurMetingen.Patterns.Concurrency.Actor
{
    /// <summary>
    /// Manages the Actor system for sensor data processing and alerting.
    /// Provides a high-level interface for interacting with the underlying actor system.
    /// </summary>
    public class ActorSystemManager : IDisposable
    {
        /// <summary>
        /// The core Akka.NET actor system that manages all actors.
        /// </summary>
        private readonly ActorSystem _actorSystem;
        
        /// <summary>
        /// Actor reference for the sensor data processing actor.
        /// </summary>
        private readonly IActorRef _sensorDataActor;
        
        /// <summary>
        /// Actor reference for the alert generation actor.
        /// </summary>
        private readonly IActorRef _alertActor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSystemManager"/> class.
        /// Creates and initializes the actor system with sensor data and alert actors.
        /// </summary>
        public ActorSystemManager()
        {
            // Create the actor system
            _actorSystem = ActorSystem.Create("SensorMonitoringSystem");

            // Create the actors
            _sensorDataActor = _actorSystem.ActorOf(Props.Create(() => new SensorDataActor()), "sensorData");
            _alertActor = _actorSystem.ActorOf(Props.Create(() => new AlertActor()), "alerts");

            Console.WriteLine("Actor system initialized with SensorDataActor and AlertActor");
        }

        /// <summary>
        /// Processes sensor data by sending it to both the data processing and alert actors.
        /// </summary>
        /// <param name="data">The sensor data to process.</param>
        public void ProcessSensorData(SensorData data)
        {
            var message = new SensorDataMessage(data);

            // Send to both actors
            _sensorDataActor.Tell(message);
            _alertActor.Tell(message);
        }

        /// <summary>
        /// Asynchronously requests analysis of sensor data for a specific sensor type.
        /// </summary>
        /// <param name="sensorType">The type of sensor to analyze.</param>
        /// <returns>A task that resolves to a dictionary containing statistical results.</returns>
        public async Task<Dictionary<string, double>> AnalyzeSensorTypeAsync(string sensorType)
        {
            var result = await _sensorDataActor.Ask<DataAnalysisResult>(
                new AnalyzeDataMessage(sensorType),
                TimeSpan.FromSeconds(5)
            );

            return result.Stats;
        }

        /// <summary>
        /// Asynchronously retrieves the count of processed data points.
        /// </summary>
        /// <returns>A task that resolves to the number of processed data points.</returns>
        public async Task<int> GetProcessedDataCountAsync()
        {
            var result = await _sensorDataActor.Ask<SystemStatusResult>(
                new GetStatusMessage(),
                TimeSpan.FromSeconds(5)
            );

            return result.ProcessedDataPoints;
        }

        /// <summary>
        /// Asynchronously retrieves the current list of alerts from the alert actor.
        /// </summary>
        /// <returns>A task that resolves to a string containing the formatted alerts.</returns>
        public async Task<string> GetAlertsAsync()
        {
            var result = await _alertActor.Ask<string>(
                new GetStatusMessage(),
                TimeSpan.FromSeconds(5)
            );

            return result;
        }

        /// <summary>
        /// Disposes the actor system, terminating all actors and releasing resources.
        /// </summary>
        public void Dispose()
        {
            _actorSystem?.Terminate();
            _actorSystem?.Dispose();
        }
    }
}