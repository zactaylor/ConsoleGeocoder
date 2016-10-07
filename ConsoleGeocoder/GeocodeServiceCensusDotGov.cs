using ConsoleGeocoder.CensusModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGeocoder
{
    class GeocodeServiceCensusDotGov
    {
        public static GeocodeItem GetGeocode(GeocodeAddress address)
        {
            GeocodeItem geocodeItem = new GeocodeItem();

            if (!string.IsNullOrEmpty(address.Street) && !string.IsNullOrEmpty(address.City) && !string.IsNullOrEmpty(address.State) && !string.IsNullOrEmpty(address.Zip))
            {
                string searchParameters = "geographies/address?street=" + address.Street + "&city=" + address.City + "&state=" + address.State + "&zip=" + address.Zip + "&benchmark=Public_AR_Current&vintage=Current_Current&layers=8&format=json";

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
                        geocodeItem.MsaCode = tractInfo.OBJECTID;
                        geocodeItem.StateCode = tractInfo.STATE;
                        geocodeItem.CountyCode = tractInfo.COUNTY;
                        geocodeItem.CensusTractCode = tractInfo.BASENAME;
                        geocodeItem.GeocodeSource = "Census.gov";
                    }
                    else
                    {
                        //NOT found programmatically form FFIEC website
                        geocodeItem.GeocodeSource = "Address Not Found";
                    }
                }

            }
            return geocodeItem;
        }
    }
}
