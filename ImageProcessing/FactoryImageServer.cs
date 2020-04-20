// -----------------------------------------------------------------------
// <author> 
//      Suman Saurav Panda
// </author>
//
// <date> 
//      1-10-2018 
// </date>
// 
// <reviewer>
//      Sooraj Tom
// </reviewer>
//
// <copyright file="FactoryImageServer.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
//
// <summary>
//      This file is the main class for Image Processing module.
//      Calls to other classes originate from here.
// </summary>
// -----------------------------------------------------------------------
namespace Masti.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// this is a static factory class to return the image obect of given IImageProcessing interface
    /// </summary>
    public static class FactoryImageServer
    {
        /// <summary>
        /// this is the static method given to UI team to create ImageProcessing object of IImageProcessing interface
        /// </summary>
        /// <returns>returns Image Processing object with proper interface to the caller class</returns>
        public static IImageProcessing CreateImageProcessing()
        {
            return new ImageProcessing();
        }
    }
}
