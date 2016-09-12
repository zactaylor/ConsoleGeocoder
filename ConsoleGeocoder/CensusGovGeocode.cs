using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGeocoder
{
    class CensusGovGeocode
    {
        public class result
        {
            public class input
            {
                public class address
                {
                    public string state { get; set; }
                    public string city { get; set; }
                    public string street { get; set; }
                }

                public class benchmark
                {
                    public string id { get; set; }
                    public string isDefault { get; set; }
                    public string benchmarkDescription { get; set; }
                    public string benchmarkName { get; set; }
                }

                public class vintage
                {
                    public string id { get; set; }
                    public string isDefault { get; set; }
                    public string vintageName { get; set; }
                    public string vintageDescription { get; set; }

                }
            }

            public class addressMatches
            {
                public class geographies
                {
                    //[Display(Name="Census Tracts")]
                    public class CensusTracts
                    {
                        public string OID { get; set; }
                        public string STATE { get; set; }
                        public string FUNCSTAT { get; set; }
                        public string NAME { get; set; }
                        public string AREAWATER { get; set; }
                        public string LSADC { get; set; }
                        public string CENTLON { get; set; }
                        public string BASENAME { get; set; }
                        public string INTPTLAT { get; set; }
                        public string MTFCC { get; set; }
                        public string COUNTY { get; set; }
                        public string GEOID { get; set; }
                        public string CENTLAT { get; set; }
                        public string INTPTLON { get; set; }
                        public string AREALAND { get; set; }
                        public string OBJECTID { get; set; }
                        public string TRACT { get; set; }
                    }

                        
                    }
                public string matchedAddress { get; set; }
                public class coordinates
                {
                    public string x { get; set; }
                    public string y { get; set; }
                }
                public class tigerLine
                {
                    public string tigetLineId { get; set; }
                    public string side {get; set;}
                }
                public class addressComponents
                {
                    public string state { get; set; }
                    public string fromAddress { get; set; }
                    public string toAddress { get; set; }
                    public string preQualifier { get; set; }
                    public string preDirection { get; set; }
                    public string preType { get; set; }
                    public string streetName { get; set; }
                    public string suffixType { get; set; }
                    public string suffixDirection { get; set; }
                    public string suffixQualifier { get; set; }
                    public string zip { get; set; }
                    public string city { get; set; }
                }
            }

        }
        



        //public test() {
        //    var obj = JObject.Parse(geoString);
        //    foreach (var fix in (from property in obj.Descendants().OfType<JProperty>()
        //                         let newName = XmlConvert.EncodeLocalName(property.Name.Replace(" ", ""))
        //                         where newName != property.Name
        //                         select new { Old = property, New = new JProperty(newName, property.Value) })
        //               .ToList())
        //    {
        //        fix.Old.Replace(fix.New);
        //    }

        //    var xmldoc = JsonConvert.DeserializeXmlNode(obj.ToString());
        //}
    }
}
