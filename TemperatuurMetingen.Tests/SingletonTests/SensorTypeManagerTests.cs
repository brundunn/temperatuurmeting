using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TemperatuurMetingen.Patterns.Creational.Singleton;

namespace TemperatuurMetingen.Tests.SingletonTests
{
    /// <summary>
    /// Unit tests for the SensorTypeManager singleton class.
    /// </summary>
    [TestClass]
    public class SensorTypeManagerTests
    {
        /// <summary>
        /// Tests that the singleton instance returns the same instance.
        /// </summary>
        [TestMethod]
        public void SingletonInstance_ShouldReturnSameInstance()
        {
            // Arrange & Act
            var instance1 = SensorTypeManager.Instance;
            var instance2 = SensorTypeManager.Instance;

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance1, instance2, "Singleton should return the same instance");
        }

        /// <summary>
        /// Tests that registering and retrieving a sensor type works correctly.
        /// </summary>
        [TestMethod]
        public void RegisterAndGetSensorType_ShouldWorkCorrectly()
        {
            // Arrange
            var singleton = SensorTypeManager.Instance;
            string serialNumber = "TEST123";
            string sensorType = "temperature";

            // Act
            singleton.RegisterSensorType(serialNumber, sensorType);
            string retrievedType = singleton.GetSensorType(serialNumber);

            // Assert
            Assert.AreEqual(sensorType, retrievedType, "Retrieved sensor type should match registered type");
        }

        /// <summary>
        /// Tests that retrieving a sensor type for an unknown sensor returns "unknown".
        /// </summary>
        [TestMethod]
        public void GetSensorType_ForUnknownSensor_ShouldReturnUnknown()
        {
            // Arrange
            var singleton = SensorTypeManager.Instance;
            string nonExistentSerial = "NONEXISTENT";

            // Act
            string result = singleton.GetSensorType(nonExistentSerial);

            // Assert
            Assert.AreEqual("unknown", result, "Unknown sensor should return 'unknown' type");
        }

        /// <summary>
        /// Tests that registering the same sensor type multiple times does not duplicate entries.
        /// </summary>
        [TestMethod]
        public void RegisterSensorType_MultipleTimes_ShouldNotDuplicate()
        {
            // Arrange
            var singleton = SensorTypeManager.Instance;
            string uniqueSerial = "TEST-" + Guid.NewGuid().ToString(); // Completely unique
            string uniqueType = "type-" + Guid.NewGuid().ToString();   // Completely unique

            // Act & Assert
            int initialCount = singleton.GetSensorCount();

            // First registration
            singleton.RegisterSensorType(uniqueSerial, uniqueType);
            Assert.AreEqual(initialCount + 1, singleton.GetSensorCount());
            Assert.AreEqual(uniqueType, singleton.GetSensorType(uniqueSerial));

            // Save the count after first registration
            int countAfterFirst = singleton.GetSensorCount();

            // Second registration with same values
            singleton.RegisterSensorType(uniqueSerial, uniqueType);
            Assert.AreEqual(countAfterFirst, singleton.GetSensorCount(),
                "Count should not change when registering same sensor-type combination");
        }

        /// <summary>
        /// Tests that the singleton maintains consistency when accessed from multiple threads.
        /// </summary>
        [TestMethod]
        public void MultithreadAccess_ShouldMaintainConsistency()
        {
            // This test verifies that the singleton remains consistent when accessed from multiple threads

            // Arrange
            const int threadCount = 100;
            var tasks = new List<Task>();
            var serialNumbers = new List<string>();

            for (int i = 0; i < threadCount; i++)
            {
                string serial = $"MT-SENSOR-{i}";
                serialNumbers.Add(serial);
            }

            // Act - Access singleton from multiple threads simultaneously
            for (int i = 0; i < threadCount; i++)
            {
                string serial = serialNumbers[i];
                string type = $"type-{i}";

                tasks.Add(Task.Run(() => {
                    var instance = SensorTypeManager.Instance;
                    instance.RegisterSensorType(serial, type);
                }));
            }

            // Wait for all tasks to complete
            Task.WhenAll(tasks).Wait();

            // Assert
            var singleton = SensorTypeManager.Instance;
            int registeredCount = 0;

            foreach (var serial in serialNumbers)
            {
                string type = singleton.GetSensorType(serial);
                if (type != "unknown")
                    registeredCount++;
            }

            Assert.AreEqual(threadCount, registeredCount, "All sensor types should be registered successfully across threads");
        }
    }
}