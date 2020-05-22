using System.Collections.Generic;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public class SerializerTestBase
    {
        /// <summary>
        /// Add custom serializer for serialization tests. 
        /// </summary>
        public static IEnumerable<object[]> SerializersMemberData =>
          new List<object[]>
          {
                new[] { new NewtonsoftJsonSerializer() },
          };
    }
}
