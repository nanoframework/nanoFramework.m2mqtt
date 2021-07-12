﻿using System;
using System.Diagnostics;
using System.Text;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Exceptions;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.TestFramework;

namespace MessageUnitTests
{
    [TestClass]
    public class PubrecTests
    {
        [TestMethod]
        public void PubrecBasicEncodeTestv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 80, 2, 0, 42 };
            MqttMsgPubrec pubrec = new();
            pubrec.MessageId = 42;
            // Act
            byte[] encoded = pubrec.GetBytes(MqttProtocolVersion.Version_3_1_1);
            Helpers.DumpBuffer(encoded);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrecBasicEncodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 80, 4, 0, 42, 0, 0 };
            MqttMsgPubrec pubrec = new();
            pubrec.MessageId = 42;
            // Act
            byte[] encoded = pubrec.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrecAdvancedEncodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 80,51,0,42,151,47,31,0,26,89,111,117,32,104,97,118,101,32,101,120,99,101,101,100,
                32,121,111,117,114,32,113,117,111,116,97,38,0,6,80,114,111,112,32,49,38,0,6,80,114,
                111,112,32,50 };
            MqttMsgPubrec pubrec = new();
            pubrec.MessageId = 42;
            pubrec.ReasonCode = MqttReasonCode.QuotaExceeded;
            pubrec.Reason = "You have exceed your quota";
            pubrec.UserProperties.Add("Prop 1");
            pubrec.UserProperties.Add("Prop 2");
            // Act
            byte[] encoded = pubrec.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrecAdvancedEncodeMaximumPacketSizeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 80, 4, 0, 42, 0x97, 0 };
            MqttMsgPubrec pubrec = new();
            pubrec.MessageId = 42;
            pubrec.MaximumPacketSize = 5;
            // This should not be send at all as exceeding the maximum packet size
            pubrec.ReasonCode = MqttReasonCode.QuotaExceeded;
            pubrec.Reason = "You have exceed your quota";
            pubrec.UserProperties.Add("Prop 1");
            pubrec.UserProperties.Add("Prop 2");
            // Act
            byte[] encoded = pubrec.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrecBasicDecodeTestv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 2, 0, 42 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPubrec pubrec = MqttMsgPubrec.Parse(80, MqttProtocolVersion.Version_3_1_1, mokChannel);
            // Assert
            Assert.Equal((ushort)42, pubrec.MessageId);
        }

        [TestMethod]
        public void PubrecBasicDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 4, 0, 42, 0x97, 0 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPubrec pubrec = MqttMsgPubrec.Parse(80, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((byte)MqttMessageType.PublishReceived, (byte)pubrec.Type);
            Assert.Equal((ushort)42, pubrec.MessageId);
            Assert.Equal((byte)0x97, (byte)pubrec.ReasonCode);
        }

        [TestMethod]
        public void PubrecAdvanceDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 51,0,42,151,47,31,0,26,89,111,117,32,104,97,118,101,32,101,120,99,101,101,100,
                32,121,111,117,114,32,113,117,111,116,97,38,0,6,80,114,111,112,32,49,38,0,6,80,114,
                111,112,32,50 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPubrec pubrec = MqttMsgPubrec.Parse(80, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((ushort)42, pubrec.MessageId);
            Assert.Equal((byte)pubrec.ReasonCode, (byte)MqttReasonCode.QuotaExceeded);
            Assert.Equal(pubrec.Reason, "You have exceed your quota");
            Assert.Equal(pubrec.UserProperties.Count, 2);
            Assert.Equal((string)pubrec.UserProperties[0], "Prop 1");
            Assert.Equal((string)pubrec.UserProperties[1], "Prop 2");
        }
    }
}
