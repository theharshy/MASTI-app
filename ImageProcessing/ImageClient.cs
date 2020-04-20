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
// <copyright file="ImageClient.cs" company="B'15, IIT Palakkad">
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
    using IitPkd.SchemaTeam;
    using Networking;

    /// <summary>
    /// this class handles the client side function of image module.
    /// the internal objects are created here and assigned their job.
    /// </summary>
    public class ImageClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageClient"/> class.
        /// The constructor for Image processing in client side
        /// </summary>
        public ImageClient()
        {
            this.Communication = GetCommunicator();
            this.ImageSchema = GetSchema();
            this.ImageCommunication = new ImageCommunication(this.Communication, this.ImageSchema);
            this.Compression = new Compression();
            this.ImageProcessingClient = new ImageProcessingClient(this.ImageSchema, this.ImageCommunication, this.Compression);
        }

        /// <summary>
        /// Gets or sets the ImageSchema object
        /// </summary>
        private ISchema ImageSchema { get; set; }

        /// <summary>
        /// Gets or sets the Communication object
        /// </summary>
        private ICommunication Communication { get; set; }

        /// <summary>
        /// Gets or sets the ImageProcessing client object
        /// </summary>
        private ImageProcessingClient ImageProcessingClient { get; set; }

        /// <summary>
        /// Gets or sets the image communication object
        /// </summary>
        private ImageCommunication ImageCommunication { get; set; }

        /// <summary>
        /// Gets or sets the Compression object in Image module
        /// </summary>
        private Compression Compression { get; set; }

        /// <summary>
        /// this method is to be implemented after getting factory method from networking team
        /// </summary>
        /// <returns>return a ICommunication object</returns>
        private static ICommunication GetCommunicator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// this method is to be implemented after getting the factory method from the schema team
        /// </summary>
        /// <returns>return an object of schema type</returns>
        private static ISchema GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
