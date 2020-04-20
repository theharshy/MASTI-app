// -----------------------------------------------------------------------
// <author> 
//      Libin N George
// </author>
//
// <date> 
//      03-11-2018 
// </date>
// 
// <reviewer>
//      Ayush Mittal
// </reviewer>
//
// <copyright file="StubSchemaForNetworking.cs" company="B'15, IIT Palakkad">
//      This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
//
// <summary>
//      This file implements a stub for ISchema 
//      Note that only the functions used in Networking module are implemented 
// </summary>
// -----------------------------------------------------------------------

namespace Masti.Networking.Stubs
{
    using System;
    using System.Collections.Generic;
    using Masti.Schema;

    /// <summary>
    /// A Stub class implementing ISchema
    /// </summary>
    public class StubSchemaForNetworking : ISchema
    {
        /// <summary>
        /// stub function for decode (only partial decode is implemented)
        /// </summary>
        /// <param name="data">data packet</param>
        /// <param name="partialDecoding">always true in case of Networking module</param>
        /// <returns>Dictionary containing type</returns>
        public IDictionary<string, string> Decode(string data, bool partialDecoding)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            if (partialDecoding)
            {
                if (data.Contains("Image"))
                {
                    dictionary.Add("type", "ImageProcessing");
                    return dictionary;
                }
                else if (data.Contains("Messaging"))
                {
                    dictionary.Add("type", "Messaging");
                    return dictionary;
                }
                else
                {
                    dictionary.Add("type", "Not Found");
                    return dictionary;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// stub function not implemented as it is not used in Networking Module
        /// </summary>
        /// <param name="tagDict">Dictionary for encode</param>
        /// <returns>encoded data</returns>
        public string Encode(Dictionary<string, string> tagDict)
        {
            throw new NotImplementedException();
        }
    }
}
