using Xunit;

namespace WindowsAzure.Tests.Attributes
{
    /// <summary>
    ///     Skips tests when runned outside cloud environment.
    /// </summary>
    public sealed class IntegrationalFactAttribute : FactAttribute
    {
#if NOCLOUDSTORAGE
        public IntegrationalFactAttribute()
        {
            Skip = "Skipped integrational test";
        }
#endif
    }
}