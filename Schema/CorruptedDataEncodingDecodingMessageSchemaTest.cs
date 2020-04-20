//-----------------------------------------------------------------------
// <copyright file="CorruptedDataEncodingDecodingMessageSchemaTest.cs" company="B'15, IIT Palakkad">
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
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Masti.QualityAssurance;

    /// <summary>
    /// Corrupts data after encoding and matches it with decoded data
    /// </summary>
    public class CorruptedDataEncodingDecodingMessageSchemaTest : ITest
    {
        /// <summary>
        /// Denotes path to test messages
        /// </summary>
        private static string path = "../../../Schema/MessageSchemaTestData/CorruptedDataEncodingDecodingMessageSchema.txt";

        /// <summary>
        /// The logger is used for Logging functionality within the test.
        /// Helps in debugging of tests.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorruptedDataEncodingDecodingMessageSchemaTest" /> class.
        /// </summary>
        /// <param name="logger">Assigns the Logger to be used by the Test</param>
        public CorruptedDataEncodingDecodingMessageSchemaTest(ILogger logger)
        {
            this.logger = logger;
        }
       
        /// <summary>
        /// Tests encoding and decoding functionality by adding noise
        /// </summary>
        /// <returns>Returns whether the status is successful or not.</returns>
        public bool Run()
        {
            this.logger.LogInfo("Testing started");
            if (!File.Exists(@path))
            {
                this.logger.LogError("File path =" + path + " does not exist");
                this.logger.LogError("Test failed");
                return false;
            }
           
            string[] lines = File.ReadAllLines(path: @path);
            int lineNumber = 0;
            int numOfKeys = int.Parse(lines[lineNumber], CultureInfo.CurrentCulture);
            lineNumber += 1;

            Dictionary<string, string> testDictionary = new Dictionary<string, string>();
            
            // adding all string to string tags
            while (numOfKeys > 0)
            {
                numOfKeys -= 1;
                string[] keyValue = lines[lineNumber].Split(':');
                lineNumber += 1;
                testDictionary.Add(keyValue[0], keyValue[1]);
            }
            
            this.logger.LogInfo("Test file read successfully");
            MessageSchema messageSchema = new MessageSchema();
            this.logger.LogInfo("Encoding data started");

            // encoding data with help of imageSchema Encode function
            string encodedata = messageSchema.Encode(testDictionary);
            this.logger.LogInfo("Data encoding completed");

            // decoding data with help of imageSchema Encode function
            this.logger.LogInfo("Inserting some random string into encoded data");
            Random rnd = new Random();
            int randomIndex = rnd.Next(1, encodedata.Length);
            encodedata = encodedata.Substring(0, randomIndex) + "noise" + encodedata.Substring(randomIndex);
            this.logger.LogInfo("Encoding data started");

            // Successful operation if decoding throws error
            try
            {
                messageSchema.Decode(encodedata, false);
            }
            catch
            {
                this.logger.LogInfo("Corrupted data found, cannot decode.");
                this.logger.LogSuccess("Test successful");
                return true;
            }
            
            // Unsuccessful operation if decoding throws no error
            this.logger.LogError("Test unsuccessful");
            return false;
        }
    }
}
