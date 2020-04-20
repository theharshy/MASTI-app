// -----------------------------------------------------------------------
// <author> 
//      Sooraj Tom
// </author>
//
// <date> 
//      28-10-2018 
// </date>
// 
// <reviewer>
//      Axel James
// </reviewer>
//
// <copyright file="ImageProcessing.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
//
// <summary>
//      This file is the main class for Image Processing module.
//      Calls to other classes originate from here.
// </summary>
// -----------------------------------------------------------------------
namespace Masti.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Net;
    using IitPkd.SchemaTeam;
    using Networking;

    /// <summary>
    /// This is the base class for screen sharing. This class is to be instantiated by the UI team in both server and receiver.
    /// </summary>
    public class ImageProcessing : IImageProcessing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProcessing"/> class.
        /// The constructor for Image 
        /// </summary>
        public ImageProcessing()
        {
            this.Communication = GetCommunicator();
            this.ImageSchema = GetSchema();
            this.ImageCompression = new Compression();
            this.ImageCommunicator = new ImageCommunication(this.Communication, this.ImageSchema);
            this.ImageRecieve = new ReceiveImage(this.ImageSchema, this.ImageCompression, this.ImageCommunicator);
        }

        /// <summary>
        /// Gets or sets the Communication object for the sending and recieving object
        /// </summary>
        public ICommunication Communication { get; set; }

        /// <summary>
        /// Gets or sets ImageSchema which is an object to pack and unpack data in proper format.
        /// </summary>
        public ISchema ImageSchema { get; set; }

        /// <summary>
        /// Gets or sets the ImageCompression object for compression and decompression of image
        /// </summary>
        public Compression ImageCompression { get; set; }

        /// <summary>
        /// Gets or sets the ImageRecieve class
        /// </summary>
        public ReceiveImage ImageRecieve { get; set; }

        /// <summary>
        /// Gets or sets the Image Communicator class
        /// </summary>
        public ImageCommunication ImageCommunicator { get; set; }

        /// <summary>
        /// This method starts the screen sharing process.
        /// It is called by the server (professor) with the IP address of the client (student).
        /// </summary>
        /// <param name="clientIP">This parameter is the IP address of the target client computer</param>
        /// <returns>Ruturns the status</returns>
        public bool GetSharedScreen(string clientIP)
        {
            IPAddress clientIPAddress = IPAddress.Parse(clientIP);
            return this.ImageCommunicator.SignalImageModule(clientIPAddress, Signal.Start);
        }

        /// <summary>
        /// StopSharedScreen method called by UI to stop screen sharing
        /// <returns>Ruturns the status</returns>
        /// </summary>
        /// <param name="clientIP"></param>
        public bool StopSharedScreen(string clientIP)
        {
            var clientIPAddress = IPAddress.Parse(clientIP);
            return this.ImageCommunicator.SignalImageModule(clientIPAddress, Signal.Stop);
        }

        /// <summary>
        /// This method registers the image updater for UI.
        /// </summary>
        /// <param name="imageReceivedNotifyHandler">The updater function from UI that updates the screen shared</param>
        public void RegisterImageUpdateHandler(EventHandler<ImageEventArgs> imageReceivedNotifyHandler)
        {
            this.ImageRecieve.RegisterListenerImageDecoded(imageReceivedNotifyHandler);
        }

        /// <summary>
        /// this method is to be implemented while Schema team gives their factory method to create their object
        /// </summary>
        /// <returns>return a imageSchema type object</returns>
        private static ImageSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// this method is to be modified when networking team would provide their factory method.
        /// </summary>
        /// <returns>returna an object of ICommunication interface</returns>
        private static ICommunication GetCommunicator()
        {
            throw new NotImplementedException();
        }
    }
}
