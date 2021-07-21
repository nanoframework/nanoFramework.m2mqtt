// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.TestFramework;

namespace MessageUnitTests
{
    [TestClass]
    class SubackTests
    {
        [TestMethod]
        public void SubackBasicEncodingTestsv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 144, 4, 0, 42, 1, 2 };
            MqttMsgSuback suback = new();
            suback.MessageId = 42;
            suback.GrantedQoSLevels = new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce, MqttQoSLevel.ExactlyOnce };
            // Act
            byte[] encoded = suback.GetBytes(MqttProtocolVersion.Version_3_1_1);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void SubackBasicEncodingTestsv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 144, 5, 0, 42, 0, 1, 2 };
            MqttMsgSuback suback = new();
            suback.MessageId = 42;
            suback.GrantedQoSLevels = new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce, MqttQoSLevel.ExactlyOnce };
            // Act
            byte[] encoded = suback.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void SubackAdvanceEncodingTestsv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 144,84,0,42,78,31,0,49,84,104,105,115,32,105,115,32,97,32,103,111,111,100,32,114,
                101,97,115,111,110,58,32,121,111,117,114,32,97,114,101,32,110,111,116,32,97,117,116,
                104,111,114,105,122,101,100,33,33,33,38,0,23,111,110,101,32,112,114,111,112,32,111,
                110,108,121,32,116,104,105,115,32,116,105,109,101,1,2,135 };
            MqttMsgSuback suback = new();
            suback.MessageId = 42;
            suback.ReasonCodes = new MqttReasonCode[] { MqttReasonCode.GrantedQoS1, MqttReasonCode.GrantedQoS2, MqttReasonCode.NotAuthorized };
            suback.Reason = "This is a good reason: your are not authorized!!!";
            suback.UserProperties.Add("one prop only this time");
            // Act
            byte[] encoded = suback.GetBytes(MqttProtocolVersion.Version_5);
            // Assert
            Assert.Equal(encodedCorrect, encoded);
        }

        [TestMethod]
        public void SubackBasicDecodingTestsv311()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 4, 0, 42, 1, 2 };
            MokChannel mokChannel = new MokChannel(encodedCorrect);
            // Act
            MqttMsgSuback suback = MqttMsgSuback.Parse(144, MqttProtocolVersion.Version_3_1_1, mokChannel);
            // Assert
            Assert.Equal((ushort)42, suback.MessageId);
            Assert.Equal(suback.GrantedQoSLevels, new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce, MqttQoSLevel.ExactlyOnce });
        }

        [TestMethod]
        public void SubackBasicDecodingTestsv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 5, 0, 42, 0, 1, 2 };
            MokChannel mokChannel = new MokChannel(encodedCorrect);
            // Act
            MqttMsgSuback suback = MqttMsgSuback.Parse(144, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((ushort)42, suback.MessageId);
            Assert.Equal(suback.GrantedQoSLevels, new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce, MqttQoSLevel.ExactlyOnce });
            Assert.Equal(suback.ReasonCodes, new MqttReasonCode[] { MqttReasonCode.GrantedQoS1, MqttReasonCode.GrantedQoS2 });
            // And this should work as well as it's value comparison
            Assert.Equal(suback.GrantedQoSLevels, new MqttReasonCode[] { MqttReasonCode.GrantedQoS1, MqttReasonCode.GrantedQoS2 });
        }

        [TestMethod]
        public void SubackAdvancedDecodingTestsv5()
        {
            // Arrange
            byte[] encodedCorrect = new byte[] { 84,0,42,78,31,0,49,84,104,105,115,32,105,115,32,97,32,103,111,111,100,32,114,
                101,97,115,111,110,58,32,121,111,117,114,32,97,114,101,32,110,111,116,32,97,117,116,
                104,111,114,105,122,101,100,33,33,33,38,0,23,111,110,101,32,112,114,111,112,32,111,
                110,108,121,32,116,104,105,115,32,116,105,109,101,1,2,135 };
            MokChannel mokChannel = new MokChannel(encodedCorrect);
            // Act
            MqttMsgSuback suback = MqttMsgSuback.Parse(144, MqttProtocolVersion.Version_5, mokChannel);
            // Assert
            Assert.Equal((ushort)42, suback.MessageId);
            Assert.Equal(suback.GrantedQoSLevels, new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce, MqttQoSLevel.ExactlyOnce, (MqttQoSLevel)MqttReasonCode.NotAuthorized });
            Assert.Equal(suback.ReasonCodes, new MqttReasonCode[] { MqttReasonCode.GrantedQoS1, MqttReasonCode.GrantedQoS2, MqttReasonCode.NotAuthorized });
            // And this should work as well as it's value comparison
            Assert.Equal(suback.GrantedQoSLevels, new MqttReasonCode[] { MqttReasonCode.GrantedQoS1, MqttReasonCode.GrantedQoS2, MqttReasonCode.NotAuthorized });
        }
    }
}
