//-----------------------------------------------------------------------
// <author>
//      Sooraj Tom
// </author>
// <reviewer>
//      Axel James
// </reviewer>
// <date>
//      11-Oct-2018
// </date>
// <summary>
//      Interface for the Image Processing module
// </summary>
// <copyright file="IImageProcessing.cs" company="B'15, IIT Palakkad">
//     This project is licensed under GNU General Public License v3. (https://fsf.org) 
// </copyright>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System;

    /// <summary>
    /// The IImageProcessing interface.
    /// This module gives functionality related to sharing screen, storing and retrieving screenshots.
    /// This module will be instantiated by the UI module.
    /// This module is dependent on Networking and Schema modules.
    /// </summary>
    public interface IImageProcessing
    {
        /// <summary>
        /// This method starts the screen sharing process.
        /// It is called by the server (professor) with the IP address of the client (student).
        /// </summary>
        /// <param name="clientIP">This parameter is the IP address of the target client computer</param>
        /// <returns>Returns the status</returns>
        bool GetSharedScreen(string clientIP);

        /// <summary>
        /// This method terminates the screen sharing session.
        /// </summary>
        /// <param name="clientIP">which client to stop sending image</param>
        /// <returns>Returns the status</returns>
        bool StopSharedScreen(string clientIP);

        /// <summary>
        /// This method registers the image updater for UI.
        /// </summary>
        /// <param name="imageReceivedNotifyHandler">The updater function from UI that updates the screen shared</param>
        void RegisterImageUpdateHandler(EventHandler<ImageEventArgs> imageReceivedNotifyHandler);
    }
}
