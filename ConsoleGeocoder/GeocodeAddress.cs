using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGeocoder
{
    class GeocodeAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public GeocodeAddress(string street, string city, string state, string zip)
        {
            Street = street;
            City = city;
            State = state;
            Zip = zip;
        }

        public string OneLineAddress()
        {
            return string.Format("{0} {1}, {2} {3}", Street, City, State, Zip);
        }
    }
}
