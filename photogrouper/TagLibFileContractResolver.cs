using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Managers
{
    class TagLibFileContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            properties = properties.Where(x => !ignoreProperties.Contains(x.PropertyName)).ToList();

            return properties;
        }


        private static readonly List<string> ignoreProperties = new List<string>()
        {
            "Structure",
            "Data"
        };
    }
}
