//-----------------------------------------------------------------------
// <copyright file="FullEncodingDecodingImageSchemaTest.cs" company="B'15, IIT Palakkad">
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
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Masti.QualityAssurance;

    /// <summary>
    /// Test for proper encoding and decoding.
    /// </summary>
    public class FullEncodingDecodingImageSchemaTest : ITest
    {
        /// <summary>
        /// Path denotes path to test images
        /// </summary>
        private static string path = "../../../Schema/ImageSchemaTestData/partialEncodingDecodingImageSchema.txt";

        /// <summary>
        /// The logger is used for Logging functionality within the test.
        /// Helps in debugging of tests.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FullEncodingDecodingImageSchemaTest" /> class.
        /// </summary>
        /// <param name="logger">Assigns the Logger to be used by the Test</param>
        public FullEncodingDecodingImageSchemaTest(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Describe the Comparison test defined by the module developer.
        /// </summary>
        /// <returns>Returns whether the status is successful or not.</returns>
        public bool Run()
        {
            this.logger.LogInfo("Testing started");
            if (!File.Exists(@path))
            {
                this.logger.LogError("File path =" + path + " does not exist");
                this.logger.LogError("test failed");
                return false;
            }

            string[] lines = File.ReadAllLines(path: @path);
            int lineNumber = 0;
            int numOfKeys = int.Parse(lines[lineNumber], CultureInfo.CurrentCulture);
            lineNumber += 1;

            Dictionary<string, string> testDictionary = new Dictionary<string, string>();
            //// adding all string to string tags
            while (numOfKeys > 0)
            {
                numOfKeys -= 1;
                string[] keyValue = lines[lineNumber].Split(':');
                lineNumber += 1;
                testDictionary.Add(keyValue[0], keyValue[1]);
            }

            numOfKeys = int.Parse(lines[lineNumber], CultureInfo.CurrentCulture);
            lineNumber += 1;
            //// adding all string to image_string tags
            while (numOfKeys > 0)
            {
                numOfKeys -= 1;
                string[] keyValue = lines[lineNumber].Split(':');
                lineNumber += 1;
                path = "../../../Schema/";
                if (!File.Exists(@path + keyValue[1].Replace(" ", string.Empty)))
                {
                    this.logger.LogError("Image path " + path + keyValue[1].Replace(" ", string.Empty) + " does not exist ");
                    this.logger.LogError("Test failed");
                    return false;
                }

                byte[] imageBytes = File.ReadAllBytes(@path + keyValue[1].Replace(" ", string.Empty));
                testDictionary.Add(keyValue[0], Convert.ToBase64String(imageBytes));
            }

            this.logger.LogInfo("Test file read successful");
            ImageSchema imageSchema = new ImageSchema();
            this.logger.LogInfo("Encoding data started");

            // encoding data with help of imageSchema Encode function
            string encodedata = imageSchema.Encode(testDictionary);
            this.logger.LogInfo("Data encoding completed");

            // decoding data with help of imageSchema Encode function
            this.logger.LogInfo("Decoding data started");
            Dictionary<string, string> decodedDictionary = new Dictionary<string, string>(imageSchema.Decode(encodedata, false));

            // checking all the key value pair are same or not for full decoding
            this.logger.LogInfo("Decoding data ended");
            this.logger.LogInfo("checking if encoding decoding is correct");
            foreach (string key in testDictionary.Keys)
            {
                if (!(decodedDictionary.ContainsKey(key) & decodedDictionary[key] == testDictionary[key]))
                {
                    this.logger.LogError("Test failed");
                    return false;
                }
            }

            this.logger.LogSuccess("Test successful");
            return true;
        }
    }
}
