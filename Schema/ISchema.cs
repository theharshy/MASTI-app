//-----------------------------------------------------------------------
// <copyright file="ISchema.cs" company="B'15, IIT Palakkad">
//      Open Source. Feel free to use the code, but don't forget to acknowledge. 
// </copyright>
// <author>
//      Akshat
// </author>
// <review>
//      Libin N George
// </review>
//-----------------------------------------------------------------------

namespace Masti.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// It provide the interface to encode and decode the data into standard format 
    /// </summary>
    public interface ISchema
    {
        /// <summary>
        /// Decoding the string data into  tags in a dictionary
        /// </summary>
        /// <param name="data">encode data in string format</param>
        /// <param name="partialDecoding">Boolean if true give type of object which encode this data</param>
        /// <returns>Dictionary containing tag of the encode data</returns>
        IDictionary<string, string> Decode(string data, bool partialDecoding);

        /// <summary>
        /// Encodes the data given into a dictionary containing tags into standard format 
        /// </summary>
        /// <param name="tagDict">Dictionary containing Tags to be encoded</param>
        /// <returns>Encode data into string</returns>
        string Encode(Dictionary<string, string> tagDict);
    }
}
