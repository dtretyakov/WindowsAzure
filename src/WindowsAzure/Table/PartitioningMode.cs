namespace WindowsAzure.Table
{
    /// <summary>
    ///     Batch partitioning mode.
    /// </summary>
    public enum PartitioningMode
    {
        /// <summary>
        ///     Partitioning with sequential execution.
        /// </summary>
        Sequential,

        /// <summary>
        ///     Partitioning with parallel execution.
        /// </summary>
        Parallel,

        /// <summary>
        ///     No partitioning.
        /// </summary>
        None
    }
}