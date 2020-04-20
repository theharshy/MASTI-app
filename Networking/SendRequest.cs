// -----------------------------------------------------------------------
// <author> 
//     Jude K Anil 
// </author>
//
// <date> 
//     11-10-2018 
// </date>
// 
// <reviewer> 
//     Libin N George 
// </reviewer>
// 
// <copyright file="SendRequest.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      The file contains a definition of a structure along
//      with an instantiation of a concurrent queue for items
//      of type of the defined structure. The instantiation of
//      the queue is done in a partial class, i.e. Communication.
// </summary>
// -----------------------------------------------------------------------

namespace Masti.Networking
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;

    /// <summary>
    /// The structure encapsulated the message and relevant meta information needed
    /// for correct transmission over the network.
    /// </summary>
    public struct SendRequest : IEquatable<SendRequest>
    {
        /// <summary>
        /// This field stores the message to transferred.
        /// This field cannot be reset to a different value.
        /// </summary>
        private string data;

        /// <summary>
        /// This field is used to store target IP-Address
        /// This field cannot be reset to a different value.
        /// </summary>
        private IPAddress targetIPAddress;

        /// <summary>
        /// This field is used to store the message originating module information.
        /// </summary>
        private DataType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendRequest" /> struct.
        /// </summary>
        /// <param name="msg">Data to be seny</param>
        /// <param name="targetIP">recipient IP</param>
        /// <param name="type">Value that tells source component.</param>
        public SendRequest(string msg, IPAddress targetIP, DataType type) : this()
        {
            this.data = msg;
            this.targetIPAddress = targetIP;
            this.type = type;
        }

        /// <summary>
        /// Gets the value of the message field.
        /// </summary>
        public string Data { get => this.data; }

        /// <summary>
        /// Gets the value of targetIPAddress.
        /// </summary>
        public IPAddress TargetIPAddress { get => this.targetIPAddress; }

        /// <summary>
        /// Gets or sets the value of type.
        /// </summary>
        public DataType Type { get => this.type; set => this.type = value; }

        /// <summary>
        /// Syntactic sugar operator method for the Equals method defined above.
        /// </summary>
        /// <param name="left">The SendRequest object on the left side of the '==' operator.</param>
        /// <param name="right">The SendRequest object on the right side of the '==' operator.</param>
        /// <returns>The boolean result of equality testing.</returns>
        public static bool operator ==(SendRequest left, SendRequest right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Operator method that returns the negation of the'==' operator method.
        /// </summary>
        /// <param name="left">The SendRequest object on the left side of the '!=' operator.</param>
        /// <param name="right">The SendRequest object on the right side of the '!=' operator.</param>
        /// <returns>The boolean result of not-equal testing.</returns>
        public static bool operator !=(SendRequest left, SendRequest right)
        {
            return !(left == right);
        }

        /// <summary>
        /// A method that tests equality of the types of the objects being compared
        /// and consequently checks for equality of object values.
        /// </summary>
        /// <param name="obj">The object to be compared against.</param>
        /// <returns>The boolean result of checking equality.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SendRequest))
            {
                return false;
            }

            return this.Equals((SendRequest)obj);
        }

        /// <summary>
        /// This method is called by the object Equals method. It checks for equality
        /// of values of the fields of sendRequest. 
        /// </summary>
        /// <param name="other">The SendRequest object to be checked against for equality.</param>
        /// <returns>The boolean result of checking equality.</returns>
        public bool Equals(SendRequest other)
        {
            if (this.Data.Equals(other.Data, StringComparison.Ordinal) & this.TargetIPAddress.Equals(other.TargetIPAddress) &
                this.Type.Equals(other.Type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
   
        /// <summary>
        /// A method that computes the hash-value of an object of type sendRequest.
        /// It does this by XORing the hash-values of Data and TargetIpAddress
        /// along with int value of Type.
        /// </summary>
        /// <returns>The hash value of the calling object.</returns>
        public override int GetHashCode()
        {
            return this.Data.GetHashCode() ^ this.TargetIPAddress.GetHashCode() ^ (int)this.Type;
        }
    }

    /// <summary>
    /// A partial class containing the functionalities of Communication module.
    /// </summary>
    public partial class Communication : ICommunication
    {
        /// <summary>
        /// Gets the Queue. This queue is used to store structures that encapsulate messages
        /// received by the communication module for the purpose of transmission over
        /// the network. The queue allows concurrent access and hence is thread-safe.
        /// </summary>
        public static ConcurrentQueue<SendRequest> SendRequestQueue { get; } = new ConcurrentQueue<SendRequest>();
    }
}
