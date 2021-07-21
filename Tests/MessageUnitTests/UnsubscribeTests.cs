// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.TestFramework;

namespace MessageUnitTests
{
    [TestClass]
    public class UnsubscribeTests
    {
        [TestMethod]
        public void UnbscribeBasicEncodeTestv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 162, 18, 0, 42, 0, 6, 116, 112, 111, 105, 99, 49, 0, 6, 116, 111, 112, 105, 99, 50 };
            MqttMsgUnsubscribe unsubscribe = new MqttMsgUnsubscribe(new string[] { "tpoic1", "topic2" });
            unsubscribe.MessageId = 42;
            // Act
            byte[] encoded = unsubscribe.GetBytes(MqttProtocolVersion.Version_3_1_1);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void UnbscribeBasicEncodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 162, 19, 0, 42, 0, 0, 6, 116, 112, 111, 105, 99, 49, 0, 6, 116, 111, 112, 105, 99, 50 };
            MqttMsgUnsubscribe unsubscribe = new MqttMsgUnsubscribe(new string[] { "tpoic1", "topic2" });
            unsubscribe.MessageId = 42;
            // Act
            byte[] encoded = unsubscribe.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void UnbscribeAdvanceEncodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 162,43,0,42,24,38,0,21,87,111,119,44,32,97,110,111,116,104,101,114,32,112,114,111,
                112,101,114,116,121,0,6,116,112,111,105,99,49,0,6,116,111,112,105,99,50};
            MqttMsgUnsubscribe unsubscribe = new MqttMsgUnsubscribe(new string[] { "tpoic1", "topic2" });
            unsubscribe.MessageId = 42;
            unsubscribe.UserProperties.Add("Wow, another property");
            // Act
            byte[] encoded = unsubscribe.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void UnbscribeBasicDecodeTestv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 18, 0, 42, 0, 6, 116, 112, 111, 105, 99, 49, 0, 6, 116, 111, 112, 105, 99, 50 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgUnsubscribe unsubscribe = MqttMsgUnsubscribe.Parse(162, MqttProtocolVersion.Version_3_1_1, mokChannel);
            // Assert
            Assert.Equal((ushort)42, unsubscribe.MessageId);
            Assert.Equal(unsubscribe.Topics.Length, 2);
            Assert.Equal(unsubscribe.Topics, new string[] { "tpoic1", "topic2" });
        }

        [TestMethod]
        public void UnbscribeBasicDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 19, 0, 42, 0, 0, 6, 116, 112, 111, 105, 99, 49, 0, 6, 116, 111, 112, 105, 99, 50 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgUnsubscribe unsubscribe = MqttMsgUnsubscribe.Parse(162, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((ushort)42, unsubscribe.MessageId);
            Assert.Equal(unsubscribe.Topics.Length, 2);
            Assert.Equal(unsubscribe.Topics, new string[] { "tpoic1", "topic2" });
        }

        [TestMethod]
        public void UnbscribeAdvanceDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 43,0,42,24,38,0,21,87,111,119,44,32,97,110,111,116,104,101,114,32,112,114,111,
                112,101,114,116,121,0,6,116,112,111,105,99,49,0,6,116,111,112,105,99,50};
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgUnsubscribe unsubscribe = MqttMsgUnsubscribe.Parse(162, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((ushort)42, unsubscribe.MessageId);
            Assert.Equal(unsubscribe.Topics.Length, 2);
            Assert.Equal(unsubscribe.Topics, new string[] { "tpoic1", "topic2" });
            Assert.Equal("Wow, another property", (string)unsubscribe.UserProperties[0]);
        }
    }
}
