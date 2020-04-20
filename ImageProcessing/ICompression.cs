//-----------------------------------------------------------------------
// <author> 
//     Amish Ranjan
// </author>
//
// <date> 
//     11-10-2018 
// </date>
// 
// <reviewer> 
//     Sooraj Tom 
// </reviewer>
// 
// <copyright file="ICompression.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This is an interface intended to be used for Compression class, which would provide 
//      option to compress and decompress image.
// </summary>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// This interface provides compression and decompression of image for ImageProcessing team.
    /// </summary>
    public interface ICompression
    {
        /// <summary>
        /// It compresses the given Bitmap using diff technique.
        /// </summary>
        /// <param name="curBitmap">Bitmap for current image</param>
        /// <param name="implementDiff">Give true/false whether you want to implement diff or 
        /// not</param>
        /// <returns>
        /// A dictionary of int and Bitmap, with int representing the changed section of screen
        /// if diff implemented
        /// </returns>
        Dictionary<int, Bitmap> Compress(Bitmap curBitmap, bool implementDiff);

        /// <summary>
        /// It decompresses the given Bitmap dictionary using diff technique.
        /// </summary>
        /// <param name="curBmpDict">Dictionary of int and Bitmap representing compressed 
        /// form of current Bitmap</param>
        /// <param name="implementDiff">Give true/false whether you want to implement diff
        /// or not</param>
        /// <returns>A Bitmap of current image</returns>
        Bitmap Decompress(Dictionary<int, Bitmap> curBmpDict, bool implementDiff);

        /// <summary>
        /// This method helps to convert a dictionary of int, bitmap to json string
        /// </summary>
        /// <param name="bmpDict">A int, bitmap dictionary</param>
        /// <returns>string in json format</returns>
        string BmpDictToString(Dictionary<int, Bitmap> bmpDict);

        /// <summary>
        /// This method helps to convert a json string back to dictionady of int, bitmap
        /// </summary>
        /// <param name="bmpDictString">Json string of dictionary of int, bitmap</param>
        /// <returns>Dictionary of int, bitmap</returns>
        Dictionary<int, Bitmap> StringToBmpDict(string bmpDictString);

        /// <summary>
        /// It tests working of complete Compression class.
        /// </summary>
        /// <returns>True/False Compression class works correctly/incorrectly</returns>
        bool TestCompression();
    }
}
