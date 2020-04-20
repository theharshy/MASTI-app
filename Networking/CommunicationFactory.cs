//-----------------------------------------------------------------------
// <author> 
//     Parth Patel
// </author>
//
// <date> 
//    29-10-2018
// </date>
// 
// <reviewer> 
//     Libin N George
// </reviewer>
// 
// <copyright file="CommunicationFactory.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      Implements singleton Communicator factory.
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
    /// Singleton Communicator Factory
    /// </summary>
    public static class CommunicationFactory
    {
        /// <summary>
        /// lock to make factory thread-safe
        /// </summary>
        private static readonly object Padlock = new object();

        /// <summary>
        /// Stores first communicator instance
        /// </summary>
        private static ICommunication communicator = null;

        /// <summary>
        /// #retries allowed if ack isn't received from recipient
        /// </summary>
        private static int maxRetries = 5;

        /// <summary>
        /// #seconds to wait before retrying to send data.
        /// </summary>
        private static int waitTimeForAcknowledge = 5;

        /// <summary>
        /// To get communicator instance for server side
        /// </summary>
        /// <param name="serverPort">port on which server will listen for requests</param>
        /// <returns>communicator instance</returns>
        public static ICommunication GetCommunicator(int serverPort)
        {   
            // thread-safe instance creation
            if (communicator == null)
            {
                lock (Padlock)
                {
                    if (communicator == null)
                    {
                        communicator = new Communication(serverPort, maxRetries, waitTimeForAcknowledge);
                    }
                }
            }

            return communicator;
        }

        /// <summary>
        /// To get communicator instance for client side
        /// </summary>
        /// <param name="serverIP">Server's IP</param>
        /// <param name="serverPort">port number where server is listening for client requests</param>     
        /// <returns>communicator instance</returns>
        public static ICommunication GetCommunicator(string serverIP, int serverPort)
        {
            // thread-safe instance creation
            if (communicator == null)
            {
                lock (Padlock)
                {
                    if (communicator == null)
                    {
                        communicator = new Communication(serverIP, serverPort, maxRetries, waitTimeForAcknowledge);
                    }
                }
            }

            return communicator;
        }
    }
}
