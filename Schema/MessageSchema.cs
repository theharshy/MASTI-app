//-----------------------------------------------------------------------
// <copyright file="MessageSchema.cs" company="B'15, IIT Palakkad">
//      Open Source. Feel free to use the code, but don't forget to acknowledge. 
// </copyright>
// <author>
//      Harsh Yadav
// </author>
// <review>
//      Libin N George
// </review>
//-----------------------------------------------------------------------

namespace Masti.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Masti.QualityAssurance;

    /// <summary>
    /// The following class implements the interface of ISchema for text message encoding and partial and/or full decoding
    /// </summary>
    public class MessageSchema : ISchema
    {
        /// <summary>
        /// Encodes data obtained from a dictionary to a string
        /// </summary>
        /// <param name="tagDict">Dictionary containing tag to be encoded</param>
        /// <returns>Encoded string of data from dictionary</returns>
        public string Encode(Dictionary<string, string> tagDict)
        {
            if (tagDict == null)
            {
                MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Null is not accepted to encode"));
                throw new ArgumentNullException(nameof(tagDict));
            }

            Dictionary<string, string>.KeyCollection keys = tagDict.Keys;
            string result = string.Empty;
            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Encoding started"));

            // inserting the Type tag into encoding data
            result = EncodeTo64("type") + ':' + EncodeTo64("Messaging") + ';';
            foreach (string key in keys)
            {
                result += EncodeTo64(key) + ':' + EncodeTo64(tagDict[key]) + ";";
            }
            
            // calculating hash of encoded data and append hash of it at the end of encoded data
            result = result + "Hash:" + ComputeSha256Hash(result);
            MastiDiagnostics.LogInfo("Hash calculation successful. Appended to data for sanity check.");
            return result;
        }

        /// <summary>
        /// Function used to parse string data to convert into JSON format
        /// </summary>
        /// <param name="data">Encoded string</param>
        /// <param name="partialDecoding">Flag used to partially decode data to get object type (ImageProcessing/Messaging)</param>
        /// <returns>Dictionary of tags of encoded data</returns>
        public IDictionary<string, string> Decode(string data, bool partialDecoding)
        {
            Dictionary<string, string> tagDict = new Dictionary<string, string>();
            if (data == null)
            {
                MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Null data cannot be decoded."));
                throw new ArgumentNullException(nameof(data));
            }

            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Decoding started."));
            string[] keyValues = data.Split(';');
            int numberOfKeyValues = keyValues.Length;
            string hashOfEncodedData = keyValues[numberOfKeyValues - 1];

            // collecting all string array without hash
            string encodedDataWithoutHash = string.Empty;
            for (int index = 0; index < numberOfKeyValues - 1; index++)
            {
                encodedDataWithoutHash += keyValues[index] + ";";
            }

            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Performing sanity check by comparing hash."));
            if ("Hash:" + ComputeSha256Hash(encodedDataWithoutHash) != hashOfEncodedData)
            {
                MastiDiagnostics.LogError("Data sent and received do not match.");
                throw new ArgumentException("Corrupted data");
            }

            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Sanity check successful."));

            // for partial decoding returning the dictionary only containing type of data give (Image/Message/Other)
            if (partialDecoding == true)
            {
                MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Partial decoding started"));
                string[] type = keyValues[0].Split(':');

                // Obtaining type of tag (Image/Message/Other)
                if (DecodeFrom64(type[0]) == "type")
                {
                    tagDict.Add(DecodeFrom64(type[0]), DecodeFrom64(type[1]));
                }
                else
                {
                    MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Type tag not found. Partial decoding failed."));
                    throw new System.ArgumentException("Data has no type tag. Partial decoding failed.", nameof(data));
                }

                MastiDiagnostics.LogSuccess(string.Format(CultureInfo.CurrentCulture, "Partial decoding successful"));
                return tagDict;
            }

            // Complete decoding of the message and returning the dictionary
            foreach (string keyValue in keyValues)
            {
                tagDict.Add(DecodeFrom64(keyValue.Split(':')[0]), DecodeFrom64(keyValue.Split(':')[1]));
            }

            tagDict.Remove("type");
            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Data decoding successful"));
            return tagDict;
        }
        
        /// <summary>
        /// Encodes a string to base64 scheme
        /// </summary>
        /// <param name="toEncode">Data to encode into Base64</param>
        /// <returns>Encoded string</returns>
        private static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        /// <summary>
        /// Decodes a string from base64 encoding
        /// </summary>
        /// <param name="encodedData">Data to decode from Base64</param>
        /// <returns>Decoded string from base64 to normal</returns>
        private static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        /// <summary>
        /// Calculating Hash of the string
        /// </summary>
        /// <param name="rawData">string data for hash calculation</param>
        /// <returns>hash of data in  string format</returns>
        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(value: bytes[i].ToString("x2", CultureInfo.CurrentCulture));
                }

                return builder.ToString();
            }
        }
    }
}
