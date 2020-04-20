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
// <copyright file="Program.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//      This is the executable used to demo Telemetry and Diagnostics.
// </summary>
//-----------------------------------------------------------------------

namespace DemoDiagnosticsAndTelemetry
{    
    using System;
    using System.Globalization;
    using Masti.QualityAssurance;

    /// <summary>
    /// Program to demonstrate diagnostics and telemetry.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Function where execution starts.
        /// </summary>
        /// <param name="args">Command line arguments if any.</param>
        public static void Main(string[] args)
        {
            // Get ready to sell some fruits.
            Fruit fruit = new Fruit();

            // Sell some fruits.
            try
            {
                Console.WriteLine("How many fruits do you want to sell?");
                var soldQuantity = int.Parse(Console.ReadLine());
                fruit.SellFruit(soldQuantity);
                MastiDiagnostics.LogSuccess(string.Format(CultureInfo.CurrentCulture, "Sold {0} fruits.", soldQuantity));
            }
            catch (Exception e)
            {
                // For exceptions that you can handle, use LogWarning.
                // For exceptions that you cannot handle, use LogError.
                // Use LogSuccess to log successful execution of operations.
                // Use LogInfo to record other information.
                // What logs need to be committed depends on the settings in 
                // QualityAssurance.Properties.Settings
                // This ensures that log is always committed to file.
                MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Failed to sell. {0}", e.Message));
                Console.WriteLine("Failed to sell fruits!!");
            }

            // Sell some more fruits.
            try
            {
                Console.WriteLine("How many fruits do you want to sell now?");
                var soldQuantity = int.Parse(Console.ReadLine());
                fruit.SellFruit(soldQuantity);
                MastiDiagnostics.LogSuccess(string.Format(CultureInfo.CurrentCulture, "Sold {0} fruits.", soldQuantity));
            }
            catch (Exception e)
            {                
                MastiDiagnostics.LogError(string.Format(CultureInfo.CurrentCulture, "Failed to sell. {0}", e.Message));
                Console.WriteLine("Failed to sell fruits!!");
            }

            // This method commits logs to file.
            // You are not required to use this explicitly anywhere.
            MastiDiagnostics.WriteLog();

            // That's enough. Now commit telemetry info.
            // Info stored by all modules get committed.
            // This needs to be called by module which handles Masti's exit (UI).
            // Others needn't.
            TelemetryCollector.Instance.StoreTelemetry();

            MastiDiagnostics.LogInfo("Closing shop. Go elsewhere.");
            
            // To illustrate mailing facility, let's mail the logs to the specified developer.
            // Mailing details can also be changed in the settings file under QualityAssurance.Properties.
            MastiDiagnostics.MailLog();
        }
    }
}
