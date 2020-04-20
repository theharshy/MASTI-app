// -----------------------------------------------------------------------
// <author> 
//      Libin N George
// </author>
//
// <date> 
//      12-10-2018 
// </date>
// 
// <reviewer>
//      Ayush Mittal
// </reviewer>
//
// <copyright file="DataRecevialNotifier.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
//
// <summary>
//      This file contains Data Receiver Publisher 
//      Publisher - DataReceiveNotifier
//      Function for Subscribing -  SubscribeForDataReceival
//      This file is a part of Networking Module
// </summary>
// -----------------------------------------------------------------------

namespace Masti.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Masti.Schema;

    /// <summary>
    /// Notifier for Receiving Data Component of Communication Class
    /// Subscription for receiving messages 
    /// and Triggering the Delegate on receiving message or data 
    /// </summary>
    public partial class Communication : ICommunication
    {
       /// <summary>
        /// Holds Instance on ISchema for usage in Communication
        /// </summary>
        private ISchema schema;

        /// <summary>
        /// Dictionary which maps Datatype with event handlers (which is triggered on data receival)
        /// </summary>
        private Dictionary<DataType, DataReceivalHandler> dataReceivalEventMap = new Dictionary<DataType, DataReceivalHandler>
        {
            { DataType.ImageSharing, null },
            { DataType.Message, null }
        };

        /// <summary>
        /// Gets or sets instance on ISchema for usage in Communication
        /// </summary>
        public ISchema Schema
        {
            get
            {
                return this.schema;
            }

            set
            {
                this.schema = value;
            }
        }

        /// <summary>
        /// Subscribes the Communication module for receiving messages.
        /// </summary>
        /// <param name="type">type (Message or ImageSharing) of data which should trigger corresponding event handler </param>
        /// <param name="receivalHandler">Event handler which has to be called when data with given type is received</param>
        /// <returns>true on success</returns>
        public bool SubscribeForDataReceival(DataType type, DataReceivalHandler receivalHandler)
        {
            // Protecting Dictionary from concurrent calls
            lock (this.dataReceivalEventMap)
            {
                try
                {
                    this.dataReceivalEventMap[type] += receivalHandler;
                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// internal method for triggering Event handlers
        /// </summary>
        /// <param name="data">Data which is received</param>
        /// <param name="fromIP">IP Address from which data is received</param>
        /// <returns>returns true on success , false on Invalid Tag </returns>
        public bool DataReceiveNotifier(string data, IPAddress fromIP)
        {
            IDictionary<string, string> decodeResult = this.schema.Decode(data, true);
            DataType fromModule;

            // Finding type of Data received
            if (decodeResult["type"].Equals("ImageProcessing", StringComparison.OrdinalIgnoreCase))
            {
                fromModule = DataType.ImageSharing;
            }
            else if (decodeResult["type"].Equals("Messaging", StringComparison.OrdinalIgnoreCase)) 
            {
                fromModule = DataType.Message;
            }
            else
            {
                // Case type is not valid
                return false;
            }

            // Calling Delegates
            if (this.dataReceivalEventMap[fromModule] != null)
            {
                this.dataReceivalEventMap[fromModule](data, fromIP);
            }

            return true;
        }
    }
}
