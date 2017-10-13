using Xunit;

namespace WindowsAzure.Tests.Attributes
{
    /// <summary>
    ///     Skips tests when runned outside cloud environment.
    /// </summary>
    public sealed class IntegrationFactAttribute : FactAttribute
    {
        private string _skip = "Skipped integration test";
        
#if CLOUDSTORAGE
        public IntegrationFactAttribute()
        {
            _skip = null
        }
#endif
        
        public override string Skip
        {
            get { return _skip; }
            set { _skip = value; }
        }
    }
}