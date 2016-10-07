using System.ComponentModel;

namespace ConsoleGeocoder
{
    class GeocodeItem
    {
        [DisplayName("MsaCode")]
        public string MsaCode { get; set; }
        [DisplayName("StateCode")]
        public string StateCode { get; set; }
        [DisplayName("CountyCode")]
        public string CountyCode { get; set; }
        [DisplayName("CensusTract")]
        public string CensusTractCode { get; set; }
        [DisplayName("GeocodeSource")]
        public string GeocodeSource { get; set; }


        public string CsvResult()
        {
            return string.Format("{0},{1},{2},{3},{4}", MsaCode, StateCode, CountyCode, CensusTractCode, GeocodeSource);
        }

        public string DefaultHeaders()
        {
            //Could I make this use the DisplayName stuff?
            return "MsaCode,StateCode,CountyCode,CensusTract,GeocodeSource";
        }
    }
}
