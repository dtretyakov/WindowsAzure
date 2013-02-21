using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace WindowsAzure.Blob.Extensions
{
    /// <summary>
    ///     Cloud Blob Client Extensions.
    /// </summary>
    public static class CloudBlobClientExtensions
    {
        /// <summary>
        ///     Gets a reference to a blob in this container asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="blobUri">The URI of the blob.</param>
        /// <param name="accessCondition">
        ///     An object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A reference to the blob.
        /// </returns>
        public static Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            this CloudBlobClient blobClient,
            Uri blobUri,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobClient.BeginGetBlobReferenceFromServer(blobUri, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<ICloudBlob>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobClient.EndGetBlobReferenceFromServer(result);
                    });
        }

        /// <summary>
        ///     Gets a reference to a blob in this container asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="blobUri">The URI of the blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A reference to the blob.
        /// </returns>
        public static Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            this CloudBlobClient blobClient,
            Uri blobUri,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return GetBlobReferenceFromServerAsync(blobClient, blobUri, null, cancellationToken);
        }

        /// <summary>
        ///     Gets the properties of the blob service asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        public static Task<ServiceProperties> GetServicePropertiesAsync(
            this CloudBlobClient blobClient,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobClient.BeginGetServiceProperties(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<ServiceProperties>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobClient.EndGetServiceProperties(result);
                    });
        }

        /// <summary>
        ///     Returns a result segment containing a collection of blob items in the container asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="useFlatBlobListing">Whether to list blobs in a flat listing, or whether to list blobs hierarchically, by virtual directory.</param>
        /// <param name="blobListingDetails">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails" /> enumeration describing which items to include in the listing.
        /// </param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned at a time, up to the
        ///     per-operation limit of 5000. If this value is null, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="continuationToken">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobContinuationToken" /> returned by a previous listing operation.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A result segment containing objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" />.
        /// </returns>
        public static Task<BlobResultSegment> ListBlobsSegmentedAsync(
            this CloudBlobClient blobClient,
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobClient.BeginListBlobsSegmented(prefix, useFlatBlobListing, blobListingDetails, maxResults, continuationToken, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<BlobResultSegment>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobClient.EndListBlobsSegmented(result);
                    });
        }

        /// <summary>
        ///     Returns a result segment containing a collection of blob items in the container asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="continuationToken">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobContinuationToken" /> returned by a previous listing operation.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A result segment containing objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" />.
        /// </returns>
        public static Task<BlobResultSegment> ListBlobsSegmentedAsync(
            this CloudBlobClient blobClient,
            string prefix,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsSegmentedAsync(blobClient, prefix, false, BlobListingDetails.None, null, continuationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="cloudBlobs">List of cloud blobs.</param>
        /// <param name="prefix">The blob name prefix.</param>
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
            this CloudBlobClient blobClient,
            List<IListBlobItem> cloudBlobs,
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return blobClient
                .ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, maxResults, continuationToken, cancellationToken)
                .Then(result =>
                    {
                        cloudBlobs.AddRange(result.Results);

                        BlobContinuationToken continuation = result.ContinuationToken;

                        // Checks whether enumeration has been completed
                        if (continuation != null && maxResults.HasValue &&
                            maxResults.Value < cloudBlobs.Count)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            return ListBlobsImplAsync(blobClient, cloudBlobs, prefix, useFlatBlobListing, blobListingDetails, maxResults, continuationToken, cancellationToken);
                        }

                        return TaskHelpers.FromResult(cloudBlobs);
                    });
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="useFlatBlobListing">Whether to list blobs in a flat listing, or whether to list blobs hierarchically, by virtual directory.</param>
        /// <param name="blobListingDetails">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails" /> enumeration describing which items to include in the listing.
        /// </param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned at a time, up to the
        ///     per-operation limit of 5000. If this value is null, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobClient blobClient,
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobClient, new List<IListBlobItem>(), prefix, useFlatBlobListing, blobListingDetails, maxResults, null, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobClient blobClient,
            string prefix,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobClient, new List<IListBlobItem>(), prefix, false, BlobListingDetails.None, null, null, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobClient blobClient,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobClient, new List<IListBlobItem>(), null, false, BlobListingDetails.None, null, null, cancellationToken);
        }

        /// <summary>
        ///     Returns a result segment containing a collection of containers whose names begin with the specified prefix asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="prefix">The container name prefix.</param>
        /// <param name="detailsIncluded">A value that indicates whether to return container metadata with the listing.</param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned
        ///     in the result segment, up to the per-operation limit of 5000. If this value is null, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="continuationToken">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobContinuationToken" /> returned by a previous listing operation.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A result segment of containers.
        /// </returns>
        public static Task<ContainerResultSegment> ListContainersSegmentedAsync(
            this CloudBlobClient blobClient,
            string prefix,
            ContainerListingDetails detailsIncluded,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobClient.BeginListContainersSegmented(prefix, detailsIncluded, maxResults, continuationToken, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<ContainerResultSegment>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobClient.EndListContainersSegmented(result);
                    });
        }

        /// <summary>
        ///     Returns a result segment containing a collection of containers whose names begin with the specified prefix asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="continuationToken">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Blob.BlobContinuationToken" /> returned by a previous listing operation.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A result segment of containers.
        /// </returns>
        public static Task<ContainerResultSegment> ListContainersSegmentedAsync(
            this CloudBlobClient blobClient,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListContainersSegmentedAsync(blobClient, null, ContainerListingDetails.None, null, continuationToken, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="cloudContainers">List of cloud containers.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="detailsIncluded">A value that indicates whether to return container metadata with the listing.</param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned
        ///     in the result segment, up to the per-operation limit of 5000. If this value is null, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="continuationToken">Continuation token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        private static Task<List<CloudBlobContainer>> ListContainersImplAsync(
            this CloudBlobClient blobClient,
            List<CloudBlobContainer> cloudContainers,
            string prefix,
            ContainerListingDetails detailsIncluded,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return blobClient
                .ListContainersSegmentedAsync(prefix, detailsIncluded, maxResults, continuationToken, cancellationToken)
                .Then(result =>
                    {
                        cloudContainers.AddRange(result.Results);

                        BlobContinuationToken continuation = result.ContinuationToken;

                        // Checks whether enumeration has been completed
                        if (continuation != null && maxResults.HasValue &&
                            maxResults.Value < cloudContainers.Count)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            return ListContainersImplAsync(blobClient, cloudContainers, prefix, detailsIncluded, maxResults, continuationToken, cancellationToken);
                        }

                        return TaskHelpers.FromResult(cloudContainers);
                    });
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="detailsIncluded">A value that indicates whether to return container metadata with the listing.</param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned
        ///     in the result segment, up to the per-operation limit of 5000. If this value is null, the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<CloudBlobContainer>> ListContainersAsync(
            this CloudBlobClient blobClient,
            string prefix,
            ContainerListingDetails detailsIncluded,
            int? maxResults,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListContainersImplAsync(blobClient, new List<CloudBlobContainer>(), prefix, detailsIncluded, maxResults, null, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<CloudBlobContainer>> ListContainersAsync(
            this CloudBlobClient blobClient,
            string prefix,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListContainersImplAsync(blobClient, new List<CloudBlobContainer>(), prefix, ContainerListingDetails.None, null, null, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<CloudBlobContainer>> ListContainersAsync(
            this CloudBlobClient blobClient,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListContainersImplAsync(blobClient, new List<CloudBlobContainer>(), null, ContainerListingDetails.None, null, null, cancellationToken);
        }

        /// <summary>
        ///     Sets the properties of the blob service asynchronously.
        /// </summary>
        /// <param name="blobClient">Cloud blob client.</param>
        /// <param name="serviceProperties">The blob service properties.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        public static Task SetServicePropertiesAsync(
            this CloudBlobClient blobClient,
            ServiceProperties serviceProperties,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobClient.BeginSetServiceProperties(serviceProperties, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobClient.EndSetServiceProperties(result);
                    });
        }
    }
}