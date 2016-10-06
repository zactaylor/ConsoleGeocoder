using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGeocoder.CensusModel
{
    public class Result
    {
        [JsonProperty(PropertyName = "input")]
        public Input Input { get; set; }
        [JsonProperty(PropertyName = "addressMatches")]
        public List<AddressMatches> AddressMatches = new List<AddressMatches>();


    }
    public class Input
    {
        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }
        [JsonProperty(PropertyName = "benchmark")]
        public Benchmark Benchmark { get; set; }
        [JsonProperty(PropertyName = "vintage")]
        public Vintage Vintage { get; set; }
    }
    public class Address
    {
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }
    }

    public class Benchmark
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "isDefault")]
        public string IsDefault { get; set; }
        [JsonProperty(PropertyName = "benchmarkDescription")]
        public string BenchmarkDescription { get; set; }
        [JsonProperty(PropertyName = "benchmarkName")]
        public string BenchmarkName { get; set; }
    }

    public class Vintage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "isDefault")]
        public string IsDefault { get; set; }
        [JsonProperty(PropertyName = "vintageName")]
        public string VintageName { get; set; }
        [JsonProperty(PropertyName = "vintageDescription")]
        public string VintageDescription { get; set; }

    }
    public class Geographies
    {
        [JsonProperty(PropertyName = "Census Tracts")]
        public List<CensusTracts> CensusTracts = new List<CensusTracts>();

    }
    public class CensusTracts
    {
        [JsonProperty(PropertyName = "OID")]
        public string OID { get; set; }

        [JsonProperty(PropertyName = "STATE")]
        public string STATE { get; set; }

        [JsonProperty(PropertyName = "FUNCSTAT")]
        public string FUNCSTAT { get; set; }

        [JsonProperty(PropertyName = "NAME")]
        public string NAME { get; set; }

        [JsonProperty(PropertyName = "AREAWATER")]
        public string AREAWATER { get; set; }

        [JsonProperty(PropertyName = "LSADC")]
        public string LSADC { get; set; }

        [JsonProperty(PropertyName = "CENTLON")]
        public string CENTLON { get; set; }

        [JsonProperty(PropertyName = "BASENAME")]
        public string BASENAME { get; set; }

        [JsonProperty(PropertyName = "INTPTLAT")]
        public string INTPTLAT { get; set; }

        [JsonProperty(PropertyName = "MTFCC")]
        public string MTFCC { get; set; }

        [JsonProperty(PropertyName = "COUNTY")]
        public string COUNTY { get; set; }

        [JsonProperty(PropertyName = "GEOID")]
        public string GEOID { get; set; }

        [JsonProperty(PropertyName = "CENTLAT")]
        public string CENTLAT { get; set; }

        [JsonProperty(PropertyName = "INTPTLON")]
        public string INTPTLON { get; set; }

        [JsonProperty(PropertyName = "AREALAND")]
        public string AREALAND { get; set; }

        [JsonProperty(PropertyName = "OBJECTID")]
        public string OBJECTID { get; set; }

        [JsonProperty(PropertyName = "TRACT")]
        public string TRACT { get; set; }
    }
    public class Coordinates
    {
        [JsonProperty(PropertyName = "x")]
        public string x { get; set; }
        [JsonProperty(PropertyName = "y")]
        public string y { get; set; }
    }
    public class TigerLine
    {
        [JsonProperty(PropertyName = "tigerLineId")]
        public string TigerLineId { get; set; }
        [JsonProperty(PropertyName = "side")]
        public string Side { get; set; }
    }
    public class AddressComponents
    {
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "fromAddress")]
        public string FromAddress { get; set; }
        [JsonProperty(PropertyName = "toAddress")]
        public string ToAddress { get; set; }
        [JsonProperty(PropertyName = "preQualifier")]
        public string PreQualifier { get; set; }
        [JsonProperty(PropertyName = "preDirection")]
        public string PreDirection { get; set; }
        [JsonProperty(PropertyName = "preType")]
        public string PreType { get; set; }
        [JsonProperty(PropertyName = "streetName")]
        public string StreetName { get; set; }
        [JsonProperty(PropertyName = "suffixType")]
        public string SuffixType { get; set; }
        [JsonProperty(PropertyName = "suffixDirection")]
        public string SuffixDirection { get; set; }
        [JsonProperty(PropertyName = "suffixQualifier")]
        public string SuffixQualifier { get; set; }
        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
    }
    public class AddressMatches
    {

        public Geographies Geographies { get; set; }
        public string MatchedAddress { get; set; }
        public Coordinates Coordinates { get; set; }
        public TigerLine TigerLine { get; set; }
        public AddressComponents AddressComponents { get; set; }

    }
    public class CensusGovGeocode
    {
        [JsonProperty(PropertyName = "result")]
        public Result Result { get; set; }
    }
}
