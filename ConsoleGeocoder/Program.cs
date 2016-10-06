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
            Console.WriteLine("Please enter the full path of the '.csv' file that you would like to geocode");
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
                writingFile = Path.GetFileNameWithoutExtension(readingFile).ToString() + "_Results.csv";
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
                                string fullAddress = row[2] + " " + row[3] + ", " + row[4] + " " + row[5];

                                //Geocode FFIEC
                                string[] geocodeFFIEC = new string[5];
                                string currentYear = DateTime.Now.Year.ToString();
                                geocodeFFIEC = GetGeocodeFFIEC(fullAddress, currentYear);

                                //Geocode Census.Gov
                                string[] geocodeCensus = new string[5];
                                geocodeCensus = GetGeocodeCensusDotGov(row[2], row[3], row[4], row[5]);

                                results.AppendLine("\""+row[0] + "\",\"" + row[1] + "\",\"" + row[2] + "\",\"" + row[3] + "\",\"" + row[4] + "\",\"" + row[5] + "\"," + geocodeFFIEC[0] + "," + geocodeFFIEC[1] + "," + geocodeFFIEC[2] + "," + geocodeFFIEC[3] + "," + geocodeFFIEC[4] + "," + geocodeCensus[0] + "," + geocodeCensus[1] + "," + geocodeCensus[2] + "," + geocodeCensus[3] + "," + geocodeCensus[4]);

                                
                                Console.WriteLine(counter.ToString("0000000") + ": " + row[0] + ' ' + geocodeFFIEC[4] + ' ' + geocodeCensus[4]);
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

        public static string[] GetGeocodeFFIEC(string address, string year)
        {
            string[] result = new string[5];

            if (!String.IsNullOrEmpty(address) && !String.IsNullOrEmpty(year))
            {

                string URLAuth = "https://geomap.ffiec.gov/FFIECGeocMap/GeocodeMap1.aspx/GetGeocodeData";
                string json = "{\"sSingleLine\":\"" + address + "\"," +
                                  "\"iCensusYear\":\"" + year + "\"}";

                const string contentType = "application/json; charset=UTF-8";
                System.Net.ServicePointManager.Expect100Continue = false;

                CookieContainer cookies = new CookieContainer();
                HttpWebRequest webRequest = WebRequest.Create(URLAuth) as HttpWebRequest;
                webRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                webRequest.Method = "POST";
                webRequest.ContentType = contentType;
                webRequest.CookieContainer = cookies;
                webRequest.ContentLength = json.Length;
                webRequest.Host = "geomap.ffiec.gov";

                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.130 Safari/537.36";
                webRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                webRequest.Referer = "https://geomap.ffiec.gov/FFIECGeocMap/GeocodeMap1.aspx";

                using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {


                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)webRequest.GetResponse();

                Stream receiveStream = myHttpWebResponse.GetResponseStream();
                StreamReader readStream = null;

                if (myHttpWebResponse.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(myHttpWebResponse.CharacterSet));

                string data = readStream.ReadToEnd();
                dynamic geocodeInfo = JsonConvert.DeserializeObject(data);

                var info = geocodeInfo.d;

                if (info.sMsg == "Match Found.")
                {
                    //found programmatically form FFIEC website
                    result[0] = info.sMSACode;
                    result[1] = info.sStateCode;
                    result[2] = info.sCountyCode;
                    result[3] = info.sTractCode;
                    result[4] = "FFIEC";
                }
                else if (info.sMsg == "No Match.")
                {
                    //NOT found programmatically form FFIEC website
                    result[0] = "";
                    result[1] = "";
                    result[2] = "";
                    result[3] = "";
                    result[4] = "Geocode Not Found";
                }
                else if (info.sMsg == "Low Score")
                {
                    //NOT found programmatically form FFIEC website
                    result[0] = "";
                    result[1] = "";
                    result[2] = "";
                    result[3] = "";
                    result[4] = "Address Not Found";
                }
                else
                {
                    result[0] = "";
                    result[1] = "";
                    result[2] = "";
                    result[3] = "";
                    result[4] = "?";
                }
                myHttpWebResponse.Close();
                readStream.Close();

                myHttpWebResponse.Close();
            }
            return result;
        }

        public static string[] GetGeocodeCensusDotGov(string street, string city, string state, string zip)
        {
            string[] result = new string[5];

            if (!string.IsNullOrEmpty(street) && !string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(state))
            {
                string searchParameters = "geographies/address?street=" + street + "&city=" + city + "&state=" + state + "&zip=" + zip + "&benchmark=Public_AR_Current&vintage=Current_Current&layers=8&format=json";

                string URLAuth = "https://geocoding.geo.census.gov/geocoder/" + searchParameters;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                using (WebClient webClient = new WebClient())
                {
                    var json = webClient.DownloadString(URLAuth);
                    string valueOriginal = Convert.ToString(json);
                    CensusGovGeocode geo = JsonConvert.DeserializeObject<CensusGovGeocode>(json);
                    //dynamic geocodeInfo = JsonConvert.DeserializeObject(json);
                    
                    if (geo.Result?.AddressMatches.Any() ?? false)
                    {
                        var tractInfo = geo.Result.AddressMatches.FirstOrDefault().Geographies.CensusTracts.FirstOrDefault();
                        result[0] = tractInfo.OBJECTID;
                        result[1] = tractInfo.STATE;
                        result[2] = tractInfo.COUNTY;
                        result[3] = tractInfo.BASENAME;
                        result[4] = "Census.gov";
                    } else
                    {
                        //NOT found programmatically form FFIEC website
                        result[0] = "";
                        result[1] = "";
                        result[2] = "";
                        result[3] = "";
                        result[4] = "Address Not Found";
                    }
                }

            }
            return result;
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
