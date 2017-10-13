using Xunit;

namespace WindowsAzure.Tests.Attributes
{
    /// <summary>
    ///     Skips tests when runned outside cloud environment.
    /// </summary>
    public sealed class IntegrationFactAttribute : FactAttribute
    {
#if NOCLOUDSTORAGE
        public IntegrationFactAttribute()
        {
            Skip = "Skipped integration test";
        }
#endif
    }
}