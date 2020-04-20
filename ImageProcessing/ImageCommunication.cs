// -----------------------------------------------------------------------
// <author> 
//      Axel James
// </author>
//
// <date> 
//      22-10-2018 
// </date>
// 
// <reviewer>
//      Sooraj Tom
// </reviewer>
//
// <copyright file="ImageCommunication.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
//
// <summary>
//      This file is a part of ImageProcessing Module.
//      File contains methods to facilitate communication between ImageProcessing modules in server and client.
// </summary>
// -----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using IitPkd.SchemaTeam;
    using Networking;

    /// <summary>
    /// Delegate signature for receiving notifications on different signals
    /// </summary>
    /// <param name="data">Data received</param>
    /// <param name="fromIP">IP from which data has been received</param>
    public delegate void SignalHandler(string data, IPAddress fromIP);

    /// <summary>
    /// Indicate the type of data or signal being communicated
    /// </summary>
    public enum Signal
    {
        /// <summary>
        /// Use to signal client to start image sharing
        /// </summary>
        Start,

        /// <summary>
        /// Use to signal client to stop image sharing
        /// </summary>
        Stop,

        /// <summary>
        /// Use to signal client to start resend full image by Compression class
        /// </summary>
        Resend,

        /// <summary>
        /// Use to send image from client to server
        /// </summary>
        Image
    }

    /// <summary>
    /// Facilitates communication between ImageProcessing modules in clients and server
    /// </summary>
    public class ImageCommunication
    {
        /// <summary>
        /// Store a local pointer to Communication object
        /// </summary>
        private ICommunication communication;

        /// <summary>
        /// Store a pointer to ISchema object for encoding and decoding
        /// </summary>
        private ISchema schema;

        /// <summary>
        /// Map Signals to Handler functions
        /// </summary>
        private Dictionary<Signal, SignalHandler> signalReceivalEventMap = new Dictionary<Signal, SignalHandler>
        {
            { Signal.Start, null },
            { Signal.Stop, null },
            { Signal.Resend, null },
            { Signal.Image, null }
        };

        /// <summary>
        /// Stores the id of last signal sent.
        /// </summary>
        private ulong dataID;

        /// <summary>
        /// Initializes a new instance of the ImageCommunication class with 
        /// copies of Communication and Schema class objects.  
        /// It also registers 'ReceiveSignalAndNotify' method for data listening
        /// </summary>
        /// <param name="communication">ICommunication class object to be used</param>
        /// <param name="schema">ISchema class object to be used</param>
        public ImageCommunication(ICommunication communication, ISchema schema)
        {
            this.dataID = 0;
            this.schema = schema;
            this.communication = communication;

            if (this.communication != null)
            {
                this.communication.SubscribeForDataReceival(DataType.ImageSharing, this.ReceiveSignalAndNotify);
            }
        }

        /// <summary>
        /// Sends signal and data to given IP
        /// </summary>
        /// <param name="toIP">IP to which signal is to be sent</param>
        /// <param name="signalType">Indicates the type of signal being made</param>
        /// <param name="data"> To pass any string data to client</param>
        /// <returns>Returns 0 on successful call and -1 for invalid call</returns>
        public bool SignalImageModule(IPAddress toIP, Signal signalType, string data)
        {
            Dictionary<string, string> msgDictionary = new Dictionary<string, string>();
            string msg;

            // Handle different signals
            switch (signalType)
            {
                case Signal.Start:
                    msgDictionary.Add("signal", "Start");
                    msgDictionary.Add("imageData", data);
                    msg = this.schema.Encode(msgDictionary);
                    break;

                case Signal.Stop:
                    msgDictionary.Add("signal", "Stop");
                    msgDictionary.Add("imageData", data);
                    msg = this.schema.Encode(msgDictionary);
                    break;

                case Signal.Resend:
                    msgDictionary.Add("signal", "Resend");
                    msgDictionary.Add("imageData", data);
                    msg = this.schema.Encode(msgDictionary);
                    break;

                case Signal.Image:
                    msgDictionary.Add("signal", "Image");
                    msgDictionary.Add("imageData", data);
                    msg = this.schema.Encode(msgDictionary);
                    break;

                default:
                    // Case type is not valid
                    return false;
            }

            this.dataID = this.dataID + 1;
            this.communication.Send(msg, this.dataID, toIP, DataType.ImageSharing);

            return true;
        }

        /// <summary>
        /// Sends signal and data to given IP
        /// </summary>
        /// <param name="toIP">IP to which signal is to be sent</param>
        /// <param name="signalType">Indicates the type of signal being made</param>
        /// <returns>Returns 0 on successful call and -1 for invalid call</returns>
        public bool SignalImageModule(IPAddress toIP, Signal signalType)
        {
            return this.SignalImageModule(toIP, signalType, null);
        }

        /// <summary>
        /// This function is subscribed to Communication module's Datatype 'ImageProcessing'.
        /// It also notifies any methods registered for Signals.
        /// </summary>
        /// <param name="data">Data that is received</param>
        /// <param name="fromIP">IP address from which data has been received</param>
        public void ReceiveSignalAndNotify(string data, IPAddress fromIP)
        {
            IDictionary<string, string> decodedDict = this.schema.Decode(data, true);
            Signal signalType;

            // Finding type of Signal Received
            if (decodedDict["signal"].Equals("Image", StringComparison.OrdinalIgnoreCase))
            {
                signalType = Signal.Image;
            }
            else if (decodedDict["signal"].Equals("Start", StringComparison.OrdinalIgnoreCase))
            {
                signalType = Signal.Start;
            }
            else if (decodedDict["signal"].Equals("Stop", StringComparison.OrdinalIgnoreCase))
            {
                signalType = Signal.Stop;
            }
            else if (decodedDict["signal"].Equals("Resend", StringComparison.OrdinalIgnoreCase))
            {
                signalType = Signal.Resend;
            }
            else
            {
                // Case signalType is not valid
                return;
            }

            // Calling Delegate
            if (this.signalReceivalEventMap[signalType] != null)
            {
                string imageData = decodedDict["imageData"];
                this.signalReceivalEventMap[signalType](imageData, fromIP);
            }

            return;
        }

        /// <summary>
        /// Registers Handler to a given Signal type.
        /// Handler will be be called when such a Signal type arrives.
        /// </summary>
        /// <param name="signal">Signal for which notification has to be received</param>
        /// <param name="receivalHandler">Handler that has to be called on signal. This is of signature "void Handler(string data, IPAddress fromIP)"</param>
        /// <returns>Returns 'true' if registering successful. Otherwise returns 'false'.</returns>
        public bool SubscribeForSignalReceival(Signal signal, SignalHandler receivalHandler)
        {
            try
            {
                // Protecting Dictionary from concurrent calls
                lock (this.signalReceivalEventMap)
                {
                    this.signalReceivalEventMap[signal] += receivalHandler;
                }

                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}
