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

        public MyInt32Enum Int32Enum { get; set; }

        public MyInt64Enum Int64Enum { get; set; }
        
        public MyInt32Enum? NullableInt32Enum { get; set; }

        public MyInt64Enum? NullableInt64Enum { get; set; }
    }

    public enum MyInt64Enum : long
    {
        NA = -9223372036854775801L,
        NB = -9223372036854775802L,
        A = 9223372036854775801L,
        B = 9223372036854775802L
    }

    public enum MyInt32Enum : int
    {
        NA = -2147483641,
        NB = -2147483642,
        A = 2147483641,
        B = 2147483642
    }
}