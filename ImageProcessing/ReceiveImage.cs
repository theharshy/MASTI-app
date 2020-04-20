//-----------------------------------------------------------------------
// <author>
//      Suman Saurav Panda
// </author>
// <reviewer>
//      Ravindra Kumar, Axel James
// </reviewer>
// <date>
//      28-Oct-2018
// </date>
// <summary>
//      Server side receive image class implementation 
// </summary>
// <copyright file="ReceiveImage.cs" company="B'15, IIT Palakkad">
//     This project is licensed under GNU General Public License v3. (https://fsf.org) 
// </copyright>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using IitPkd.SchemaTeam;
    using Networking;

    /// <summary>
    /// This class creates instance which takes rawdata given by the networking module. This class is the publisher for
    /// the image to the UI module. It checks the integrity received data and then either publishes to UI module upon succes
    /// otherwise notify error to its upper module.
    /// </summary>
    public class ReceiveImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveImage"/> class. 
        /// </summary>
        /// <param name="schema">takes the object schema to unpack the rawstring received from networking</param>
        /// <param name="compression">compression takes the compression object to use decompress to get back the pixeliamge</param>
        /// <param name="communication">this is the signalling class object of image module which would deliver
        /// the raw string from network</param>
        public ReceiveImage(ISchema schema, ICompression compression, ImageCommunication communication)
        {
            this.Schema = schema;
            this.Compression = compression;
            communication.SubscribeForSignalReceival(Signal.Image, this.OnRecievingimage);
            this.ErrorFlag = false;
        }

        /// <summary>
        /// event to publish the  Image decoded to its subscriber
        /// </summary>
        private event EventHandler<ImageEventArgs> ImageDecoded = null;

        /// <summary>
        /// Event to publish Error on image decoding to its subscriber
        /// </summary>
        private event EventHandler<ErrorEventArgs> ErrorImageDecoded = null;

        /// <summary>
        /// Gets or sets the string received from ImageCommunication
        /// </summary>
        public string RawString { get; set; }

        /// <summary>
        /// Gets or sets ip address of client
        /// </summary>
        public IPAddress ClientIP { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether image parsed correctly or not.
        /// </summary>
        public bool ErrorFlag { get; set; }

        /// <summary>
        /// Gets or sets schema obj for unpacking method; 
        /// </summary>
        private ISchema Schema { get; set; }

        /// <summary>
        /// Gets or sets the compression obj for decompression method;
        /// </summary>
        private ICompression Compression { get; set; }

        /// <summary>
        /// Gets or sets imageBitMap which would be given to UI 
        /// </summary>
        private Bitmap ImageBitMap { get; set; }

        /// <summary>
        /// method is used to register event handler by the subscriber for the iamgeReceived event
        /// </summary>
        /// <param name="onImageDecoded">delegate function which would be given by the subscriber(UI module)
        /// for getting the image from ImageProcessing module</param>
        public void RegisterListenerImageDecoded(EventHandler<ImageEventArgs> onImageDecoded)
        {
            this.ImageDecoded += onImageDecoded;
            return;
        }

        /// <summary>
        /// method is used to register event handler for the error notification
        /// </summary>
        /// <param name="onErrorImageDecoded">delegate function for the event</param>
        public void RegisterListenerErrorImageDecoded(EventHandler<ErrorEventArgs> onErrorImageDecoded)
        {
            this.ErrorImageDecoded += onErrorImageDecoded;
            return;
        }

        /// <summary>
        /// this is event handler to signaling class Upon getting data from networking module.
        /// After parsing the rawData if some error found it sets the ErrorFlag of the object 
        /// so that the error can be propagated to the previous modules.
        /// </summary>
        /// <param name="rawData">data from networking module which would then be processed to send to the UI module</param>
        /// <param name="ip">ip address of the client required by the UI team</param>
        public void OnRecievingimage(string rawData, IPAddress ip)
        {
            this.RawString = rawData;
            this.ClientIP = ip;
            if (!this.GetBitMap())
            {
                this.OnErrorImageDecoded();
            }

            return;
        }

        /// <summary>
        /// It converts the rawstring received form networking to actual bitmap of the image
        /// </summary>
        /// <returns>true if it successfully send the image to UI module
        /// otherwise if some parsing error happens while unpacking using shema method and decompression
        /// it returns false.</returns>
        public bool GetBitMap()
        {
            IDictionary<string, string> unpackedDicitionary = this.Schema.Decode(this.RawString, false);

            if (!CheckReceivedFormat(unpackedDicitionary))
            {
                return false;
            }

            bool diffImplementationFlag = string.Equals(unpackedDicitionary["implementDiff"], "true", StringComparison.Ordinal);
            string serializedImageDictionary = unpackedDicitionary["imageDictionary"];
            Dictionary<int, Bitmap> imageMap = DeserializeImageDictionary(serializedImageDictionary);

            this.ImageBitMap = this.Compression.Decompress(imageMap, diffImplementationFlag);

            if (this.ImageBitMap == null)
            {
                return false;
            }
                        
            this.OnImageDecoded();
            return true;
        }

        /// <summary>
        /// raises the event for error
        /// </summary>
        protected virtual void OnErrorImageDecoded()
        {
            ErrorEventArgs errorEventArgs = new ErrorEventArgs() { ErrorMessage = "Error" };
            this.ErrorImageDecoded?.Invoke(this, errorEventArgs);
        }

        /// <summary>
        /// raises the event to pass the encoded image to its subscriber
        /// </summary>
        protected virtual void OnImageDecoded()
        {
            ImageEventArgs imageEventArgs = new ImageEventArgs() { ImageBitMap = this.ImageBitMap };
            this.ImageDecoded?.Invoke(this, imageEventArgs);
        }

        /// <summary>
        /// it takes deserialized string obtained from schema and converts it back to bitmap dictionary
        /// which is passed to decompress class for getting the bitmap of image.
        /// </summary>
        /// <param name="serializedString">serialized string representation of bitmap dictionary</param>
        /// <returns>returns the bitmap dictionary</returns>
        private static Dictionary<int, Bitmap> DeserializeImageDictionary(string serializedString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// helper function to check if the format after decoding by schema team matches the correct format
        /// </summary>
        /// <param name="unpackedDicitionary"> this parameter is obtained after decoding the rawData by schema team</param>
        /// <returns>true if the dictionary format matches otherwise false</returns>
        private static bool CheckReceivedFormat(IDictionary<string, string> unpackedDicitionary)
        {
            if (unpackedDicitionary.ContainsKey("implementDiff") && unpackedDicitionary.ContainsKey("imageDictionary"))
            {
                return true;
            }

            return false;
        }
    }
}
