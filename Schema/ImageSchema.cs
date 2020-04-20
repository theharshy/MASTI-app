//-----------------------------------------------------------------------
// <copyright file="ImageSchema.cs" company="B'15, IIT Palakkad">
//      Open Source. Feel free to use the code, but don't forget to acknowledge. 
// </copyright>
// <author>
//      vishal kumar chaudhary
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
    using System.Threading.Tasks;
    using Masti.QualityAssurance;

    /// <summary>
    /// Class implementing the ISchema interface which is specific to image encoding and decoding
    /// </summary>
    public class ImageSchema : ISchema
    {
        /// <summary>
        /// Decodes the string and returns as key-value pair in a dictionary
        /// </summary>
        /// <param name="data">Encoded data in string format</param>
        /// <param name="partialDecoding">> Boolean if true just returns type of data (ImageProcessing or Messaging)</param>
        /// <returns> Dictionary containing key-value pairs of the encoded data</returns>
        public IDictionary<string, string> Decode(string data, bool partialDecoding)
        {
            if (data == null)
            {
                MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Null data given to decode"));
                throw new ArgumentNullException(nameof(data));
            }

            Dictionary<string, string> tagDict = new Dictionary<string, string>();
            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Starting data decoding"));

            // extracting hash for the given data
            string[] keyValues = data.Split(';');
            int numberOfKeyValues = keyValues.Length;
            string hashOfEncodedData = keyValues[numberOfKeyValues - 1];
            
            // collecting all string array without hash
            string encodedDataWithoutHash = string.Empty;
            for (int index = 0; index < numberOfKeyValues - 1; index++)
            {
                encodedDataWithoutHash += keyValues[index] + ";";
            }

            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Checking the validity of data (sanity check)"));
            if ("Hash:" + ComputeSha256Hash(encodedDataWithoutHash) != hashOfEncodedData)
            {
                MastiDiagnostics.LogError("Data given is corrupted");
                throw new ArgumentException("Corrupted data");
            }
            
            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Sanity check passed"));

            // for partial decoding returning the dictionary only containing type of data (ImageProcessing/Messaging)
            if (partialDecoding == true)
            {
                MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Partial decoding started"));
                string[] type = keyValues[0].Split(':');

                // type is the first tag inserted into data while encoding.
                if (DecodeFrom64(type[0]) == "type")
                {
                    tagDict.Add(DecodeFrom64(type[0]), DecodeFrom64(type[1]));
                }
                else
                {
                    MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Invalid data given to decode which does not contain type"));
                    throw new System.ArgumentException("Data is corrupted as it does not contain the type(tag) of data", nameof(data));
                }

                MastiDiagnostics.LogSuccess(string.Format(CultureInfo.CurrentCulture, "Decoded Successfully"));
                return tagDict;
            }

            // fully decoding if the partialDecoding is false and adding into the dictionary
            foreach (string keyValue in keyValues)
            {
                tagDict.Add(DecodeFrom64(keyValue.Split(':')[0]), DecodeFrom64(keyValue.Split(':')[1]));
            }

            tagDict.Remove("type");
            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Data decoded successfully"));
            return tagDict;
        }

        /// <summary>
        /// Encodes the data given into a dictionary containing tags into standard format(JSON)
        /// </summary>
        /// <param name="tagDict"> Dictionary containing Tags to be encoded</param>
        /// <returns> Encoded data in string format</returns>
        public string Encode(Dictionary<string, string> tagDict)
        {
            if (tagDict == null)
            {
                MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Data given to encode is empty"));
                throw new ArgumentNullException(nameof(tagDict));
            }

            Dictionary<string, string>.KeyCollection keys = tagDict.Keys;
            string encodedData = string.Empty;

            MastiDiagnostics.LogInfo(string.Format(CultureInfo.CurrentCulture, "Encoding started"));

            // inserting the Type tag into encoding data
            encodedData = EncodeTo64("type") + ':' + EncodeTo64("ImageProcessing") + ';';
            foreach (string key in keys)
            {
                encodedData += EncodeTo64(key) + ':' + EncodeTo64(tagDict[key]) + ";";
            }

            MastiDiagnostics.LogInfo("Hash is calculated and appended for sanity check in future");

            // calculating hash of encoded data and append hash of it at the end of encoded data
            encodedData = encodedData + "Hash:" + ComputeSha256Hash(encodedData);
            MastiDiagnostics.LogSuccess(string.Format(CultureInfo.CurrentCulture, "Data encoded successfully"));
            return encodedData;
        }

        /// <summary>
        /// Converting string to base64 Encoding
        /// </summary>
        /// <param name="toEncode"> Data to encode into Base64</param>
        /// <returns>Encoded Base64 string</returns>
        private static string EncodeTo64(string toEncode)
        {
            if (toEncode == null)
            {
                throw new ArgumentException("ERROR Encoded/EncodedTo64 :Empty data to encode");
            }

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        /// <summary>
        /// Decode base64 Decoding string
        /// </summary>
        /// <param name="encodedData"> Data to decode</param>
        /// <returns> Decoded ASCII string</returns>
        private static string DecodeFrom64(string encodedData)
        {
            if (encodedData == null)
            {
                throw new ArgumentException("ERROR Encoded/EncodedTo64 :Empty data to encode");
            }

            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        /// <summary>
        /// Calculating hash of the string
        /// </summary>
        /// <param name="rawData"> String data for hash calculation</param>
        /// <returns> Hash of data in string format</returns>
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
