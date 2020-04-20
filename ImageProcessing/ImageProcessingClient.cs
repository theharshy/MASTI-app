//-----------------------------------------------------------------------
// <author> 
//     Ravindra kumar
// </author>
//
// <date> 
//     28-10-2018 
// </date>
// 
// <reviewer> 
//     Suman Panda 
// </reviewer>
// 
// <copyright file="ImageProcessingClient.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This class captures the screen and sends it to server.
//      This class is invokes when a signal of start/resend
//      screen sharing is received.
// </summary>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using IitPkd.SchemaTeam;

    /// <summary>
    /// Client-side image processing module.
    /// </summary>
    public class ImageProcessingClient : IImageProcessingClient, IDisposable
    {
        /// <summary>
        /// Refers to the Schema class object.
        /// </summary>
        private ISchema schemaObject;

        /// <summary>
        /// Refers to the compression class object;
        /// </summary>
        private Compression compressionObject;

        /// <summary>
        /// Refers to the ImageCommunication class object;
        /// </summary>
        private ImageCommunication imageCommunicationObject;

        /// <summary>
        /// Refers a timer for repeated capturing and sending of captured screen.
        /// </summary>
        private System.Threading.Timer timer;

        /// <summary>
        /// Time interval between two screen captures.
        /// </summary>
        private uint timeInterval = 3000;

        /// <summary>
        /// Initializes a new instance of the ImageProcessingClient class.
        /// </summary>
        /// <param name="schema">Takes the schema class object to encode the Image.</param>
        /// <param name="imageCommunication">ImageCommunication class object to subscribe for different events.</param>
        /// <param name="compression">Takes the Compression class object to compress the image.</param>
        public ImageProcessingClient(ISchema schema, ImageCommunication imageCommunication, Compression compression)
        {
            if (schema == null || compression == null || imageCommunication == null)
            {
                this.ErrorFlag = true;
                return;
            }

            this.schemaObject = schema;
            this.compressionObject = compression;
            this.imageCommunicationObject = imageCommunication;
            this.ErrorFlag = false;
            this.imageCommunicationObject.SubscribeForSignalReceival(Signal.Start, this.StartSharingEventHandler);
            this.imageCommunicationObject.SubscribeForSignalReceival(Signal.Stop, this.StopSharingEventHandler);
            this.imageCommunicationObject.SubscribeForSignalReceival(Signal.Resend, this.ResendEventHandler);
        }

        /// <summary>
        /// Gets or sets the Data received from the publisher.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the IP Address from which the request came.
        /// </summary>
        public IPAddress FromIP { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a error in the program or not.
        /// </summary>
        public bool ErrorFlag { get; private set; }

        /// <summary>
        /// Gets or sets the time interval between two images.
        /// </summary>
        public uint TimeInterval
        {
            get
            {
                return this.timeInterval;
            }

            set
            {
                this.timeInterval = value;
            }
        }
 
        /// <summary>
        /// To start the image sharing from client side. It will capture the screen and send it to Compression class to compress it
        /// then it pass it to Schema class to encode it and finally ask SignalImageModule to publish it to Networking team.
        /// </summary>
        /// <param name="data">stores the data (if any) received from server side.</param>
        /// <param name="toIP">stores the IP address whom to send the captured image.</param>
        public void StartSharingEventHandler(string data, IPAddress toIP)
        {
            this.TimeInterval = 3000;
            this.Data = data;
            this.FromIP = toIP;

            // Capturing the screen.
            Bitmap capturedScreen = this.CaptureScreen();

            // Setting parameters to pass to Compression class.
            this.compressionObject.CreatePrevBmp = true;
            Dictionary<int, Bitmap> compressedImage = this.compressionObject.Compress(capturedScreen, false);
            this.compressionObject.CreatePrevBmp = false;
            string serializedCompressedDict = this.compressionObject.BmpDictToString(compressedImage);

            // Creating a dictionary of <string,string> type.
            Dictionary<string, string> dictCompressedImage = new Dictionary<string, string>();
            dictCompressedImage["implementDiff"] = "true";
            dictCompressedImage["imageDictionary"] = serializedCompressedDict;

            // Calling the Schema class to encode the dictionary.
            string encodedString = this.schemaObject.Encode(dictCompressedImage);
            int status = this.imageCommunicationObject.SignalImageModule(this.FromIP, Signal.Image, encodedString);
            this.timer = new System.Threading.Timer(new TimerCallback(this.TimerCallBack), null, this.TimeInterval, this.TimeInterval);
            Console.ReadLine();
            return;
        }

        /// <summary>
        /// To stop the image sharing by disposing off the timer.
        /// </summary>
        /// <param name="data">stores the data (if any) received from server side.</param>
        /// <param name="toIP">stores the IP address whom to send the captured image.</param>
        public void StopSharingEventHandler(string data, IPAddress toIP)
        {
            this.timer.Dispose();
            return;
        }

        /// <summary>
        /// To resend the image which is lost while sending to server.
        /// </summary>
        /// <param name="data">stores the data (if any) received from server side.</param>
        /// <param name="toIP">stores the IP address whom to send the captured image.</param>
        public void ResendEventHandler(string data, IPAddress toIP)
        {
            this.Data = data;
            this.FromIP = toIP;
            Bitmap capturedScreen = this.CaptureScreen();
            Dictionary<int, Bitmap> compressedImage = this.compressionObject.Compress(capturedScreen, true);
            string serializedCompressedDict = this.compressionObject.BmpDictToString(compressedImage);
            Dictionary<string, string> dictCompressedImage = new Dictionary<string, string>();
            dictCompressedImage["implementDiff"] = "true";
            dictCompressedImage["imageDictionary"] = serializedCompressedDict;
            string encodedString = this.schemaObject.Encode(dictCompressedImage);
            int status = this.imageCommunicationObject.SignalImageModule(toIP, Signal.Image, encodedString);
            return;
        }

        /// <summary>
        /// Public implementation of Dispose pattern.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the timer.
        /// </summary>
        /// <param name="disposing">Pass true to dispose the disposable object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.timer.Dispose();
            }
        }

        /// <summary>
        /// This method is called after every particular interval of time to capture the screen and send it to server.
        /// </summary>
        /// <param name="s">A object of timer</param>
        private void TimerCallBack(object s)
        {
            Bitmap capturedScreen = this.CaptureScreen();
            Dictionary<int, Bitmap> compressedImage = this.compressionObject.Compress(capturedScreen, true);
            string serializedCompressedDict = this.compressionObject.BmpDictToString(compressedImage);
            Dictionary<string, string> dictCompressedImage = new Dictionary<string, string>();
            dictCompressedImage["implementDiff"] = "true";
            dictCompressedImage["imageDictionary"] = serializedCompressedDict;
            string encodedString = this.schemaObject.Encode(dictCompressedImage);
            int status = this.imageCommunicationObject.SignalImageModule(this.FromIP, Signal.Image, encodedString);
        }

        /// <summary>
        /// To capture the screen.
        /// </summary>
        /// <returns>Bitmap of the captured screen.</returns>
        private Bitmap CaptureScreen()
        {
            // Declare a bitmap of size of the screen size.
            Bitmap bitmapCapturedScreen = new Bitmap(
                                                Screen.PrimaryScreen.Bounds.Width,
                                                Screen.PrimaryScreen.Bounds.Height,
                                                PixelFormat.Format32bppArgb);

            // Creating a New Graphics Object
            Graphics graphicsCapturedScreen = Graphics.FromImage(bitmapCapturedScreen);
            try
            {
                // Copying Image from The Screen
                graphicsCapturedScreen.CopyFromScreen(
                                                    Screen.PrimaryScreen.Bounds.Width,
                                                    Screen.PrimaryScreen.Bounds.Height,
                                                    0,
                                                    0,
                                                    Screen.PrimaryScreen.Bounds.Size,
                                                    CopyPixelOperation.SourceCopy);
            }
            catch (Exception exception)
            {
                this.ErrorFlag = true;
                MessageBox.Show(exception.Message);
            }

            return bitmapCapturedScreen;
        }
    }
}
