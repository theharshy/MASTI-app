// -----------------------------------------------------------------------
// <author> 
//      Suman Saurav Panda
// </author>
//
// <date> 
//      2-10-2018 
// </date>
// 
// <reviewer>
//      Not reviewed yet
// </reviewer>
//
// <copyright file="FactoryImageClient.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
//
// <summary>
//      This file is the main class for Image Processing module in the client side.
//      creates the internal object required and sets proper relationship between them in the client side.
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
    /// this class is the factory method to return image client object to the UI
    /// </summary>
    public static class FactoryImageClient
    {
        /// <summary>
        /// this factory method returns an object of type imageClient
        /// </summary>
        /// <returns>an object of IImageClient interfacew</returns>
        public static ImageClient CreateImageClient()
        {
            return new ImageClient();
        }
    }
}
