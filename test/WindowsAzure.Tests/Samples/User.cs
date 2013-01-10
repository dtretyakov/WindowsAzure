using System;
using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public sealed class User
    {
        internal double InternalField;
        public int PublicField;
        private double _privateField;

        [PartitionKey]
        public string FirstName { get; set; }

        [RowKey]
        public string LastName { get; set; }

        public string PublicProperty { get; set; }

        private float PrivateProperty { get; set; }

        public Int64 PrivateSetterProperty { get; private set; }

        public string PrivateGetterProperty { private get; set; }

        internal Int64 InternalProperty { get; set; }

        public string PublicMethod()
        {
            return string.Empty;
        }

        private int PrivateMethod()
        {
            return 222;
        }

        internal bool InternalMethod()
        {
            return false;
        }
    }
}