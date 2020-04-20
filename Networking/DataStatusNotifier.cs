//-----------------------------------------------------------------------
// <author> 
//     Athul.M.A 
// </author>
//
// <date> 
//    12/10/2018 
// </date>
// 
// <reviewer> 
//     Ayush Mittal
//     Libin N George
// </reviewer>
// 
// <copyright file="DataStatusNotifier.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This file implements the Data Status Handler that notifies subscribers the status of their message send operation
// </summary>
//---------------

namespace Masti.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Notifies the Subscribers 
    /// if a message was sent successfully or not.
    /// Invoked by the Data Outgoing Component of Networking Module
    /// </summary>
    public partial class Communication : ICommunication
    {
        /// <summary>
        /// Event handler for the Messaging module
        /// </summary>
        private event DataStatusHandler MessageDataStatusEventHandler = null;

        /// <summary>
        /// Event handler for the Image Sharing module
        /// </summary>
        private event DataStatusHandler ImageSharingDataStatusEventHandler = null;

        /// <summary>
        /// Subscribes the function calling module
        /// to be notified of the status of a message 
        /// </summary>
        /// <param name="type">Subscription name (Message or ImageSharing) with which a module subscribes to DataStatusNotifier</param>
        /// <param name="statusHandler">Event handler that has to be called when data with given type is received</param>
        /// <returns>true on successful subscription; false otherwise</returns>
        public bool SubscribeForDataStatus(DataType type, DataStatusHandler statusHandler)
        {
            switch (type)
            {
                case DataType.Message:
                    {
                        this.MessageDataStatusEventHandler += statusHandler;
                        return true;
                    }

                case DataType.ImageSharing:
                    {
                        this.ImageSharingDataStatusEventHandler += statusHandler;
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Notifies the module that sent the message
        /// of ID = "dataID" the status of the operation
        /// and calls the corresponding event handler
        /// </summary>
        /// <param name="data">The message itself</param>
        /// <param name="status">Status of the message send operation</param>
        public void DataStatusNotify(string data, StatusCode status)
        {
            IDictionary<string, string> decodeResult = this.schema.Decode(data, true);
            DataType fromModule;

            // Finding module from which the data originated
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
                return;
            }

            switch (fromModule)
            {
                case DataType.Message:
                    {
                        if (this.MessageDataStatusEventHandler != null)
                        {
                            this.MessageDataStatusEventHandler(data, status);
                        }

                        break;
                    }

                case DataType.ImageSharing:
                    {
                        if (this.ImageSharingDataStatusEventHandler != null)
                        {
                            this.ImageSharingDataStatusEventHandler(data, status);
                        }

                        break;
                    }

                default:
                    {
                        return;
                    }
            }
        }
    }
}
