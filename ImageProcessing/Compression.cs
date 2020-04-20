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
// <copyright file="Compression.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This is a class intended to provide option to compress and decompress image.
//      It uses diff technique to implement compression. The whole image is divided into
//      xDiv * yDiv(variables) parts, Compress returns only those parts which are different
//      from previous one. Decompress checks for key -1(full image) if found returns the 
//      whole image, else uses the previous image to return current image using same diff.
// </summary>
//-----------------------------------------------------------------------

namespace Masti.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Newtonsoft.Json;
    using System.IO;
    using System.Security.Cryptography;
    using System.Linq;

    /// <summary>
    /// This class provides option for compression and decompression of image.
    /// </summary>
    public class Compression : ICompression, IDisposable
    {
        /// <summary>
        /// Makes sure that a bitmap dictionary is created for further use if customized
        /// constructor for diff is called.
        /// </summary>
        private bool createPrevBmp = false;

        /// <summary>
        /// To be used on client side to store values corresponding to previous image.
        /// </summary>
        private Dictionary<int, byte[]> prevBmpDict = new Dictionary<int, byte[]>();

        /// <summary>
        /// To be used on server side to store values corresponding to previous image.
        /// </summary>
        private Bitmap prevBmp;

        /// <summary>
        /// Number of divisions of image on x axis and y axis, feel free to make changes.
        /// </summary>
        private int xDiv = 1, yDiv = 4;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the Compression class
        /// </summary>
        public Compression()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether createPrevBmp is set or not
        /// </summary>
        public bool CreatePrevBmp
        {
            get
            {
                return this.createPrevBmp;
            }

            set
            {
                this.createPrevBmp = value;
            }
        }

        /// <summary>
        /// It compresses the given Bitmap using diff technique.
        /// </summary>
        /// <param name="curBitmap">Bitmap for current image</param>
        /// <param name="implementDiff">Give true/false whether you want to implement 
        /// diff or not</param>
        /// <returns>
        /// A dictionary of int and Bitmap, with int representing the changed section 
        /// of screen if diff implemented
        /// </returns>
        public Dictionary<int, Bitmap> Compress(Bitmap curBitmap, bool implementDiff)
        {
            Dictionary<int, Bitmap> bmpDict = new Dictionary<int, Bitmap>();

            if (curBitmap == null)
            {
                bmpDict.Add(-2, null);
                return bmpDict;
                ////throw new ArgumentNullException("Invalid Bitmap recieved");
            }

            int bmpPlace = 0;
            int widthBmp = curBitmap.Width;
            int heightBmp = curBitmap.Height;
            int[] recDim = new int[4];

            Bitmap divBmp;

            if (implementDiff == true && this.prevBmpDict.Count != this.xDiv * this.yDiv)
            {
                bmpDict.Add(-2, null);
                return bmpDict;
            }
            else if (implementDiff == false && this.createPrevBmp == false)
            {
                bmpDict.Add(-1, curBitmap);
                return bmpDict;
            }

            for (int x = 0; x < this.xDiv; ++x)
            {
                for (int y = 0; y < this.yDiv; ++y)
                {
                    recDim = this.GetRectangleDim(x, y, widthBmp, heightBmp);
                    Rectangle rect = new Rectangle(recDim[0], recDim[1], recDim[2], recDim[3]);
                    divBmp = curBitmap.Clone(rect, curBitmap.PixelFormat);
                    byte[] hashCode = this.GetHashCode(divBmp);
                    if (implementDiff == true && !hashCode.SequenceEqual(this.prevBmpDict[bmpPlace]))
                    {
                        this.prevBmpDict[bmpPlace] = hashCode;
                        bmpDict.Add(bmpPlace, divBmp);
                    }
                    else if (this.createPrevBmp == true)
                    {
                        this.prevBmpDict.Add(bmpPlace, hashCode);
                    }

                    bmpPlace++;
                }
            }

            if (this.createPrevBmp == true)
            {
                bmpDict.Add(-1, curBitmap);
            }

            return bmpDict;
        }

        /// <summary>
        /// It decompresses the given Bitmap dictionary using diff technique.
        /// </summary>
        /// <param name="curBmpDict">Dictionary of int and Bitmap representing 
        /// compressed form of current Bitmap</param>
        /// <param name="implementDiff">Give true/false whether you want to implement 
        /// diff or not</param>
        /// <returns>A Bitmap of current image</returns>
        public Bitmap Decompress(Dictionary<int, Bitmap> curBmpDict, bool implementDiff)
        {
            if (curBmpDict == null)
            {
                return null;
                ////throw new ArgumentNullException("Invalid Bitmap dictionary recieved");
            }

            bool errorVal = false;
            if (curBmpDict.ContainsKey(-1))
            {
                this.prevBmp = new Bitmap(curBmpDict[-1]);
            }
            else
            {
                this.StitchImage(curBmpDict, ref errorVal);
            }

            if (errorVal == true)
            {
                return null;
            }

            return this.prevBmp;
        }

        /// <summary>
        /// This method helps to convert a dictionary of int, bitmap to json string
        /// </summary>
        /// <param name="bmpDict">A int, bitmap dictionary</param>
        /// <returns>string in json format</returns>
        public string BmpDictToString(Dictionary<int, Bitmap> bmpDict)
        {
            string json;
            json = JsonConvert.SerializeObject(bmpDict, new ImageConvert());
            return json;
        }

        /// <summary>
        /// This method helps to convert a json string back to dictionady of int, bitmap
        /// </summary>
        /// <param name="bmpDictString">Json string of dictionary of int, bitmap</param>
        /// <returns>Dictionary of int, bitmap</returns>
        public Dictionary<int, Bitmap> StringToBmpDict(string bmpDictString)
        {
            Dictionary<int, Bitmap> bmpDict;
            bmpDict = JsonConvert.DeserializeObject<Dictionary<int, Bitmap>>(bmpDictString, new ImageConvert());
            return bmpDict;
        }

        /// <summary>
        /// It tests working of complete Compression class.
        /// </summary>
        /// <returns>True/False Compression class works correctly/incorrectly</returns>
        public bool TestCompression()
        {
            using (Compression testCompressor = new Compression())
            {
                using (Compression testDecompressor = new Compression())
                {
                    bool testWithDiff = this.TestUtility(true, testCompressor, testDecompressor);
                    bool testWithoutDiff = this.TestUtility(false, testCompressor, testDecompressor);
                    return testWithDiff && testWithoutDiff;
                }
            }
        }

        /// <summary>
        /// Public implementation of Dispose pattern.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">boolean to trigger disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.prevBmp != null)
                {
                    this.prevBmp.Dispose();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// This method does the unit testing of this module when diff is enabled/disabled.
        /// </summary>
        /// <param name="implementDiff">Give true/false whether you want to implement 
        /// diff or not</param>
        /// <param name="testCompressor"> Compression object for client side </param>
        /// <param name="testDecompressor">Compression object for Server side </param>
        /// <returns>true/false for correct/incorrect functioning</returns>
        private bool TestUtility(bool implementDiff, Compression testCompressor, Compression testDecompressor)
        {
            Bitmap bmp1 = (Bitmap)Image.FromFile(@"../../Specs/111501032Amish/1.png");
            Bitmap bmp2 = (Bitmap)Image.FromFile(@"../../Specs/111501032Amish/2.png");

            Dictionary<int, Bitmap> bmp1Dict;

            if (implementDiff == true)
            {
                testCompressor.CreatePrevBmp = true;
                bmp1Dict = testCompressor.Compress(bmp1, false);
                testCompressor.CreatePrevBmp = false;
            }
            else
            {
                bmp1Dict = testCompressor.Compress(bmp1, false);
            }
            
            if (bmp1Dict.ContainsKey(-2))
            {
                return false;
            }

            Bitmap bmp1Decompressed = testDecompressor.Decompress(bmp1Dict, false);
            if (bmp1Decompressed == null || !this.Equals(bmp1, bmp1Decompressed))
            {
                return false;
            }

            if (implementDiff == true)
            {
                Dictionary<int, Bitmap> bmp2Dict = testCompressor.Compress(bmp2, true);
                if (bmp2Dict.ContainsKey(-2))
                {
                    return false;
                }

                Bitmap bmp2Decompressed = testDecompressor.Decompress(bmp2Dict, true);
                if (bmp2Decompressed == null || !this.Equals(bmp2, bmp2Decompressed))
                {
                    return false;
                }
            }
            else
            {
                Dictionary<int, Bitmap> bmp2Dict = testCompressor.Compress(bmp2, false);
                if (bmp2Dict.ContainsKey(-2))
                {
                    return false;
                }

                Bitmap bmp2Decompressed = testDecompressor.Decompress(bmp2Dict, false);
                if (bmp2Decompressed == null || !this.Equals(bmp2, bmp2Decompressed))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method is a helper function of Decompress, it replaces a part of larger bitmap
        /// by smaller one.
        /// </summary>
        /// <param name="curBmpDict">A dictionary of int(representing place) and bitmap to 
        /// be replced in previous bitmap</param>
        /// <param name="errorVal">Changes value to true/false for error/success</param>
        private void StitchImage(Dictionary<int, Bitmap> curBmpDict, ref bool errorVal)
        {
            int x, y;
            int[] recDim = new int[4];
            Bitmap updatedBmp = this.prevBmp;
            foreach (var smallBmpPair in curBmpDict)
            {
                x = smallBmpPair.Key / this.yDiv;
                y = smallBmpPair.Key % this.yDiv;
                recDim = this.GetRectangleDim(x, y, updatedBmp.Width, updatedBmp.Height);
                try
                {
                    Graphics g = Graphics.FromImage(updatedBmp);
                    g.DrawImageUnscaled(smallBmpPair.Value, recDim[0], recDim[1]);
                    g.Dispose();
                }
                catch (ArgumentNullException)
                {
                    errorVal = true;
                    return;
                    ////throw new ArgumentNullException("Invalid image");
                }
            }

            this.prevBmp = updatedBmp;
        }

        /// <summary>
        /// Gets MD5 hashcode for a given bitmap
        /// </summary>
        /// <param name="bmp">Bitmap for which we want hashcode</param>
        /// <returns></returns>
        private byte[] GetHashCode(Bitmap bmp)
        {
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Jpeg); // gif for example
                bytes = ms.ToArray();
            }

            // hash the bytes
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(bytes);
            return hash;
        }

        /// <summary>
        /// This method calculates the parameters of reactangle, given x and y
        /// (x,y) = (0,yDiv) for top left and (x,y) = (xDiv, 0) for bottom right
        /// </summary>
        /// <param name="x">X axis value for bitmap</param>
        /// <param name="y">Y axis value for bitmap</param>
        /// <param name="width">Width for whole bitmap</param>
        /// <param name="height">Height for whole bitmap</param>
        /// <returns>Integer array of top left x and y coordinate of rectange and 
        /// its width and height</returns>
        private int[] GetRectangleDim(int x, int y, int width, int height)
        {
            int recTopLeftX, recTopLeftY, recWidth, recHeight;
            int widthDiv = width / this.xDiv;
            int heightDiv = height / this.yDiv;

            recTopLeftX = x * widthDiv;
            recTopLeftY = y * heightDiv;
            if (x == this.xDiv - 1)
            {
                recWidth = width - (x * widthDiv);
            }
            else
            {
                recWidth = widthDiv;
            }

            if (y == this.yDiv - 1)
            {
                recHeight = height - (y * heightDiv);
            }
            else
            {
                recHeight = heightDiv;
            }

            int[] recDim = new int[4] { recTopLeftX, recTopLeftY, recWidth, recHeight };
            return recDim;
        }

        /// <summary>
        /// Checks two bitmaps if they are eual or not
        /// </summary>
        /// <param name="bmp1">First bitmap</param>
        /// <param name="bmp2">Second bitmap</param>
        /// <returns>Boolean value whether the bitmaps are equal or not</returns>
        private unsafe bool Equals(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
            {
                return false;
            }

            Rectangle rc = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            BitmapData bd1 = bmp1.LockBits(rc, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bd2 = bmp2.LockBits(rc, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            bool retval = true;
            int* p1 = (int*)bd1.Scan0;
            int* p2 = (int*)bd2.Scan0;
            int cnt = bmp1.Height * bd1.Stride / 4;

            for (int ix = 0; ix < cnt; ++ix)
            {
                if (*p1++ != *p2++)
                {
                    retval = false;
                    break;
                }
            }

            bmp1.UnlockBits(bd1);
            bmp2.UnlockBits(bd2);
            return retval;
        }
    }
}
