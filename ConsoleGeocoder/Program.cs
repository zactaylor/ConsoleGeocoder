using ConsoleGeocoder.CensusModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleGeocoder
{
    class Program
    {
        static void Main(string[] args)
        {
            string readingFile = "";
            string writingFile = "";
            string consoleLineBreak = "\r\n";

            //Deal with the user and ask them what file to process
            Console.Title = "..::FFIEC Console Geocoder::..";
            Console.WriteLine("Welcome to the FFIEC Console Geocoder written by Zac Taylor");
            Console.WriteLine(consoleLineBreak);
            Console.WriteLine("Please enter the full path of the '.csv' file that you would like to geocode or you can drag and drop the file into this window.");
            readingFile = Console.ReadLine();
            if (Path.GetExtension(readingFile) != ".csv")
            {
                Console.WriteLine(consoleLineBreak);
                Console.WriteLine("You didn't enter a .csv file so this tool will now exit.");
                CountDownToClose(5, true);
            }

            Console.WriteLine(consoleLineBreak);
            Console.WriteLine("You entered the file path as: '" + readingFile + "'.");
            Console.WriteLine("Is that file correct?  If so press 'Y' to proceed");
            string proceed = Console.ReadLine();
            if (proceed != "Y" && proceed != "y")
            {
                Console.WriteLine(consoleLineBreak);
                Console.Write("You said you didn't want to proceed so this tool will now exit.");
                CountDownToClose(5, true);
            }
            else //You entered a Y so let's do this stuff!
            {
                writingFile = Path.Combine(Path.GetDirectoryName(readingFile), Path.GetFileNameWithoutExtension(readingFile).ToString() + "_Results.csv");
                var results = new StringBuilder();

                int counter = 0;
                if (readingFile != null && writingFile != null)
                {
                    Console.WriteLine("The geocoding process will now begin");
                    using (CsvFileReader reader = new CsvFileReader(readingFile))
                    {
                        CsvRow row = new CsvRow();
                        while (reader.ReadRow(row))
                        {
                            //process the column titles
                            if (counter == 0)
                            {
                                results.AppendLine("\"" + row[0] + "\",\"" + row[1] + "\",\"" + row[2] + "\",\"" + row[3] + "\",\"" + row[4] + "\",\"" + row[5] + "\",MSACode,StateCode,CountyCode,CensusTract,Source,MSACode,StateCode,CountyCode,CensusTract,Source");
                                File.WriteAllText(writingFile, results.ToString());
                                counter++;
                            }
                            else
                            {
                                GeocodeAddress address = new GeocodeAddress(row[2], row[3], row[4], row[5]);

                                ////Geocode FFIEC
                                GeocodeItem geocodeFFIEC = GeocodeServiceFFIEC.GetGeocode(address);

                                ////Geocode Census.Gov
                                GeocodeItem geocodeCensus = GeocodeServiceCensusDotGov.GetGeocode(address);

                                results.AppendLine("\""+row[0] + "\",\"" + row[1] + "\",\"" + row[2] + "\",\"" + row[3] + "\",\"" + row[4] + "\",\"" + row[5] + "\"," + geocodeFFIEC.CsvResult() + "," + geocodeCensus.CsvResult());

                                
                                Console.WriteLine(counter.ToString("0000000") + ":\t" + string.Format("00000000000000000", row[0]) + '\t' + geocodeFFIEC.GeocodeSource + '\t' + geocodeCensus.GeocodeSource);
                                //Thread.Sleep(2000);

                                File.WriteAllText(writingFile, results.ToString());
                                counter++;
                            }
                        }
                    }
                }

                Console.WriteLine(consoleLineBreak);
                Console.Write("This tool has completed processing your file '" + writingFile + "' and it will now be opened for your review.  Thank you.");

                //we are done so let's open the file:
                Process.Start(writingFile);

                CountDownToClose(5, true);
            }
        }

       

        public static void CountDownToClose(int durationInSeconds, bool exitApplicationAtEnd = false)
        {
            Console.WriteLine("\r\n");
            Console.WriteLine("This tool will close in...");
            for (int i = 0; i <= durationInSeconds; i++)
            {
                Console.WriteLine((durationInSeconds - i) + " seconds");
                Thread.Sleep(1000);
            }
            if (exitApplicationAtEnd)
            {
                Environment.Exit(0);    //Close the application
            }
        }
    }
}
