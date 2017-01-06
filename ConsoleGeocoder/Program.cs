using Gtus.Etools.FairLender.GeocodingService.GeocodeServices;
using Gtus.Etools.FairLender.GeocodingService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ConsoleGeocoder
{
    internal class Program
    {
        private static void Main()
        {
            const string consoleLineBreak = "\r\n";

            //Deal with the user and ask them what file to process
            Console.Title = "..::FFIEC Console Geocoder::..";
            Console.WriteLine("Welcome to the FFIEC Console Geocoder written by Zac Taylor");
            Console.WriteLine(consoleLineBreak);
            Console.WriteLine("Please enter the full path of the '.csv' file that you would like to geocode or you can drag and drop the file into this window.");
            var readingFile = Console.ReadLine();
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
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(readingFile);
            if (proceed != "Y" && proceed != "y")
            {
                Console.WriteLine(consoleLineBreak);
                Console.Write("You said you didn't want to proceed so this tool will now exit.");
                CountDownToClose(5, true);
            }
            else //You entered a Y so let's do this stuff!
            {
                bool standardize = false;
                Console.WriteLine(consoleLineBreak);
                Console.WriteLine("Would you like to attempt to standardize the address in the file '" + readingFile + "'?");
                Console.WriteLine("This process will validate the addresses supplied against the Google Maps API.");
                Console.WriteLine("If the address suggested by Google will be used if available.");
                Console.WriteLine("We will list when this occurs in the output file for your reference.");
                Console.WriteLine("If so press 'Y' to proceed");
                proceed = Console.ReadLine();
                if (proceed == "Y" | proceed == "y")
                {
                    Console.WriteLine(consoleLineBreak);
                    Console.Write("We will attempt to standardize each address against Google Maps API's.");
                    standardize = true;
                }

                if (fileNameWithoutExtension != null)
                {
                    var writingFile = Path.Combine(path1: Path.GetDirectoryName(readingFile), path2: fileNameWithoutExtension + "_Results.csv");
                    var results = new StringBuilder();

                    int counter = 0;
                    Console.WriteLine("The geocoding process will now begin");
                    using (CsvFileReader reader = new CsvFileReader(readingFile))
                    {
                        CsvRow row = new CsvRow();
                        while (reader.ReadRow(row))
                        {
                            //process the column titles
                            if (counter == 0)
                            {
                                results.AppendLine("\"" + row[0] + "\",\"" + row[1] + "\",\"" + row[2] + "\",\"" + row[3] + "\",\"" + row[4] + "\",\"" + row[5] + "\",MSACode,StateCode,CountyCode,CensusTract,Sources");
                                File.WriteAllText(writingFile, results.ToString());
                                counter++;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(row[2]) && !string.IsNullOrEmpty(row[3]) && !string.IsNullOrEmpty(row[4]) && !string.IsNullOrEmpty(row[5]))
                                {
                                    GeocodeAddress address = new GeocodeAddress(row[2], row[3], row[4], row[5], standardize);
                                    List<IGeocodeService> geocodeServiceOrder = new List<IGeocodeService>
                                    {
                                        new GeocodeServiceFfiec(),
                                        new GeocodeServiceCensusDotGov(),
                                        new GeocodeServiceFcc()
                                    };

                                    GeocodeItem geoItem = new GeocodeItem();
                                    foreach (IGeocodeService geoServ in geocodeServiceOrder)
                                    {
                                        geoItem = geoServ.GetGeocode(address);
                                        if (geoItem != null)
                                        {
                                            if (geoItem.IsValid())
                                            {
                                                break;
                                            }
                                        }

                                    }

                                    if (geoItem != null)
                                    {
                                        results.AppendLine("\"" + row[0] + "\",\"" + row[1] + "\"," + address.CsvResult() + geoItem.CsvResult());

                                        Console.WriteLine(counter.ToString("0000000") + "\t\t" + row[0].PadLeft(12, '0') + "\t\t" + geoItem.GeocodeSource);
                                    }

                                    File.WriteAllText(writingFile, results.ToString());
                                    counter++;
                                }
                                else
                                {
                                    while (row.Count < 6) //it didn't pick up the last parameter normally because it was blank
                                    {
                                        row.Add("");
                                    }
                                    results.AppendLine("\"" + row[0] + "\",\"" + row[1] + "\"," + string.Format("{0},{1},{2},{3},", row[2], row[3], row[4], row[5]) + ",,,");

                                    Console.WriteLine(counter.ToString("0000000") + "\t\t" + row[0].PadLeft(12, '0') + "\t\t" + "Invalid Address");

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
                }

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
