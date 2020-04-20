//-----------------------------------------------------------------------
// <author> 
//    Anish M M (anishmathewdev@gmail.com)
// </author>
//
// <date> 
//     12th November, 2018
// </date>
// 
// <reviewer> 
//
// </reviewer>
// 
// <copyright file="FruitTelemetry.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This is the telemetry class for the Fruit class used to demo Telemetry and Diagnostics.
// </summary>
//-----------------------------------------------------------------------

namespace DemoDiagnosticsAndTelemetry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Masti.QualityAssurance;

    /// <summary>
    /// Class to handle telemetry for Fruit module.
    /// </summary>
    public class FruitTelemetry : ITelemetry
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="FruitTelemetry"/> class.
        /// </summary>
        public FruitTelemetry()
        {
            this.DataCapture.Add("total", "0");
        }

        /// <summary>
        /// Gets or sets DataCapture.
        /// This is the essential part. Should contain the data.
        /// </summary>
        public IDictionary<string, string> DataCapture { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Handle telemetry data manipulation. This is just a sample.
        /// Non-essential. Developers are free to handle data manipulation whichever way they want.
        /// </summary>
        /// <param name="soldQuantity">Number of fruits sold.</param>
        /// <returns>tNew total.</returns>
        public int FindNewTotal(int soldQuantity)
        {
            int total = int.Parse(this.DataCapture["total"]);

            return total + soldQuantity;
        }

        /// <summary>
        /// Non-essential. Developers are free to handle data manipulation whichever way they want.
        /// This is just a sample.
        /// </summary>
        /// <param name="soldQuantity">Number of fruits sold.</param>
        public void Calculate(int soldQuantity)
        {
            this.DataCapture["total"] = this.FindNewTotal(soldQuantity).ToString();
        }
    }
}
