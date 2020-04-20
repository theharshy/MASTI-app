//-----------------------------------------------------------------------
// <author>
//      Suman Saurav Panda
// </author>
// <reviewer>
//      Sooraj Tom
// </reviewer>
// <date>
//      30-Oct-2018
// </date>
// <summary>
//      Server side class implementation for passing the bitmap image to UI using event handler 
// </summary>
// <copyright file="ImageEventArgs.cs" company="B'15, IIT Palakkad">
//     This project is licensed under GNU General Public License v3. (https://fsf.org) 
// </copyright>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// this class is the event arguement class for passing the image bitmap to its intended subscriber
    /// </summary>
    public class ImageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the ImageBitmap which would be passed to UI
        /// </summary>
        public Bitmap ImageBitMap { get; set; }
    }
}
