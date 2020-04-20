//-----------------------------------------------------------------------
// <author> 
//     Aryan Raj 
// </author>
//
// <date> 
//     12-10-2018 
// </date>
// 
// <reviewer> 
//     Aryan Raj 
// </reviewer>
// 
// <copyright file="Handler.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This file implements delegates .
// </summary>
//-----------------------------------------------------------------------

namespace Messenger
{
    using System;

    /// <summary>
    /// Handler class
    /// </summary>
    public static class Handler

    {
        public enum StatusCode
        {
            /// <summary>
            /// Send Success 
            /// </summary>
            Success,

            /// <summary>
            /// Send Failure
            /// </summary>
            Failure
        }
        /// <summary>
        /// This is data receiver handler to which data receiving callbacks from UX will be subscribed 
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="toIP">IP of where messages is to be sent</param>
        /// <param name="fromIP">IP of sender</param>
        /// <param name="dateTime">date and time of message</param>
        public delegate void DataReceiverHandler(string message, string toIP, string fromIP, string dateTime);

        /// <summary>
        /// This is status receiver handler to which status receiving callbacks from UX will be subscribed
        /// </summary>
        /// <param name="status">1 or 0 for success or failure</param>
        /// <param name="message">text message</param>
        public delegate void DataStatusHandlers(StatusCode status, string message);

        public delegate void ConnectHandlers(string fromIP,string userName);

    }
}
