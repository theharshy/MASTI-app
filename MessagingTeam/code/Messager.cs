//-----------------------------------------------------------------------
// <author> 
//     Rahul Dhawan
// </author>
//
// <date> 
//     11-oct-2018 
// </date>
// 
// <reviewer> 
//     Aryan Raj
// </reviewer>
// 
// <copyright file="ICommunication.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      The following file contain the callbackfn used by the communicator class.
// </summary>
//-----------------------------------------------------------------------

namespace Messenger
{
    using System;
    using System.Net;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static Messenger.Handler;
    using System.Text;
    using System.Threading.Tasks;
    using Persistence;
    using Masti.Schema;
    using Masti.Networking;
    using System.Net.Sockets;



    /// <summary>
    /// Represents a Messager class to send and recieve message.
    /// </summary>
    /*public interface IMessager
    {
        /// <summary>
        /// Function triggered when the communication will successful or fail in message delivery where message is the message send and the ipaddress is the ip from it come and stauscode is the status of the code.
        /// </summary>
        void StatusCallback(string message, string ipaddress, int statuscode);

        /// <summary>
        /// Function triggered when the communication will recived message and they want to transfer.
        /// </summary>
        void DataCallback(string message, string ipaddres);
    }*/



    public class Messager : IUXMessage
    {

        Handler.DataReceiverHandler DataReceiver;

        Handler.DataStatusHandlers StatusReciever;

        Handler.ConnectHandlers Connectifier;


        private ISchema schemaObj;

        private IPersistence persistObj;

        private ICommunication comm;

        public Messager(int port)
        {
            schemaObj = new MessageSchema();
            persistObj = new Persistence();
            comm =  CommunicationFactory.GetCommunicator(port);
        }
        public Messager(string ip,int port,string uname)
        {
            schemaObj = new MessageSchema();
            persistObj = new Persistence();
            comm = CommunicationFactory.GetCommunicator(ip,port);
            ///var time = DateTime.Now;
            ///string formattedTime = time.ToString("yyyy, MM, dd, hh, mm, ss");
            ///Connectifier = new Handler.ConnectHandlers(handler);


            ///SendMessage(uname, ip, formattedTime, "False");
        }


        public void DataCallback(string message, IPAddress ipaddres)
        {
            IDictionary<string, string> DecodedMsg;
            DecodedMsg = schemaObj.Decode(message, false);
            string fromIP = DecodedMsg["fromIP"];
            string PureMessage = DecodedMsg["Msg"];
            string toIP = DecodedMsg["toIP"];
            string dateTime = DecodedMsg["dateTime"];
            string flag = DecodedMsg["flag"];
            /// DateTime DateTime = Convert.ToDateTime(StrDateTime);
            if (String.Equals("True", flag))
                DataReceiver(PureMessage, toIP, fromIP, dateTime);
            else
                Connectifier(fromIP, PureMessage);

        }

        
        public void StatusCallback(string message, Masti.Networking.StatusCode statuscode)
        {
            IDictionary<string, string> DecodedMsg;
            DecodedMsg = schemaObj.Decode(message, false);
            string PureMessage = DecodedMsg["Msg"];
            Handler.StatusCode newStatus = (Handler.StatusCode)statuscode;
            StatusReciever(newStatus, PureMessage);
        }

        public void SendMessage(string message, string toIP, string dateTime)
        {
            string EncodeMsg;
            bool status;
            string fromIP="127.0.0.1";
            ///string frIP = fromIP.ToString();
            ///

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    fromIP= ip.ToString();
                }
            }


            IPAddress targetAddress = IPAddress.Parse(toIP);
            Dictionary<string, string> Msg=new Dictionary<string, string>();
            Msg["Msg"] = message;
            Msg["fromIP"] = fromIP;
            Msg["toIP"] = toIP;
            Msg["flag"] = "True";
            
            Msg["dateTime"] = dateTime;

            //added time stamp to message and using schemaObj encode the message
            EncodeMsg = schemaObj.Encode(Msg);

            //bool Send(string msg, ulong dataID, IPAddress targetIP, DataType type);
            //using Icommunicate send this encoded message to communication
            status = comm.Send(EncodeMsg, targetAddress, DataType.Message);
        }

        public void SubscribeToDataReceiver(DataReceiverHandler handler)
        {
            bool status;
            //throw new NotImplementedException();
            DataReceiver = new Handler.DataReceiverHandler(handler);
            status = comm.SubscribeForDataReceival(DataType.Message, DataCallback);
            //pass our callback functions using subscriber functions of communicator class
        }

        public void SubscribeToStatusReceiver(DataStatusHandlers handler)
        {
            bool status;
            
            //throw new NotImplementedException();
            StatusReciever = new Handler.DataStatusHandlers(handler);
            status = comm.SubscribeForDataStatus(DataType.Message,StatusCallback);
            //pass our callback functions using subscriber functions of communicator class
        }
        public void SubscribeToConnectifier(ConnectHandlers handler)
        {
            Connectifier = new Handler.ConnectHandlers(handler);
        }

        public string RetrieveMessage(int startSession, int endSession)
        {
            string path = @"C:\Users\log.txt";
            var retrivingMessage = new List<string>(persistObj.RetrieveSession(startSession, endSession));
            string concatMessage = String.Join(" ", retrivingMessage.ToArray());
            System.IO.File.WriteAllText(path, concatMessage);
            return path;
        }

        public void DeleteMessage(int startSession, int endSession)
        {
            persistObj.DeleteSession(startSession, endSession);
        }

        public void StoreMessage(Dictionary<string, string> currSession)
        {
            string concatMessage = "";
            foreach (KeyValuePair<string, string> messageKeyValuePair in currSession)
            {
                concatMessage += messageKeyValuePair.Value + " ";
            }
            persistObj.SaveSession(concatMessage);
        }

        public void Connectify(string uname, string toIP)
        {
            string EncodeMsg;
            bool status;
            string fromIP = "127.0.0.1";
            ///string frIP = fromIP.ToString();
            ///

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    fromIP = ip.ToString();
                }
            }


            IPAddress targetAddress = IPAddress.Parse(toIP);
            Dictionary<string, string> Msg = new Dictionary<string, string>();
            Msg["Msg"] = uname;
            Msg["fromIP"] = fromIP;
            Msg["toIP"] = toIP;
            Msg["flag"] = "False";
            var time = DateTime.Now;
            string formattedTime = time.ToString("yyyy, MM, dd, hh, mm, ss");
            Msg["dateTime"] = formattedTime;
            EncodeMsg = schemaObj.Encode(Msg);
            comm.Send(EncodeMsg, targetAddress, DataType.Message);


        }
    }
}