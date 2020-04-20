// -----------------------------------------------------------------------
// <author> 
//      Ayush Mittal (29ayush@gmail.com) 
// </author>
//
// <date> 
//      12-10-2018 
// </date>
// 
// <reviewer>
// Rajat Sharma
// </reviewer>
//
// <copyright file="DataOutgoing.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
//      You are allowed to use the file and/or redistribute/modify as long as you preserve this copyright header and author tag.
// </copyright>
//
// <summary>
//      This File contains the Data Outgoing Component.
//      It checks for data in queue by polling.If queue has a message, send it to its designation.
//      This file is a part of Networking Module.
// </summary>
// -----------------------------------------------------------------------

namespace Masti.Networking
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Masti.Schema;

    /// <summary>
    /// Following partial class implements DataOutGoing Component - 
    /// Interface Function Send 
    /// Function to send data from the queue over the network.
    /// Constructors to start this and receiving component.
    /// </summary>
    public partial class Communication : ICommunication, IDisposable
    {
        /// <summary>
        /// lockObjects,lockStatus,ConnectedClients are HashTables accessed by multiple threads, 
        /// any addition/deletion to these tables is thus made thread safe by using this lock.
        /// </summary>
        private readonly object mainLock = new object();

        /// <summary>
        /// acknowledgeStatus is a HashTable accessed by multiple threads, 
        /// any addition/deletion to this tables is thus made thread safe by using this lock.
        /// </summary>
        private readonly object ackLock = new object();

        /// <summary>
        /// It stores IP of professor and can be accessed by calling the get function.
        /// On professor's machine it's basically the local IP.
        /// </summary>
        private string ipAddress;

        /// <summary>
        /// It stores Port at which professor is listening and can be accessed by calling the get function.
        /// /// </summary>
        private int port;

        /// <summary>
        /// When the module is instantiated as student, it stores whether the socket to professor is working or not.
        /// The purpose it serves is that sometimes the message might fail (due to waitTimeForAcknowledgement being less.) 
        /// But without any fault of network, In that case the module don't need to be restarted,but wait for some time to see
        /// If problem persists.
        /// When the module is instantiated as professor, it stores whether the professor is listening or not, again it helps in debugging purposes.
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// Stores the listening socket for professor, nothing for student.
        /// </summary>
        private TcpListener serverSocket;

        /// <summary>
        /// No of times - 1  to `re`send a message if acknowledgement is not received.
        /// </summary>
        private int maxRetries;

        /// <summary>
        /// Time to wait for recipient to send acknowledgement in milliseconds.
        /// </summary>
        private int waitTimeForAcknowledge;

        /// <summary>
        /// Local Variable to Store if this module is instantiated as professor or student.
        /// </summary>
        private bool isStudent = false;

        /// <summary>
        /// Can be used when testing using threads to identify log corresponding to various threads.
        /// </summary>
        private string testIP = string.Empty;

        /// <summary>
        /// Event used to manage wait in SendFromQueue function.
        /// </summary>
        private AutoResetEvent queueEvent;

        /// <summary>
        /// Event used to manage stop the MainThread.
        /// </summary>
        private ManualResetEvent stopEvent;

        /// <summary>
        /// Objective :: Keep a reference to existing connection(socket) corresponding to an IP:Port.
        /// Key       :: IPEndPoint as string.
        /// Value     :: Socket
        /// </summary>
        private Hashtable connectedClients;

        /// <summary>
        /// Objective :: When a message is sent, it is added to this table, It tracks the messages whose acknowledgement is currently not processed.
        /// Key       :: MD5Hash of message + (not hashed)IPEndPoint. 
        /// Value     :: Bool False represents acknowledgement hasn't been received, true represents it has been received.
        /// Note      :: All Messages are supposed to be unique, since schema encodes the timestamps in message.
        /// </summary>
        private Hashtable acknowledgeStatus;

        /// <summary>
        /// Objective :: Since multiple threads are spawned to send messages (Only one queue) over the network with same source, to prevent different threads to send overlapping 
        ///           :: messages locks are used so that only one thread sends the message at a time.
        /// Key       :: IPEndPoint as string. 
        /// Value     :: Object to be used as reference for locks.
        /// </summary>
        private Hashtable lockObjects;

        /// <summary>
        /// Objective :: We use a double checked lock mechanism, it stores true/false check corresponding to the locks.
        /// Key       :: IPEndPoint as string. 
        /// Value     :: Bool to be used as check for locks.
        /// </summary>
        private Hashtable lockStatus;

        /// <summary>
        /// Reference to thread which runs the a function to pop data from queue and process it.
        /// </summary>
        private Thread sendingThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Communication"/> class.
        /// This constructor is used to initialize common data.
        /// </summary>
        public Communication()
        {
            this.connectedClients = new Hashtable(); // IP , Socket
            this.acknowledgeStatus = new Hashtable(); // MESSAGEEHASH + IP , AutoResetEvent
            this.lockStatus = new Hashtable(); // IP , Bool
            this.lockObjects = new Hashtable(); // IP, Obj
            this.schema = new ImageSchema();
            this.queueEvent = new AutoResetEvent(false);
            this.stopEvent = new ManualResetEvent(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Communication"/> class.
        /// This constructor is supposed to be used when instantiating the module for a Professor.
        /// </summary>
        /// <param name="port">Port to start the listening server on</param>
        /// <param name="maxRetries">No of times to send a message if acknowledgement is not received.</param>
        /// <param name="waitTimeForAcknowledge">Time to wait for recipient to send acknowledgement in seconds.</param>
        /// <param name="testIP"> For testing purposes.Bind to specific loop back address.(Read Test File For More) </param>
        public Communication(int port, int maxRetries, int waitTimeForAcknowledge, string testIP = "") : this()
        {
            Diagnostics.LogInfo("Communication Object Created On Professor Machine" + testIP);
            this.port = port;
            this.maxRetries = maxRetries;
            this.waitTimeForAcknowledge = waitTimeForAcknowledge * 1000;
            this.testIP = testIP;

            this.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Communication"/> class.
        /// This constructor is supposed to be used when instantiating the module for a Student.
        /// </summary>
        /// <param name="ipAddress">IP address of professor's machine.</param>
        /// <param name="port">Port of professor's machine</param>
        /// <param name="maxRetries">No of times to resend a message if acknowledgement is not received.</param>
        /// <param name="waitTimeForAcknowledge">Time to wait for recipient to send acknowledgement.</param>
        /// <param name="testIP"> For testing purposes.Bind to specific loop back address.(Read Test File For More). </param>
        public Communication(string ipAddress, int port, int maxRetries = 0, int waitTimeForAcknowledge = 0, string testIP = "") : this()
        {
            Diagnostics.LogInfo("Communication Object Created On Student Machine" + testIP);
            this.ipAddress = ipAddress;
            this.isStudent = true;
            this.port = port;
            this.maxRetries = maxRetries;
            this.waitTimeForAcknowledge = waitTimeForAcknowledge * 1000;
            this.testIP = testIP;

            this.Start();
        }
        
        /// <summary>
        /// Starts the receiving and sending part of the module.
        /// </summary>
        /// <returns> true if there were no problems with initialization(like port is free.) otherwise false. </returns>
        public bool Start()
        {
            if (this.isRunning)
            {
                // This function shouldn't be called when the module is already running.
                throw new InvalidOperationException();
            }

            if (!this.isStudent)
            {
                try
                {
                    this.StartReceiver().Start();
                }
                catch
                {
                    Diagnostics.LogInfo($"Tried Initializing on Port {this.port} which wasn't free");
                    throw;
                }
            }

            this.sendingThread = new Thread(this.SendFromQueue);

            // When running multiple threads on same device (for testing) or any other purpose, it serves well for us
            // to be able to distinguish different threads(objects).
            if (this.isStudent)
            {
                this.sendingThread.Name = "MainSendThread : " + this.testIP;
            }
            else
            {
                this.sendingThread.Name = "MainSendThread : Professor";
                this.isRunning = true;
            }

            this.sendingThread.Start();
            Diagnostics.LogInfo(Helper.ContextLogger($"Module Started Properly {this.ipAddress} {this.port}", 0));
            return true;
        }

        /// <summary>
        /// Stops all the sockets and main threads.
        /// </summary>
        public void Stop()
        {
            this.isRunning = false;
            if (this.serverSocket != null)
            {
                this.serverSocket.Stop();
            }

            // Close all clients sockets to terminate their threads
            foreach (Socket client in this.connectedClients.Values)
            {
                client.Close();
            }

            this.stopEvent.Reset();
            this.queueEvent.Set();
        }
                
        /// <summary>
        /// This method is used to dispose queueEvent,stopEvent by implementing IDisposable
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose Overload following the dispose pattern.
        /// </summary>
        /// <param name="disposing">true if called by Disposes false if called by finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                this.queueEvent.Close();
                this.stopEvent.Close();
            }

            // free native resources
        }

        /// <summary>
        /// Sends a given message by formatting it.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="retries">Tells which retry is this to send the message</param>
        /// <param name="socket">The socket to which send the data on</param>
        /// <param name="isAck">Tells if the message being sent is acknowledgment.</param>
        private void SendHelper(string message, int retries, Socket socket, int isAck)
        {
            // Create a new state object.
            ClientState currentState = new ClientState
            {
                Socket = socket
            };
            if (currentState.IPPort == "0.0.0.0")
            {
                Diagnostics.LogError(Helper.ContextLogger("Called with null socket, not supposed to happen", 1));
                throw new InvalidOperationException();
            }

            byte[] byteData = Encoding.ASCII.GetBytes(message);
            byte[] lengthBytes = Helper.GetBytes(byteData.Length);
            byte[] ackBits = new byte[] { (byte)isAck };

            // Every message sent over network has format (prefix = length + ackBit) + message bytes
            // length in prefix excludes the bytes used for length and reservedBits(ackBit)
            byte[] prefix = Helper.Combine(lengthBytes, ackBits);

            currentState.Retries = retries;
            currentState.DataSent = 0;
            currentState.Message = message;
            currentState.SetDataToSend(Helper.Combine(prefix, byteData));

            Diagnostics.LogInfo(Helper.ContextLogger("Waiting To Acquire Lock IP EndPoint" + Helper.GetEndPoint(socket), 7));

            // We maintain locks per socket so that more than one thread don't send at the same time on same socket, otherwise data would overlap.
            while (!this.GetLock(currentState.IPPort))
            {
            }

            Diagnostics.LogInfo(Helper.ContextLogger("Lock Acquired. Calling BeginSend EndPoint" + Helper.GetEndPoint(socket), 5));
            lock (this.ackLock)
            {
                this.acknowledgeStatus[Helper.MD5Hash(message) + currentState.IPPort] = new AutoResetEvent(false);
            }

            try
            {
                socket.BeginSend(currentState.DataToSend(), 0, currentState.DataToSend().Length, 0, new AsyncCallback(this.SendCallback), currentState);
            }
            catch
            {
                Diagnostics.LogError(Helper.ContextLogger("SocketError EndPoint :: " + Helper.GetEndPoint(currentState.Socket), 0));  // Socket Failed
                this.ResetClient(Helper.GetEndPointAddress(currentState.Socket), currentState.IPPort); // Remove any lock-objects,sockets,status bit associated to failed socket.
                lock (this.ackLock)
                {
                    this.acknowledgeStatus.Remove(Helper.MD5Hash(currentState.Message) + currentState.IPPort);
                }

                this.DataStatusNotify(currentState.Message, StatusCode.Failure);
                if (this.isStudent)
                {
                    this.isRunning = false;
                }
            }

            Diagnostics.LogInfo(Helper.ContextLogger("Returned EndPoint" + Helper.GetEndPoint(socket), 5));
        }

        /// <summary>
        /// Runs a infinity loop which keeps checking the queue at regular intervals,
        /// if there are any messages in the queue, it sends them.
        /// </summary>
        private void SendFromQueue()
        {
            Socket clientSocket;
            while (true)
            {
                // TryDequeue returns false if dequeue was unsuccessful 
                // If successful it returns true and stores result in currentRequest.
                if (!SendRequestQueue.TryDequeue(out SendRequest currentRequest))
                {
                    Diagnostics.LogInfo(Helper.ContextLogger("Queue Empty, sending the reset signal", 5));
                    this.queueEvent.WaitOne();
                    if (!this.stopEvent.WaitOne(10))
                    {
                        return;
                    }

                    continue;
                }

                // Below conditions results in false if there isn't any existing connection to target.
                // In case of professor we fail, In case of student we start a connection.
                if (this.connectedClients.ContainsKey(currentRequest.TargetIPAddress.ToString()))
                {
                    clientSocket = (Socket)this.connectedClients[currentRequest.TargetIPAddress.ToString()];
                    this.SendHelper(currentRequest.Data, 0, clientSocket, 0);
                }
                else
                {
                    Diagnostics.LogInfo(Helper.ContextLogger("Connection For EndPoint " + currentRequest.TargetIPAddress + "Not Found.", 5));
                    if (this.isStudent)
                    {
                        if (currentRequest.TargetIPAddress.ToString() != this.ipAddress)
                        {
                            Diagnostics.LogError(Helper.ContextLogger("The Professor Device IP Address Doesn't Match with Target IP Address.", 0));
                            throw new InvalidOperationException();
                        }

                        if (this.InitiateConnection())
                        {
                            Diagnostics.LogInfo(Helper.ContextLogger("Connection Initiated to Prof. at IP:" + this.ipAddress + " Port:" + this.port + "\n", 2));
                            this.SendHelper(currentRequest.Data, 0, (Socket)this.connectedClients[currentRequest.TargetIPAddress.ToString()], 0);
                            continue;
                        }
                    }

                    this.DataStatusNotify(currentRequest.Data, StatusCode.Failure); // failure callback
                }
            }
        }

        /// <summary>
        /// Callback for asynchronous send, if data is left to send, then send the remaining data, otherwise go into wait for acknowledgement state.
        /// </summary>
        /// <param name="ar">An IAsyncResult that references the asynchronous send.</param>
        private void SendCallback(IAsyncResult ar)
        {
            ClientState state = (ClientState)ar.AsyncState;
            int sentData = state.Socket.EndSend(ar, out SocketError socketError);

            Diagnostics.LogInfo(Helper.ContextLogger(string.Format("BytesSent : {0}   TotalBytes: {1}", sentData, state.DataToSend().Length), 2));

            if (socketError != SocketError.Success)
            {
                Diagnostics.LogError(Helper.ContextLogger("SocketError EndPoint :: " + Helper.GetEndPoint(state.Socket), 0));  // Socket Failed
                this.ResetClient(Helper.GetEndPointAddress(state.Socket), state.IPPort); // Remove any lock-objects,sockets,status bit associated to failed socket.
                lock (this.ackLock)
                {
                    this.acknowledgeStatus.Remove(Helper.MD5Hash(state.Message) + state.IPPort);
                }

                this.DataStatusNotify(state.Message, StatusCode.Failure);
                if (this.isStudent)
                {
                    this.isRunning = false;
                }

                return;
            }

            state.DataSent += sentData;

            if (state.DataSent != state.DataToSend().Length)
            {   // not all data was sent so send remaining bytes
                state.Socket.BeginSend(
                             state.DataToSend(), state.DataSent, state.DataToSend().Length - state.DataSent, SocketFlags.None, new AsyncCallback(this.SendCallback), state);
            }
            else
            {
                if (state.DataToSend()[4] == 0)
                {
                    Diagnostics.LogSuccess(Helper.ContextLogger(string.Format("EndPoint {0} Message Sent :: {1}", Helper.GetEndPoint(state.Socket), Helper.ShortLog(state.Message)), 1));
                }
                else
                {
                    Diagnostics.LogSuccess(Helper.ContextLogger(string.Format("Acknowledgement EndPoint {0} Message Sent :: {1}", Helper.GetEndPoint(state.Socket), Helper.MD5Hash(state.Message)), 2));
                }

                // Now that all data has been sent we release the lock.
                this.ReleaseLock(state.IPPort);
                if (state.DataToSend()[4] == 0)
                {
                    new Thread(() => this.WaitForAcknowledgement(Helper.GetEndPointAddress(state.Socket), state.IPPort, state.Message, state.Retries + 1))
                    {
                        Name = Thread.CurrentThread.Name + " # Target IP : " + Helper.GetEndPoint(state.Socket)
                    }.Start();
                }
            }
        }

        /// <summary>
        /// Goes into wait for acknowledgment for a given message,IP 
        /// If acknowledgement is received within given time it calls DataStatusNotify
        /// otherwise it retries.
        /// if max-retries are exceeded it calls DataStatusNotify with failure.
        /// </summary>
        /// <param name="ip">The IP Corresponding to the client it is waiting for.</param>
        /// <param name="ipPort">The IP:Port Corresponding to the client it is waiting for.</param>
        /// <param name="message">Message For which it is waiting</param>
        /// <param name="retries">How many tries have already been made to send above data</param>
        private void WaitForAcknowledgement(string ip, string ipPort, string message, int retries)
        {
            string key = Helper.MD5Hash(message) + ipPort;
            Diagnostics.LogInfo(Helper.ContextLogger("Message : " + Helper.MD5Hash(message) + "\n\t Key : " + key, 4));

            object ackEvent = this.acknowledgeStatus[key];

            if (ackEvent is AutoResetEvent)
            {
                // False if timer expires
                if (((AutoResetEvent)ackEvent).WaitOne(this.waitTimeForAcknowledge))
                {
                    lock (this.ackLock)
                    {
                        this.acknowledgeStatus.Remove(key);
                    }

                    this.DataStatusNotify(message, StatusCode.Success);
                    return;
                }
            }
            else
            {
                Diagnostics.LogError(Helper.ContextLogger("AcknowledgeStatus didn't have AutoResetEvent", 0));
                throw new InvalidOperationException();
            }

            Diagnostics.LogWarning(Helper.ContextLogger("Not Received Acknowledgement", 1));
            lock (this.ackLock)
            {
                this.acknowledgeStatus.Remove(key);
            }

            if (retries < this.maxRetries)
            {
                Socket refreshSocket = (Socket)this.connectedClients[ip];
                this.SendHelper(message, retries, refreshSocket, 0);
            }
            else
            {
                this.DataStatusNotify(message, StatusCode.Failure);
                if (this.isStudent)
                {
                    this.isRunning = false;
                }
            }
        }

        /// <summary>
        /// Starts a connection from student's machine to professor
        /// </summary>
        /// <returns> true if successful connection was made otherwise false.</returns>
        private bool InitiateConnection()
        {
            Diagnostics.LogInfo(Helper.ContextLogger(string.Format("Creating connection to professor IP {0} Port {1}", this.ipAddress, this.port), 1));

            TcpClient tcpClient = new TcpClient();
            if (this.testIP.Length != 0)
            {
                tcpClient.Client.Bind(new IPEndPoint(IPAddress.Parse(this.testIP), 0));
            }

            try
            {
                tcpClient.Connect(this.ipAddress, this.port);
            }
            catch (Exception e)
            {
                Diagnostics.LogError(Helper.ContextLogger($"General : Unable to create connection with professor Error:{e.ToString()}", 0));
            }

            Socket newSocket = tcpClient.Client;
            Thread receivingThread = new Thread(() => this.HandleClientRequest(tcpClient));
            Diagnostics.LogInfo(Helper.ContextLogger(string.Format("This Socket {0} is writable.", Helper.GetEndPoint(newSocket)), 4));

            // Below method polls a socket to tell if it's writable.
            if (newSocket.Poll(-1, SelectMode.SelectWrite))
            {
                Diagnostics.LogInfo(Helper.ContextLogger(string.Format("This Socket {0} is writable.", Helper.GetEndPoint(newSocket)), 5));
                receivingThread.Start();
                lock (this.mainLock)
                {
                    if (!this.lockObjects.ContainsKey(this.ipAddress))
                    {
                        this.lockObjects[this.ipAddress] = new object();
                    }

                    if (!this.lockStatus.ContainsKey(this.ipAddress))
                    {
                        this.lockStatus[this.ipAddress] = false;
                    }

                    this.connectedClients[this.ipAddress] = newSocket;
                }

                this.isRunning = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remove Locks,Status and objects corresponding to an IP
        /// </summary>
        /// <param name="ip">IP for which locks are to be removed.</param>
        /// <param name="ipport">IP:Port for which locks are to be removed.</param>
        private void ResetClient(string ip, string ipport)
        {
            lock (this.mainLock)
            {
                if (!((Socket)this.connectedClients[ip]).Connected)
                {
                    this.connectedClients.Remove(ip);
                    this.ReleaseLock(ipport);
                    this.lockStatus.Remove(ipport);
                    this.lockObjects.Remove(ipport);
                }
                else
                {
                    this.ReleaseLock(ipport);
                }
            }
        }

        /// <summary>
        /// Grants a Lock to a thread . There exists 1 lock/IP.
        /// </summary>
        /// <param name="ipPort">IP for which lock is needed.</param>
        /// <returns>true if lock is granted.</returns>
        private bool GetLock(string ipPort)
        {
            try
            {
                if ((bool)this.lockStatus[ipPort] != true)
                {
                    lock (this.lockObjects[ipPort])
                    {
                        if ((bool)this.lockStatus[ipPort] != true)
                        {
                            lock (this.mainLock)
                            {
                                this.lockStatus[ipPort] = true;
                            }

                            return true;
                        }
                    }
                }
            }
            catch
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Release he lock corresponding to an IP.
        /// </summary>
        /// <param name="ipPort">IP for which lock is to be released</param>
        private void ReleaseLock(string ipPort)
        {
            lock (this.mainLock)
            {
                if (this.lockStatus.ContainsKey(ipPort))
                {
                    this.lockStatus[ipPort] = false;
                }
            }
        }

        /// <summary>
        /// Updates the acknowledgeStatus table to tell that an acknowledgement message has been received.
        /// </summary>
        /// <param name="key">Key corresponding to the message to be acknowledged.</param>
        private void Acknowledge(string key)
        {
            Diagnostics.LogInfo(Helper.ContextLogger("Received Acknowledgement for HASH " + key, 2));
            if (this.acknowledgeStatus.ContainsKey(key))
            {
                Diagnostics.LogInfo(Helper.ContextLogger("Received Acknowledgement for HASH " + key + " was valid ", 1));
                ((AutoResetEvent)this.acknowledgeStatus[key]).Set();
            }
            else
            {
                this.waitTimeForAcknowledge += 1000;
            }
        }
    }
}