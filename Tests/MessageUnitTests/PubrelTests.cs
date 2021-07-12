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
    public class PubretTests
    {
        [TestMethod]
        public void PubrelBasicEncodeTestv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 98, 2, 0, 42 };
            MqttMsgPubrel pubrel = new();
            pubrel.MessageId = 42;
            // Act
            byte[] encoded = pubrel.GetBytes(MqttProtocolVersion.Version_3_1_1);
            Helpers.DumpBuffer(encoded);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrelBasicEncodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 98, 4, 0, 42, 0, 0 };
            MqttMsgPubrel pubrel = new();
            pubrel.MessageId = 42;
            // Act
            byte[] encoded = pubrel.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrelAdvancedEncodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 98,51,0,42,151,47,31,0,26,89,111,117,32,104,97,118,101,32,101,120,99,101,101,100,
                32,121,111,117,114,32,113,117,111,116,97,38,0,6,80,114,111,112,32,49,38,0,6,80,114,
                111,112,32,50 };
            MqttMsgPubrel pubrel = new();
            pubrel.MessageId = 42;
            pubrel.ReasonCode = MqttReasonCode.QuotaExceeded;
            pubrel.Reason = "You have exceed your quota";
            pubrel.UserProperties.Add("Prop 1");
            pubrel.UserProperties.Add("Prop 2");
            // Act
            byte[] encoded = pubrel.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrelAdvancedEncodeMaximumPacketSizeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 98, 4, 0, 42, 0x97, 0 };
            MqttMsgPubrel pubrel = new();
            pubrel.MessageId = 42;
            pubrel.MaximumPacketSize = 5;
            // This should not be send at all as exceeding the maximum packet size
            pubrel.ReasonCode = MqttReasonCode.QuotaExceeded;
            pubrel.Reason = "You have exceed your quota";
            pubrel.UserProperties.Add("Prop 1");
            pubrel.UserProperties.Add("Prop 2");
            // Act
            byte[] encoded = pubrel.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PubrelBasicDecodeTestv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 2, 0, 42 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPubrel pubrel = MqttMsgPubrel.Parse(98, MqttProtocolVersion.Version_3_1_1, mokChannel);
            // Assert
            Assert.Equal((ushort)42, pubrel.MessageId);
        }

        [TestMethod]
        public void PubrelBasicDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 4, 0, 42, 0x97, 0 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPubrel pubrel = MqttMsgPubrel.Parse(98, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((ushort)42, pubrel.MessageId);
            Assert.Equal((byte)0x97, (byte)pubrel.ReasonCode);
        }

        [TestMethod]
        public void PubrelAdvanceDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 51,0,42,151,47,31,0,26,89,111,117,32,104,97,118,101,32,101,120,99,101,101,100,
                32,121,111,117,114,32,113,117,111,116,97,38,0,6,80,114,111,112,32,49,38,0,6,80,114,
                111,112,32,50 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPubrel pubrel = MqttMsgPubrel.Parse(98, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((ushort)42, pubrel.MessageId);
            Assert.Equal((byte)pubrel.ReasonCode, (byte)MqttReasonCode.QuotaExceeded);
            Assert.Equal(pubrel.Reason, "You have exceed your quota");
            Assert.Equal(pubrel.UserProperties.Count, 2);
            Assert.Equal((string)pubrel.UserProperties[0], "Prop 1");
            Assert.Equal((string)pubrel.UserProperties[1], "Prop 2");
        }
    }
}
