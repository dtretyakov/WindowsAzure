namespace WindowsAzure.Table.Queryable
{
    /// <summary>
    /// An enumeration of table query segments.
    /// </summary>
    public enum QuerySegment
    {
        /// <summary>
        /// Filter segment.
        /// </summary>
        Filter,

        /// <summary>
        /// Select segment.
        /// </summary>
        Select,

        /// <summary>
        /// Top segment.
        /// </summary>
        Top
    }
}