namespace WindowsAzure.Tests.Samples
{
    public class EntityWithInvalidPropertyType
    {
        public string PKey { get; set; }
        public string RKey { get; set; }

        // Non-supported property type
        public Country Country { get; set; }
    }

}
