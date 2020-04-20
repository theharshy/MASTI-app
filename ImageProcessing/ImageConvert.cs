//-----------------------------------------------------------------------
// <author> 
//     Amish Ranjan
// </author>
//
// <date> 
//     30-10-2018 
// </date>
// 
// <reviewer> 
//     Sooraj Tom 
// </reviewer>
// 
// <copyright file="ImageConvert.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This class overrides some of the methods of Newtonsoft.Json to convert
//      dictionary of int, bitmap to string.
// </summary>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System;
    using System.Drawing;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// This class overrides some of the methods of Newtonsoft.Json to convert
    /// dictionary of int, bitmap to string.
    /// </summary>
    public class ImageConvert : Newtonsoft.Json.JsonConverter
    {
        /// <summary>
        /// Helps bitmap to be considered as convertible type in json
        /// </summary>
        /// <param name="objectType">Type of object we want to convert</param>
        /// <returns>boolean if type is convertible or not</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Bitmap);
        }

        /// <summary>
        /// Converts string to bitmap
        /// </summary>
        /// <param name="reader">A json reader</param>
        /// <param name="objectType">type of object we need to convert to bitmap</param>
        /// <param name="existingValue">Current value</param>
        /// <param name="serializer">Json serializer</param>
        /// <returns>Bitmap of the string given</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var m = new MemoryStream(Convert.FromBase64String((string)reader.Value));
            return (Bitmap)Bitmap.FromStream(m);
        }

        /// <summary>
        /// Converts bitmap to string
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value of bitmap</param>
        /// <param name="serializer">Json serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Bitmap bmp = (Bitmap)value;
            MemoryStream m = new MemoryStream();
            bmp.Save(m, System.Drawing.Imaging.ImageFormat.Bmp);

            writer.WriteValue(Convert.ToBase64String(m.ToArray()));
        }
    }
}
