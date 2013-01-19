using System;

namespace WindowsAzure.Tests.Samples
{
    public sealed class EntityWithProperties
    {
        public Boolean Boolean { get; set; }

        public Byte[] Binary { get; set; }

        public DateTime DateTime { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public Double Double { get; set; }

        public Guid Guid { get; set; }

        public Int32 Int32 { get; set; }

        public Int64 Int64 { get; set; }

        public String String { get; set; }

        public Boolean? NullableBoolean { get; set; }

        public DateTime? NullableDateTime { get; set; }

        public DateTimeOffset? NullableDateTimeOffset { get; set; }

        public Double? NullableDouble { get; set; }

        public Guid? NullableGuid { get; set; }

        public Int32? NullableInt32 { get; set; }

        public Int64? NullableInt64 { get; set; }

        public Single Single { get; set; }
    }
}