using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Structural.Composite;
using TemperatuurMetingen.Patterns.Behavioral.Visitor;

namespace TemperatuurMetingen.Tests.VisitorTests
{
    /// <summary>
    /// Unit tests for the Visitor pattern implementation.
    /// </summary>
    [TestClass]
    public class VisitorPatternTests
    {
        /// <summary>
        /// Mock implementation of ISensorVisitor for testing purposes.
        /// </summary>
        private class MockVisitor : ISensorVisitor
        {
            /// <summary>
            /// Gets the count of visited leaf sensors.
            /// </summary>
            public int LeafVisitCount { get; private set; } = 0;

            /// <summary>
            /// Gets the count of visited sensor groups.
            /// </summary>
            public int GroupVisitCount { get; private set; } = 0;

            /// <summary>
            /// Gets the list of visited sensor names.
            /// </summary>
            public List<string> VisitedNames { get; private set; } = new List<string>();

            /// <summary>
            /// Visits a sensor leaf.
            /// </summary>
            /// <param name="sensor">The sensor leaf to visit.</param>
            public void Visit(SensorLeaf sensor)
            {
                LeafVisitCount++;
                VisitedNames.Add(sensor.Name);
            }

            /// <summary>
            /// Visits a sensor group.
            /// </summary>
            /// <param name="group">The sensor group to visit.</param>
            public void Visit(SensorGroup group)
            {
                GroupVisitCount++;
                VisitedNames.Add(group.Name);
            }

            /// <summary>
            /// Resets the visitor's state.
            /// </summary>
            public void Reset()
            {
                LeafVisitCount = 0;
                GroupVisitCount = 0;
                VisitedNames.Clear();
            }

            /// <summary>
            /// Gets the result of the visit.
            /// </summary>
            /// <returns>A string describing the visit result.</returns>
            public string GetResult()
            {
                return $"Visited {LeafVisitCount} leaves and {GroupVisitCount} groups: {string.Join(", ", VisitedNames)}";
            }
        }

        /// <summary>
        /// Tests that a sensor leaf accepts a visitor.
        /// </summary>
        [TestMethod]
        public void SensorLeaf_ShouldAcceptVisitor()
        {
            // Arrange
            var sensor = new SensorLeaf("123", "TestSensor");
            var visitor = new MockVisitor();

            // Act
            sensor.Accept(visitor);

            // Assert
            Assert.AreEqual(1, visitor.LeafVisitCount, "Visitor should visit the leaf once");
            Assert.AreEqual(0, visitor.GroupVisitCount, "Visitor should not visit any groups");
            Assert.IsTrue(visitor.VisitedNames.Contains("TestSensor"), "Visitor should record the sensor name");
        }

        /// <summary>
        /// Tests that a sensor group accepts a visitor.
        /// </summary>
        [TestMethod]
        public void SensorGroup_ShouldAcceptVisitor()
        {
            // Arrange
            var group = new SensorGroup("TestGroup");
            var visitor = new MockVisitor();

            // Act
            group.Accept(visitor);

            // Assert
            Assert.AreEqual(0, visitor.LeafVisitCount, "Visitor should not visit any leaves");
            Assert.AreEqual(1, visitor.GroupVisitCount, "Visitor should visit the group once");
            Assert.IsTrue(visitor.VisitedNames.Contains("TestGroup"), "Visitor should record the group name");
        }

        /// <summary>
        /// Tests that a sensor group propagates the visitor to its children.
        /// </summary>
        [TestMethod]
        public void SensorGroup_ShouldPropagateCVisitorToChildren()
        {
            // Arrange
            var rootGroup = new SensorGroup("Root");
            var childGroup = new SensorGroup("Child");
            var sensor1 = new SensorLeaf("1", "Sensor1");
            var sensor2 = new SensorLeaf("2", "Sensor2");

            // Build the hierarchy
            rootGroup.AddComponent(childGroup);
            childGroup.AddComponent(sensor1);
            rootGroup.AddComponent(sensor2);

            var visitor = new MockVisitor();

            // Act
            rootGroup.Accept(visitor);

            // Assert
            Assert.AreEqual(2, visitor.LeafVisitCount, "Visitor should visit both leaf sensors");
            Assert.AreEqual(2, visitor.GroupVisitCount, "Visitor should visit both groups");
            Assert.AreEqual(4, visitor.VisitedNames.Count, "Visitor should visit 4 elements total");

            // Order might vary but all names should be present
            CollectionAssert.Contains(visitor.VisitedNames, "Root");
            CollectionAssert.Contains(visitor.VisitedNames, "Child");
            CollectionAssert.Contains(visitor.VisitedNames, "Sensor1");
            CollectionAssert.Contains(visitor.VisitedNames, "Sensor2");
        }

        /// <summary>
        /// Tests that the SensorHealthVisitor identifies low battery sensors.
        /// </summary>
        [TestMethod]
        public void SensorHealthVisitor_ShouldIdentifyLowBatterySensors()
        {
            // Arrange
            var healthySensor = new SensorLeaf("1", "HealthySensor");
            var lowBatterySensor = new SensorLeaf("2", "LowBatterySensor");

            // Add sensor data with different battery levels
            healthySensor.AddData(new SensorData
            {
                SerialNumber = "1",
                BatteryLevel = 80,
                BatteryMax = 100
            });

            lowBatterySensor.AddData(new SensorData
            {
                SerialNumber = "2",
                BatteryLevel = 15,
                BatteryMax = 100
            });

            var group = new SensorGroup("TestGroup");
            group.AddComponent(healthySensor);
            group.AddComponent(lowBatterySensor);

            var healthVisitor = new SensorHealthVisitor();

            // Act
            group.Accept(healthVisitor);
            string result = healthVisitor.GetResult();

            // Assert
            Console.WriteLine("Health Visitor Result:");
            Console.WriteLine(result);

            // Check that the visitor identified the batteries correctly
            Assert.IsTrue(result.Contains("Healthy Sensors: 1"), "Should report one healthy sensor");
            Assert.IsTrue(result.Contains("Critical") && result.Contains("LowBatterySensor"),
                "Should report LowBatterySensor as critical");
        }

        /// <summary>
        /// Tests that the AnomalyDetectionVisitor identifies temperature anomalies.
        /// </summary>
        [TestMethod]
        public void AnomalyDetectionVisitor_ShouldIdentifyTemperatureAnomalies()
        {
            // Arrange
            var normalSensor = new SensorLeaf("1", "NormalSensor");
            var hotSensor = new SensorLeaf("2", "HotSensor");
            var coldSensor = new SensorLeaf("3", "ColdSensor");

            // Add sensor data with different temperature values
            normalSensor.AddData(new SensorData
            {
                SerialNumber = "1",
                Type = "temp",
                Temperature = 22.0 // Normal temperature
            });

            hotSensor.AddData(new SensorData
            {
                SerialNumber = "2",
                Type = "temp",
                Temperature = 35.0 // Hot temperature
            });

            coldSensor.AddData(new SensorData
            {
                SerialNumber = "3",
                Type = "temp",
                Temperature = 10.0 // Cold temperature
            });

            var group = new SensorGroup("TestGroup");
            group.AddComponent(normalSensor);
            group.AddComponent(hotSensor);
            group.AddComponent(coldSensor);

            // Create visitor with specific thresholds
            var anomalyVisitor = new AnomalyDetectionVisitor(
                tempThresholdLow: 15.0,
                tempThresholdHigh: 30.0
            );

            // Act
            group.Accept(anomalyVisitor);
            string result = anomalyVisitor.GetResult();

            // Assert
            Console.WriteLine("Anomaly Visitor Result:");
            Console.WriteLine(result);

            Assert.IsTrue(result.Contains("2 anomalies"), "Should detect 2 anomalies");
            Assert.IsTrue(result.Contains("High temperature") && result.Contains("HotSensor"),
                "Should detect high temperature on HotSensor");
            Assert.IsTrue(result.Contains("Low temperature") && result.Contains("ColdSensor"),
                "Should detect low temperature on ColdSensor");
        }

        /// <summary>
        /// Tests that the visitor can reset its state.
        /// </summary>
        [TestMethod]
        public void Visitor_ShouldReset()
        {
            // Arrange
            var sensor = new SensorLeaf("1", "TestSensor");
            var visitor = new MockVisitor();

            // Act & Assert - First visit
            sensor.Accept(visitor);
            Assert.AreEqual(1, visitor.LeafVisitCount, "Should count 1 leaf visit");

            // Reset and confirm counters are cleared
            visitor.Reset();
            Assert.AreEqual(0, visitor.LeafVisitCount, "Count should be reset to 0");
            Assert.AreEqual(0, visitor.VisitedNames.Count, "Visited names should be cleared");

            // Visit again and confirm counting works after reset
            sensor.Accept(visitor);
            Assert.AreEqual(1, visitor.LeafVisitCount, "Should count 1 leaf visit after reset");
        }

        /// <summary>
        /// Tests that a complex sensor hierarchy is fully visited.
        /// </summary>
        [TestMethod]
        public void ComplexHierarchy_ShouldBeFullyVisited()
        {
            // Arrange - Create a more complex hierarchy
            var root = new SensorGroup("Root");

            var tempGroup = new SensorGroup("Temperature Sensors");
            var humidityGroup = new SensorGroup("Humidity Sensors");

            var tempSensor1 = new SensorLeaf("t1", "Temp1");
            var tempSensor2 = new SensorLeaf("t2", "Temp2");
            var tempSensor3 = new SensorLeaf("t3", "Temp3");

            var humSensor1 = new SensorLeaf("h1", "Humidity1");
            var humSensor2 = new SensorLeaf("h2", "Humidity2");

            // Build hierarchy
            root.AddComponent(tempGroup);
            root.AddComponent(humidityGroup);

            tempGroup.AddComponent(tempSensor1);
            tempGroup.AddComponent(tempSensor2);

            humidityGroup.AddComponent(humSensor1);
            humidityGroup.AddComponent(humSensor2);
            humidityGroup.AddComponent(tempSensor3); // Mixed sensor type in humidity group

            var visitor = new MockVisitor();

            // Act
            root.Accept(visitor);

            // Assert
            Assert.AreEqual(5, visitor.LeafVisitCount, "Should visit all 5 leaf sensors");
            Assert.AreEqual(3, visitor.GroupVisitCount, "Should visit all 3 groups");
            Assert.AreEqual(8, visitor.VisitedNames.Count, "Should visit 8 components total");

            // Verify that each component was visited
            string[] expectedNames = {
                "Root", "Temperature Sensors", "Humidity Sensors",
                "Temp1", "Temp2", "Temp3", "Humidity1", "Humidity2"
            };

            foreach (var name in expectedNames)
            {
                CollectionAssert.Contains(visitor.VisitedNames, name, $"Should visit component: {name}");
            }
        }
    }
}