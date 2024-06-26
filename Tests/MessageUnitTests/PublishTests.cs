﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Text;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Exceptions;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.TestFramework;

namespace MessageUnitTests
{
    [TestClass]
    public class PublishTests
    {
        private const string Topic = "thistopic/something";
        private const string TopicWildcard = "thistopic/something/#";
        private const string MessageString = "This is a string message";
        private const string MessageJsonString = @"{
  ""_rid"": """",
  ""Databases"": [
    {
            ""id"": ""HomeAutomation"",
      ""_rid"": ""MfAzAA=="",
      ""_self"": ""dbs/MFzAA==/"",
      ""_etag"": ""\""000020002-0000-0a00-0000-620019f80000\"""",
      ""_colls"": ""colls/"",
      ""_users"": ""users/"",
      ""_ts"": 1644173816
    }
  ],
  ""_count"": 1
}";
        private const ushort MessageId = 42;

        [TestMethod]
        public void PublishBasicEncodeTestv311()
        {
            // Arrange
            MqttMsgPublish publish = new(Topic, Encoding.UTF8.GetBytes(MessageString), true, MqttQoSLevel.ExactlyOnce, false);
            publish.MessageId = MessageId;
            byte[] encodedCorrect = new byte[] {60,47,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,84,104,105,115,32,105,115,32,97,32,115,116,114,105,110,103,32,109,101,115,
                115,97,103,101};
            // Act
            byte[] encoded = publish.GetBytes(MqttProtocolVersion.Version_3_1_1);
            // Assert
            CollectionAssert.AreEqual(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PublishBasicDecodeTestv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 47,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,84,104,105,115,32,105,115,32,97,32,115,116,114,105,110,103,32,109,101,115,
                115,97,103,101};
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPublish publish = MqttMsgPublish.Parse(60, MqttProtocolVersion.Version_3_1_1, mokChannel);
            // Assert
            Assert.AreEqual(Topic, publish.Topic);
            Assert.AreEqual(MessageId, publish.MessageId);
            Assert.AreEqual(MessageString, Encoding.UTF8.GetString(publish.Message, 0, publish.Message.Length));
            Assert.AreEqual((byte)MqttQoSLevel.ExactlyOnce, (byte)publish.QosLevel);
            Assert.AreEqual(true, publish.DupFlag);
            Assert.AreEqual(false, publish.Retain);
        }

        [TestMethod]
        public void PublishBasicEncodeTestv5()
        {
            // Arrange
            MqttMsgPublish publish = new(Topic, Encoding.UTF8.GetBytes(MessageString), true, MqttQoSLevel.ExactlyOnce, false);
            publish.MessageId = MessageId;
            byte[] encodedCorrect = new byte[] {60,48,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,0,84,104,105,115,32,105,115,32,97,32,115,116,114,105,110,103,32,109,101,
                115,115,97,103,101};
            // Act
            byte[] encoded = publish.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            CollectionAssert.AreEqual(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PublishUserPropertiesTest()
        {
            // Can't call Publish() because it will try to enqueue the message and this will fail.
            // Instead call ComposeMqttMsgPublish() which will be calling the constructor and perform the validations.

            // Arrange
            var client = new MqttClient();

            OutputHelper.WriteLine("");
            OutputHelper.WriteLine("== Testing Publish with user properties. ==");
            OutputHelper.WriteLine("");

            // OK adding a null to user properties
            OutputHelper.WriteLine("Adding a null to user properties collection.");

            _ = client.ComposeMqttMsgPublish(
                Topic,
                Encoding.UTF8.GetBytes(MessageString),
                null,
                null,
                MqttQoSLevel.ExactlyOnce,
                false);

            // OK adding an empty collection to user properties
            OutputHelper.WriteLine("Adding an empty collection to user properties collection.");

            _ = client.ComposeMqttMsgPublish(
                Topic,
                Encoding.UTF8.GetBytes(MessageString),
                null,
                new ArrayList(),
                MqttQoSLevel.ExactlyOnce,
                false);

            // add invalid user properties
            OutputHelper.WriteLine("Adding a collection of strings to user properties collection, which is not valid.");

            Assert.ThrowsException(typeof(ArgumentException), () =>
            {
                _ = client.ComposeMqttMsgPublish(
                    Topic,
                    Encoding.UTF8.GetBytes(MessageString),
                    null,
                    new ArrayList()
                    {
                        "I'm a fake user property",
                        "I'm another fake user property"
                    },
                    MqttQoSLevel.ExactlyOnce,
                    false);
            },
            "Adding array of strings to user properties should throw ArgumentException."
            );

            // add invalid user properties
            OutputHelper.WriteLine("Adding a collection of integers to user properties collection, which is not valid.");

            Assert.ThrowsException(typeof(ArgumentException), () =>
            {
                _ = client.ComposeMqttMsgPublish(
                    Topic,
                    Encoding.UTF8.GetBytes(MessageString),
                    null,
                    new ArrayList()
                    {
                        777,
                        888,
                        999
                    },
                    MqttQoSLevel.ExactlyOnce,
                    false
                    );
            },
            "Adding array of integers to user properties should throw ArgumentException."
            );

            // add proper user properties
            OutputHelper.WriteLine("Adding a valid collection of user properties.");

            _ = client.ComposeMqttMsgPublish(
                Topic,
                Encoding.UTF8.GetBytes(MessageString),
                null,
                new ArrayList()
                {
                    new UserProperty("property1", "property1_value"),
                    new UserProperty("property2", "property2_value")
                },
                MqttQoSLevel.ExactlyOnce,
                false);
        }


        [TestMethod]
        public void PublishAdvancedEncodeTestv5_00()
        {
            // Arrange
            MqttMsgPublish publish = new(Topic, Encoding.UTF8.GetBytes(MessageString), true, MqttQoSLevel.ExactlyOnce, false);
            publish.MessageId = MessageId;
            publish.ContentType = "UTF8";
            publish.IsPayloadUTF8 = true;
            publish.SubscriptionIdentifier = 268435454;
            publish.UserProperties.Add(new UserProperty("One", "prop"));
            publish.UserProperties.Add(new UserProperty("second", "property"));
            publish.CorrelationData = new byte[] { 1, 2, 3, 4, 5, 6 };
            publish.ResponseTopic = "response topic";
            publish.TopicAlias = 33;
            publish.MessageExpiryInterval = 12345;
            byte[] encodedCorrect = new byte[] { 60,127,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,79,1,1,2,0,0,48,57,35,0,33,8,0,14,114,101,115,112,111,110,115,101,32,116,
                111,112,105,99,9,0,6,1,2,3,4,5,6,38,0,3,79,110,101,0,4,112,114,111,112,38,0,6,115,
                101,99,111,110,100,0,8,112,114,111,112,101,114,116,121,11,254,255,255,127,3,0,4,85,
                84,70,56,84,104,105,115,32,105,115,32,97,32,115,116,114,105,110,103,32,109,101,115,
                115,97,103,101 };
            // Act
            byte[] encoded = publish.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            CollectionAssert.AreEqual(encodedCorrect, encoded);
        }

        [TestMethod]
        public void PublishAdvancedEncodeTestv5_01()
        {
            // Arrange
            MqttMsgPublish publish = new(Topic, Encoding.UTF8.GetBytes(MessageJsonString), true, MqttQoSLevel.ExactlyOnce, false);

            publish.MessageId = MessageId;
            publish.ContentType = "application/json";
            publish.IsPayloadUTF8 = true;
            publish.SubscriptionIdentifier = 268435454;
            publish.UserProperties.Add(new UserProperty("One", "prop"));
            publish.UserProperties.Add(new UserProperty("second", "property"));
            publish.CorrelationData = new byte[] { 1, 2, 3, 4, 5, 6 };
            publish.ResponseTopic = "response topic";
            publish.TopicAlias = 33;
            publish.MessageExpiryInterval = 12345;

            byte[] encodedCorrect = new byte[] {
                60, 167, 3, 0, 19, 116, 104, 105, 115, 116, 111, 112, 105, 99, 47, 115, 111, 109, 101, 116, 104, 105, 110,
                103, 0, 42, 91, 1, 1, 2, 0, 0, 48, 57, 35, 0, 33, 8, 0, 14, 114, 101, 115, 112, 111, 110, 115, 101, 32,
                116, 111, 112, 105, 99, 9, 0, 6, 1, 2, 3, 4, 5, 6, 38, 0, 3, 79, 110, 101, 0, 4, 112, 114, 111, 112, 38,
                0, 6, 115, 101, 99, 111, 110, 100, 0, 8, 112, 114, 111, 112, 101, 114, 116, 121, 11, 254, 255, 255, 127,
                3, 0, 16, 97, 112, 112, 108, 105, 99, 97, 116, 105, 111, 110, 47, 106, 115, 111, 110, 123, 13, 10, 32,
                32, 34, 95, 114, 105, 100, 34, 58, 32, 34, 34, 44, 13, 10, 32, 32, 34, 68, 97, 116, 97, 98, 97, 115,
                101, 115, 34, 58, 32, 91, 13, 10, 32, 32, 32, 32, 123, 13, 10, 32, 32, 32, 32, 32, 32, 32, 32, 32,
                32, 32, 32, 34, 105, 100, 34, 58, 32, 34, 72, 111, 109, 101, 65, 117, 116, 111, 109, 97, 116, 105,
                111, 110, 34, 44, 13, 10, 32, 32, 32, 32, 32, 32, 34, 95, 114, 105, 100, 34, 58, 32, 34, 77, 102,
                65, 122, 65, 65, 61, 61, 34, 44, 13, 10, 32, 32, 32, 32, 32, 32, 34, 95, 115, 101, 108, 102, 34,
                58, 32, 34, 100, 98, 115, 47, 77, 70, 122, 65, 65, 61, 61, 47, 34, 44, 13, 10, 32, 32, 32, 32,
                32, 32, 34, 95, 101, 116, 97, 103, 34, 58, 32, 34, 92, 34, 48, 48, 48, 48, 50, 48, 48, 48, 50,
                45, 48, 48, 48, 48, 45, 48, 97, 48, 48, 45, 48, 48, 48, 48, 45, 54, 50, 48, 48, 49, 57, 102,
                56, 48, 48, 48, 48, 92, 34, 34, 44, 13, 10, 32, 32, 32, 32, 32, 32, 34, 95, 99, 111, 108,
                108, 115, 34, 58, 32, 34, 99, 111, 108, 108, 115, 47, 34, 44, 13, 10, 32, 32, 32, 32, 32,
                32, 34, 95, 117, 115, 101, 114, 115, 34, 58, 32, 34, 117, 115, 101, 114, 115, 47, 34, 44,
                13, 10, 32, 32, 32, 32, 32, 32, 34, 95, 116, 115, 34, 58, 32, 49, 54, 52, 52, 49, 55, 51,
                56, 49, 54, 13, 10, 32, 32, 32, 32, 125, 13, 10, 32, 32, 93, 44, 13, 10, 32, 32, 34, 95,
                99, 111, 117, 110, 116, 34, 58, 32, 49, 13, 10, 125
            };

            // Act
            byte[] encoded = publish.GetBytes(MqttProtocolVersion.Version_5);

            // Assert
            CollectionAssert.AreEqual(encodedCorrect, encoded);
        }


        [TestMethod]
        public void PublishAdvancedBinaryEncodeTestv5()
        {
            byte[] MessageByte = new byte[] { 6, 5, 4, 3, 2, 1 };
            // Arrange
            MqttMsgPublish publish = new(Topic, MessageByte, true, MqttQoSLevel.ExactlyOnce, false);
            publish.MessageId = MessageId;
            publish.ContentType = "binary";
            publish.IsPayloadUTF8 = false;
            publish.SubscriptionIdentifier = 268435454;
            publish.UserProperties.Add(new UserProperty("One", "prop"));
            publish.UserProperties.Add(new UserProperty("second", "property"));
            publish.CorrelationData = new byte[] { 1, 2, 3, 4, 5, 6 };
            publish.ResponseTopic = "response topic";
            publish.TopicAlias = 33;
            publish.MessageExpiryInterval = 12345;
            byte[] encodedCorrect = new byte[] { 60,109,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,79,2,0,0,48,57,35,0,33,8,0,14,114,101,115,112,111,110,115,101,32,116,111,
                112,105,99,9,0,6,1,2,3,4,5,6,38,0,3,79,110,101,0,4,112,114,111,112,38,0,6,115,101,
                99,111,110,100,0,8,112,114,111,112,101,114,116,121,11,254,255,255,127,3,0,6,98,105,
                110,97,114,121,6,5,4,3,2,1};
            // Act
            byte[] encoded = publish.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            CollectionAssert.AreEqual(encodedCorrect, encoded);
        }
        [TestMethod]
        public void PublishAdvancedBinaryDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 109,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,79,2,0,0,48,57,35,0,33,8,0,14,114,101,115,112,111,110,115,101,32,116,111,
                112,105,99,9,0,6,1,2,3,4,5,6,38,0,3,79,110,101,0,4,112,114,111,112,38,0,6,115,101,
                99,111,110,100,0,8,112,114,111,112,101,114,116,121,11,254,255,255,127,3,0,6,98,105,
                110,97,114,121,6,5,4,3,2,1 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPublish publish = MqttMsgPublish.Parse(60, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.AreEqual(Topic, publish.Topic);
            Assert.AreEqual(MessageId, publish.MessageId);
            CollectionAssert.AreEqual(new byte[] { 6, 5, 4, 3, 2, 1 }, publish.Message);
            Assert.AreEqual((byte)MqttQoSLevel.ExactlyOnce, (byte)publish.QosLevel);
            Assert.AreEqual(true, publish.DupFlag);
            Assert.AreEqual(false, publish.Retain);
            Assert.AreEqual(publish.ContentType, "binary");
            Assert.AreEqual(publish.IsPayloadUTF8, false);
            Assert.AreEqual(publish.SubscriptionIdentifier, 268435454);
            Assert.AreEqual(publish.UserProperties.Count, 2);
            var prop = new UserProperty("One", "prop");
            Assert.AreEqual(((UserProperty)publish.UserProperties[0]).Name, prop.Name);
            Assert.AreEqual(((UserProperty)publish.UserProperties[0]).Value, prop.Value);
            prop = new UserProperty("second", "property");
            Assert.AreEqual(((UserProperty)publish.UserProperties[1]).Name, prop.Name);
            Assert.AreEqual(((UserProperty)publish.UserProperties[1]).Value, prop.Value);
            CollectionAssert.AreEqual(publish.CorrelationData, new byte[] { 1, 2, 3, 4, 5, 6 });
            Assert.AreEqual(publish.ResponseTopic, "response topic");
            Assert.AreEqual(publish.TopicAlias, (ushort)33);
            Assert.AreEqual(publish.MessageExpiryInterval, 12345);
        }

        [TestMethod]
        public void PublishAdvancedDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 127,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,79,1,1,2,0,0,48,57,35,0,33,8,0,14,114,101,115,112,111,110,115,101,32,116,
                111,112,105,99,9,0,6,1,2,3,4,5,6,38,0,3,79,110,101,0,4,112,114,111,112,38,0,6,115,
                101,99,111,110,100,0,8,112,114,111,112,101,114,116,121,11,254,255,255,127,3,0,4,85,
                84,70,56,84,104,105,115,32,105,115,32,97,32,115,116,114,105,110,103,32,109,101,115,
                115,97,103,101 };
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPublish publish = MqttMsgPublish.Parse(60, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.AreEqual(Topic, publish.Topic);
            Assert.AreEqual(MessageId, publish.MessageId);
            Assert.AreEqual(MessageString, Encoding.UTF8.GetString(publish.Message, 0, publish.Message.Length));
            Assert.AreEqual((byte)MqttQoSLevel.ExactlyOnce, (byte)publish.QosLevel);
            Assert.AreEqual(true, publish.DupFlag);
            Assert.AreEqual(false, publish.Retain);
            Assert.AreEqual(publish.ContentType, "UTF8");
            Assert.AreEqual(publish.IsPayloadUTF8, true);
            Assert.AreEqual(publish.SubscriptionIdentifier, 268435454);
            Assert.AreEqual(publish.UserProperties.Count, 2);
            var prop = new UserProperty("One", "prop");
            Assert.AreEqual(((UserProperty)publish.UserProperties[0]).Name, prop.Name);
            Assert.AreEqual(((UserProperty)publish.UserProperties[0]).Value, prop.Value);
            prop = new UserProperty("second", "property");
            Assert.AreEqual(((UserProperty)publish.UserProperties[1]).Name, prop.Name);
            Assert.AreEqual(((UserProperty)publish.UserProperties[1]).Value, prop.Value);
            CollectionAssert.AreEqual(publish.CorrelationData, new byte[] { 1, 2, 3, 4, 5, 6 });
            Assert.AreEqual(publish.ResponseTopic, "response topic");
            Assert.AreEqual(publish.TopicAlias, (ushort)33);
            Assert.AreEqual(publish.MessageExpiryInterval, 12345);
        }

        [TestMethod]
        public void PublishBasicDecodeTestv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 48,0,19,116,104,105,115,116,111,112,105,99,47,115,111,109,101,116,104,105,110,
                103,0,42,0,84,104,105,115,32,105,115,32,97,32,115,116,114,105,110,103,32,109,101,
                115,115,97,103,101};
            MokChannel mokChannel = new(encodedCorrect);
            // Act
            MqttMsgPublish publish = MqttMsgPublish.Parse(60, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.AreEqual(Topic, publish.Topic);
            Assert.AreEqual(MessageId, publish.MessageId);
            Assert.AreEqual(MessageString, Encoding.UTF8.GetString(publish.Message, 0, publish.Message.Length));
            Assert.AreEqual((byte)MqttQoSLevel.ExactlyOnce, (byte)publish.QosLevel);
            Assert.AreEqual(true, publish.DupFlag);
            Assert.AreEqual(false, publish.Retain);
        }

        [TestMethod]
        public void PublishBasicEncodeExceptionTestv311()
        {
            Assert.ThrowsException(typeof(MqttClientException), () =>
            {
                // Arrange
                MqttMsgPublish publish = new(TopicWildcard, Encoding.UTF8.GetBytes(MessageString), true, MqttQoSLevel.ExactlyOnce, false);
                publish.MessageId = MessageId;
                // Act
                byte[] encoded = publish.GetBytes(MqttProtocolVersion.Version_3_1_1);
                // Assert
            });
        }
    }
}
