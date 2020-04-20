// -----------------------------------------------------------------------
// <author> 
//      Libin N George
// </author>
//
// <date> 
//      03-11-2018 
// </date>
// 
// <reviewer>
//      Ayush Mittal
// </reviewer>
//
// <copyright file="DataRecevialNotifierTest.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
//
// <summary>
//      This file contains Tests for Data Receiver Notifier Component 
//      This file is a part of Networking Module Unit Testing
// </summary>
// -----------------------------------------------------------------------

namespace Masti.Networking.UnitTests
{
    using System.Net;
    using Masti.Networking.Stubs;
    using Masti.QualityAssurance;

    /// <summary>
    /// Unit Test for Data on Receive Notifier Component.
    /// </summary>
    public class DataRecevialNotifierTest : ITest
    {
        /// <summary>
        /// Holds communication Instance
        /// </summary>
        private Communication communication;
        
        /// <summary>
        /// status to check that delegate is called while receiving Image related Data.  
        /// </summary>
        private bool imagedataRecevied;

        /// <summary>
        /// status to check that delegate is called while receiving Messaging related Data. 
        /// </summary>
        private bool messagedataRecevied;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRecevialNotifierTest" /> class.
        /// </summary>
        /// <param name="logger">Logger object used for logging during test</param>
        public DataRecevialNotifierTest(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets or sets Logger instance.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Function for running the unit tests
        /// </summary>
        /// <returns>true on success and false on failure</returns>
        public bool Run()
        {
            // setting communication to Communication class instance. 
            this.communication = new Communication(8000, 3, 1);

            // Setting Stub Schema for this test.
            this.communication.Schema = new StubSchemaForNetworking();

            // boolean capturing the status of subscription for ImageProcessing Module
            bool imageModuleSubscription = false;

            // boolean capturing the status of subscription for Messaging Module
            bool messageModuleSubscription = false;

            // boolean capturing the status of successful type recognition and calling delegate for ImageProcessing Module Data packet
            bool imageDataReceiveNotified = false;

            // boolean capturing the status of successful type recognition and calling delegate for Messageing Module Data packet
            bool messageDataReceiveNotified = false;

            // boolean capturing the status of successful type recognition for Unknown Module Data packet
            bool invalidDataReceiveNotified;

            this.imagedataRecevied = false;
            this.messagedataRecevied = false;
            imageModuleSubscription = this.SubscribeForImageDataTest();
            messageModuleSubscription = this.SubscribeForMessageDataTest();
            if (!(imageModuleSubscription & messageModuleSubscription))
            {
                return false;
            }

            invalidDataReceiveNotified = this.communication.DataReceiveNotifier("UNKNOWN Packet", IPAddress.Parse("127.0.0.2"));
            if (!(invalidDataReceiveNotified | this.imagedataRecevied | this.messagedataRecevied))
            {
                this.Logger.LogSuccess("Invalid type successfully detected");
            }
            else
            {
                this.Logger.LogError("Invalid type not detected");
                return false;
            }

            imageDataReceiveNotified = this.communication.DataReceiveNotifier("ImageProcessing Packet", IPAddress.Parse("127.0.0.1"));
            if (!imageDataReceiveNotified)
            {
                this.Logger.LogError("Notification for Image packet failed due unknown type");
                return false;
            }

            messageDataReceiveNotified = this.communication.DataReceiveNotifier("Messaging Packet", IPAddress.Parse("127.0.0.2"));
            if (!messageDataReceiveNotified)
            {
                this.Logger.LogError("Notification for Message packet failed due unknown type");
                return false;
            }

            return this.imagedataRecevied & this.messagedataRecevied;
        }

        /// <summary>
        /// Function used as event handler for receiving Image related package.
        /// </summary>
        /// <param name="data">Image related data received</param>
        /// <param name="ipAddress">IP address of machine from which data packet is received</param>
        public void ReceiveImageData(string data, IPAddress ipAddress)
        {
            this.Logger.LogSuccess("Data Recevied " + data + " from " + "IPAddress " + ipAddress.ToString() + "For Image Processing Module");
            this.imagedataRecevied = true;
        }

        /// <summary>
        /// Function used as event handler for receiving Message related package.
        /// </summary>
        /// <param name="data">Message related data received</param>
        /// <param name="ipAddress">IP Address of machine from which data packet is received</param>
        public void ReceiveMessageData(string data, IPAddress ipAddress)
        {
            this.Logger.LogSuccess("Data Recevied " + data + " from " + "IPAddress " + ipAddress.ToString() + "For Messaging Module");
            this.messagedataRecevied = true;
        }

        /// <summary>
        /// Tests the subscription for Image Processing Module
        /// </summary>
        /// <returns>true on success</returns>
        public bool SubscribeForImageDataTest()
        {
            bool subscribeStatus = false;
            subscribeStatus = this.communication.SubscribeForDataReceival(DataType.ImageSharing, this.ReceiveImageData);
            if (subscribeStatus)
            {
                this.Logger.LogSuccess("Subscription for Receiving Data for " + DataType.ImageSharing.ToString() + " completed successfully.");
            }
            else
            {
                this.Logger.LogError("Subscription for Receiving Data for " + DataType.ImageSharing.ToString() + " failed.");
            }

            return subscribeStatus;
        }

        /// <summary>
        /// Tests the subscription for Messaging Module
        /// </summary>
        /// <returns>true on success</returns>
        public bool SubscribeForMessageDataTest()
        {
            bool subscribeStatus = false;
            subscribeStatus = this.communication.SubscribeForDataReceival(DataType.Message, this.ReceiveMessageData);
            if (subscribeStatus)
            {
                this.Logger.LogSuccess("Subscription for Receiving Data for " + DataType.Message.ToString() + " completed successfully.");
            }
            else
            {
                this.Logger.LogSuccess("Subscription for Receiving Data for " + DataType.Message.ToString() + " failed.");
            }

            return subscribeStatus;
        }
    }
}
