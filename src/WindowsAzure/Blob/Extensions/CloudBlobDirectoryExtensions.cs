using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzure.Blob.Extensions
{
    /// <summary>
    ///     Cloud blob directory extensions.
    /// </summary>
    public static class CloudBlobDirectoryExtensions
    {
        /// <summary>
        ///     Returns a result segment containing a collection of blob items in the container asynchronously.
        /// </summary>
        /// <param name="blobDirectory">Cloud blob directory.</param>
        /// <param name="useFlatBlobListing">Whether to list blobs in a flat listing, or whether to list blobs hierarchically, by virtual directory.</param>
        /// <param name="blobListingDetails">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails" /> enumeration describing which items to include in the listing.
        /// </param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned at a time, up to the
        ///     per-operation limit of 5000. If this value is <c>null</c>, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="continuationToken">A continuation token returned by a previous listing operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The ID of the acquired lease.
        /// </returns>
        public static Task<BlobResultSegment> ListBlobsSegmentedAsync(
            this CloudBlobDirectory blobDirectory,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobDirectory.BeginListBlobsSegmented(useFlatBlobListing, blobListingDetails, maxResults, continuationToken, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<BlobResultSegment>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobDirectory.EndListBlobsSegmented(result);
                    });
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobDirectory">Cloud blob directory.</param>
        /// <param name="cloudBlobs">List of cloud blobs.</param>
        /// <param name="useFlatBlobListing">Whether to list blobs in a flat listing, or whether to list blobs hierarchically, by virtual directory.</param>
        /// <param name="blobListingDetails">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails" /> enumeration describing which items to include in the listing.
        /// </param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned at a time, up to the
        ///     per-operation limit of 5000. If this value is null, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="continuationToken">Continuation token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        private static Task<List<IListBlobItem>> ListBlobsImplAsync(
            this CloudBlobDirectory blobDirectory,
            List<IListBlobItem> cloudBlobs,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return blobDirectory
                .ListBlobsSegmentedAsync(useFlatBlobListing, blobListingDetails, maxResults, continuationToken, cancellationToken)
                .Then(result =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        cloudBlobs.AddRange(result.Results);

                        // Checks whether maxresults entities has been received
                        if (maxResults.HasValue && cloudBlobs.Count >= maxResults.Value)
                        {
                            return TaskHelpers.FromResult(cloudBlobs.Take(maxResults.Value).ToList());
                        }

                        // Checks whether enumeration has been completed
                        if (result.ContinuationToken != null)
                        {
                            return ListBlobsImplAsync(blobDirectory, cloudBlobs, useFlatBlobListing, blobListingDetails, maxResults, continuationToken, cancellationToken);
                        }

                        return TaskHelpers.FromResult(cloudBlobs);
                    });
        }

        /// <summary>
        ///     Returns a result segment containing a collection of blob items in the container asynchronously.
        /// </summary>
        /// <param name="blobDirectory">Cloud blob directory.</param>
        /// <param name="useFlatBlobListing">Whether to list blobs in a flat listing, or whether to list blobs hierarchically, by virtual directory.</param>
        /// <param name="blobListingDetails">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails" /> enumeration describing which items to include in the listing.
        /// </param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned at a time, up to the
        ///     per-operation limit of 5000. If this value is <c>null</c>, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" />.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobDirectory blobDirectory,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobDirectory, new List<IListBlobItem>(), useFlatBlobListing, blobListingDetails, maxResults, null, cancellationToken);
        }

        /// <summary>
        ///     Returns a result segment containing a collection of blob items in the container asynchronously.
        /// </summary>
        /// <param name="blobDirectory">Cloud blob directory.</param>
        /// <param name="useFlatBlobListing">Whether to list blobs in a flat listing, or whether to list blobs hierarchically, by virtual directory.</param>
        /// <param name="blobListingDetails">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails" /> enumeration describing which items to include in the listing.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" />.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobDirectory blobDirectory,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobDirectory, new List<IListBlobItem>(), useFlatBlobListing, blobListingDetails, null, null, cancellationToken);
        }

        /// <summary>
        ///     Returns a result segment containing a collection of blob items in the container asynchronously.
        /// </summary>
        /// <param name="blobDirectory">Cloud blob directory.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" />.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobDirectory blobDirectory,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobDirectory, new List<IListBlobItem>(), false, BlobListingDetails.None, null, null, cancellationToken);
        }
    }
}