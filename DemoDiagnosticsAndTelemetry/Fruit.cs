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
// <copyright file="Fruit.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This is a module used to demo Telemetry and Diagnostics.
// </summary>
//-----------------------------------------------------------------------

namespace DemoDiagnosticsAndTelemetry
{
    using System;
    using System.Globalization;
    using Masti.QualityAssurance;


    /// <summary>
    /// Class used to sell fruits as a demo functionality.
    /// </summary>
    public class Fruit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fruit"/> class.
        /// </summary>
        public Fruit()
        {
            // Get instance which stores telemetry data across all modules. 
            ITelemetryCollector fruitTelemetryCollector = TelemetryCollector.Instance;

            // Register telemetry.
            fruitTelemetryCollector.RegisterTelemetry("FruitTelemetry", new FruitTelemetry());
        }

        /// <summary>
        /// Sell specified number of fruits.
        /// </summary>
        /// <param name="soldQuantity">Number of fruits to be sold.</param>
        public void SellFruit(int soldQuantity)
        {
            if (soldQuantity < 0)
            {
                throw new ArgumentException();
            }
            // This is the intended functionality of this method.
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} fruits sold.", soldQuantity));

            // Get instance which stores telemetry data across all modules. 
            ITelemetryCollector fruitTelemetryCollector = TelemetryCollector.Instance;
            
            // Extract your telemetry data from the collector.
            FruitTelemetry fruitTelemetry = (FruitTelemetry)fruitTelemetryCollector.GetTelemetryObject("FruitTelemetry");
            
            // Here, use whatever method you have created to update telemetry information.
            // Finally, telemetry data should be stored in the dataCapture property. 
            // Everything else is flexible.
            fruitTelemetry.Calculate(soldQuantity);
        }
    }
}
