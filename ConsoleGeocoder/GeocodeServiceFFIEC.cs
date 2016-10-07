using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ConsoleGeocoder
{
    class GeocodeServiceFFIEC
    {
        public static GeocodeItem GetGeocode(GeocodeAddress address, string year = "")
        {
            GeocodeItem geocodeItem = new GeocodeItem();

            if (String.IsNullOrEmpty(year)) {
                year = DateTime.Now.Year.ToString();
            }

            if (!String.IsNullOrEmpty(address.OneLineAddress()))
            {

                string URLAuth = "https://geomap.ffiec.gov/FFIECGeocMap/GeocodeMap1.aspx/GetGeocodeData";
                string json = "{\"sSingleLine\":\"" + address.OneLineAddress() + "\"," +
                                  "\"iCensusYear\":\"" + year + "\"}";

                const string contentType = "application/json; charset=UTF-8";
                ServicePointManager.Expect100Continue = false;

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
                    geocodeItem.MsaCode = info.sMSACode;
                    geocodeItem.StateCode =  info.sStateCode;
                    geocodeItem.CountyCode = info.sCountyCode;
                    geocodeItem.CensusTractCode = info.sTractCode;
                    geocodeItem.GeocodeSource = "FFIEC";
                }
                else if (info.sMsg == "No Match.")
                {
                    //NOT found programmatically form FFIEC website
                    geocodeItem.GeocodeSource = "Geocode Not Found";
                }
                else if (info.sMsg == "Low Score")
                {
                    //NOT found programmatically form FFIEC website
                    geocodeItem.GeocodeSource = "Address Not Found";
                }
                else
                {
                    geocodeItem.GeocodeSource = "?";
                }
                myHttpWebResponse.Close();
                readStream.Close();

                myHttpWebResponse.Close();
            }
            return geocodeItem;
        }
    }
}
