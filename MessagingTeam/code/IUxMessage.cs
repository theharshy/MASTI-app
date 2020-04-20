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
// <copyright file="IUxMessage.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This file implements interfaces used by UX team .
// </summary>
//-----------------------------------------------------------------------

namespace Messenger
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using static Messenger.Handler;
    using System.Net;

    /// <summary>
    /// This interface is used by UX to send, receive, delete and retrieve messages
    /// </summary>
    public interface IUXMessage
    {
        /// <summary>
        /// This is used by UX module to send message and IP and port
        /// </summary>
        /// <param name="message">text message</param>
        /// <param name="toIP"> IP where message is to be sent</param>
        /// <param name="toPort">port no</param>
        /// <param name="dateTime">date and  time at which message was sent</param>
        void SendMessage(string message, string toIP, string dateTime);

        /// <summary>
        /// This is subscriber function which will be used by UX module to subscribe their callbacks for receiving data
        /// </summary>
        /// <param name="handler">delegates to run callbacks</param>
        void SubscribeToDataReceiver(DataReceiverHandler handler);

        /// <summary>
        /// This is subscriber function which will be used by UX module to subscribe their callbacks for getting status
        /// </summary>
        /// <param name="handler">delegates to run callbacks</param>
        void SubscribeToStatusReceiver(DataStatusHandlers handler);

        /// <summary>
        /// This is interface to retrieve message from database
        /// </summary>
        /// <param name="startSession">start session</param>
        /// <param name="endSession">end session</param>
        /// <returns>packed string stored in database</returns>
        string RetrieveMessage(int startSession, int endSession);

        /// <summary>
        /// This is interface to delete message of database for a given time interval
        /// </summary>
        /// <param name="startSession">start session</param>
        /// <param name="endSession">end session</param>
        void DeleteMessage(int startSession, int endSession);

        /// <summary>
        /// This is for storing message  in database
        /// </summary>
        /// <param name="currSession">dictionary to store message of a particular session</param>
        void StoreMessage(Dictionary<string, string> currSession);




        void Connectify(string uname, string toIP);




        void SubscribeToConnectifier(ConnectHandlers handler);
    }
}