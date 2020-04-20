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
//      Server side class implementation for passing the error message to main class of Imaging module 
// </summary>
// <copyright file="ErrorEventArgs.cs" company="B'15, IIT Palakkad">
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
    /// this class is event arguement class for passing error message to its intended subscriber
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
