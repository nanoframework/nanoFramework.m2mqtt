﻿/*
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution. 

The Eclipse Public License is available at 
   http://www.eclipse.org/legal/epl-v10.html
and the Eclipse Distribution License is available at 
   http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
   .NET Foundation and Contributors - nanoFramework support
*/

namespace nanoFramework.M2Mqtt
{
    /// <summary>
    /// Interface for channel under MQTT library
    /// </summary>
    public interface IMqttNetworkChannel
    {
        /// <summary>
        /// Data available on channel
        /// </summary>
        bool DataAvailable { get; }

        /// <summary>
        /// True to check the certificate when connecting using TLS.
        /// Disabling this property will bypass the validation of the server root CA certificate.
        /// Using the library this way it's unsecured and, therefore, not recommended.
        /// </summary>
        bool ValidateServerCertificate { get; set; }

        /// <summary>
        /// Receive data from the network channel
        /// </summary>
        /// <param name="buffer">Data buffer for receiving data</param>
        /// <returns>Number of bytes received</returns>
        int Receive(byte[] buffer);

        /// <summary>
        /// Receive data from the network channel with a specified timeout
        /// </summary>
        /// <param name="buffer">Data buffer for receiving data</param>
        /// <param name="timeout">Timeout on receiving (in milliseconds)</param>
        /// <returns>Number of bytes received</returns>
        int Receive(byte[] buffer, int timeout);

        /// <summary>
        /// Send data on the network channel to the broker
        /// </summary>
        /// <param name="buffer">Data buffer to send</param>
        /// <returns>Number of byte sent</returns>
        int Send(byte[] buffer);

        /// <summary>
        /// Close the network channel
        /// </summary>
        void Close();

        /// <summary>
        /// Connect to remote server
        /// </summary>
        void Connect();
        
        /// <summary>
        /// Accept client connection
        /// </summary>
        void Accept();
    }
}
