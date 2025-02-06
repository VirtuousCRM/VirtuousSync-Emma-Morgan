using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sync
{
    public class AbbreviatedContact
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ContactType { get; set; }

        public string ContactName { get; set; }

        public string Website { get; set; }

        public Address Address { get; set; }

    }

    public class Address
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("postal")]
        public string Postal { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }

}
