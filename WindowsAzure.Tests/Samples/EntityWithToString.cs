namespace WindowsAzure.Tests.Samples
{
    public sealed class EntityWithToString
    {
        private readonly string _value;

        public EntityWithToString(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}