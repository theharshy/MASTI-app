//-----------------------------------------------------------------------
// <author> 
//     Ravindra Kumar
// </author>
//
// <date> 
//     12-10-2018 
// </date>
// 
// <reviewer> 
//     Sooraj Tom 
// </reviewer>
// 
// <copyright file="IImageProcessingClient.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This file contains interface for Client-side Image Processing. 
//      This interface is for my team members and not for the external teams.
// </summary>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System.Net;

    /// <summary>
    /// Prototype for client-side Image processing module.
    /// </summary>
    public interface IImageProcessingClient
    {
        /// <summary>
        /// Function which receive the signal to start the screen sharing.
        /// </summary>
        /// <param name="data">Any Data passed by server. For now it is assummed null.</param>
        /// <param name="toIP">IP address of server whom to send the capturedimage.</param>
        void StartSharingEventHandler(string data, IPAddress toIP);

        /// <summary>
        /// Function which receive the signal to stop the screen sharing and disposes the timer.
        /// </summary>
        /// <param name="data">Any Data passed by server. For now it is assummed null.</param>
        /// <param name="toIP">IP address of server whom to send the capturedimage.</param>
        void StopSharingEventHandler(string data, IPAddress toIP);

        /// <summary>
        /// Function which receive the signal when some image packet is lost and a new image packet is requested.
        /// It will capture the screen and send it.( It don't register to timer).
        /// </summary>
        /// <param name="data">Any Data passed by server. For now it is assummed null.</param>
        /// <param name="toIP">IP address of server whom to send the capturedimage.</param>
        void ResendEventHandler(string data, IPAddress toIP);
    }
}
