using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzure.Blob.Extensions
{
    /// <summary>
    ///     Cloud Blob Container Extensions.
    /// </summary>
    public static class CloudBlobContainerExtensions
    {
        /// <summary>
        ///     Acquires a lease on this container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="leaseTime">
        ///     A <see cref="T:System.TimeSpan" /> representing the span of time for which to acquire the lease,
        ///     which will be rounded down to seconds. If <c>null</c>, an infinite lease will be acquired. If not null, this must be
        ///     greater than zero.
        /// </param>
        /// <param name="proposedLeaseId">A string representing the proposed lease ID for the new lease, or null if no lease ID is proposed.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The ID of the acquired lease.
        /// </returns>
        public static Task<string> AcquireLeaseAsync(
            this CloudBlobContainer blobContainer,
            TimeSpan? leaseTime,
            string proposedLeaseId,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginAcquireLease(leaseTime, proposedLeaseId, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<string>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndAcquireLease(result);
                    });
        }

        /// <summary>
        ///     Breaks the current lease on this container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="breakPeriod">
        ///     A <see cref="T:System.TimeSpan" /> representing the amount of time to allow the lease to remain,
        ///     which will be rounded down to seconds. If <c>null</c>, the break period is the remainder of the current lease,
        ///     or zero for infinite leases.
        /// </param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:System.TimeSpan" /> representing the amount of time before the lease ends, to the second.
        /// </returns>
        public static Task<TimeSpan> BreakLeaseAsync(
            this CloudBlobContainer blobContainer,
            TimeSpan? breakPeriod = null,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginBreakLease(breakPeriod, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<TimeSpan>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndBreakLease(result);
                    });
        }

        /// <summary>
        ///     Changes the lease ID on this container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="proposedLeaseId">A string representing the proposed lease ID for the new lease. This cannot be null.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container, including a required lease ID.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The new lease ID.
        /// </returns>
        public static Task<string> ChangeLeaseAsync(
            this CloudBlobContainer blobContainer,
            string proposedLeaseId,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginChangeLease(proposedLeaseId, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<string>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndChangeLease(result);
                    });
        }

        /// <summary>
        ///     Creates the container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task CreateAsync(
            this CloudBlobContainer blobContainer,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginCreate(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobContainer.EndCreate(result);
                    });
        }

        /// <summary>
        ///     Creates the container if it does not already exist asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the container did not already exist and was created; otherwise <c>false</c>.
        /// </returns>
        public static Task<bool> CreateIfNotExistsAsync(
            this CloudBlobContainer blobContainer,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginCreateIfNotExists(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndCreateIfNotExists(result);
                    });
        }

        /// <summary>
        ///     Deletes the container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task DeleteAsync(
            this CloudBlobContainer blobContainer,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginDelete(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobContainer.EndDelete(result);
                    });
        }

        /// <summary>
        ///     Deletes the container if it already exists asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the container did not already exist and was created; otherwise <c>false</c>.
        /// </returns>
        public static Task<bool> DeleteIfExistsAsync(
            this CloudBlobContainer blobContainer,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginDeleteIfExists(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndDeleteIfExists(result);
                    });
        }

        /// <summary>
        ///     Checks existence of the container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the container exists.
        /// </returns>
        public static Task<bool> ExistsAsync(
            this CloudBlobContainer blobContainer,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginExists(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndExists(result);
                    });
        }

        /// <summary>
        ///     Retrieves the container's attributes asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task FetchAttributesAsync(
            this CloudBlobContainer blobContainer,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginFetchAttributes(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobContainer.EndFetchAttributes(result);
                    });
        }

        /// <summary>
        ///     Gets a reference to a blob in this container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A reference to the blob.
        /// </returns>
        public static Task<ICloudBlob> GetBlobReferenceFromServer(
            this CloudBlobContainer blobContainer,
            string blobName,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginGetBlobReferenceFromServer(blobName, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<ICloudBlob>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndGetBlobReferenceFromServer(result);
                    });
        }

        /// <summary>
        ///     Gets the permissions settings for the container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The container's permissions.
        /// </returns>
        public static Task<BlobContainerPermissions> GetPermissionsAsync(
            this CloudBlobContainer blobContainer,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginGetPermissions(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<BlobContainerPermissions>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndGetPermissions(result);
                    });
        }

        /// <summary>
        ///     Returns a result segment containing a collection of blob items in the container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="prefix">The blob name prefix.</param>
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
        ///     A result segment containing objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" />.
        /// </returns>
        public static Task<BlobResultSegment> ListBlobsSegmentedAsync(
            this CloudBlobContainer blobContainer,
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginListBlobsSegmented(prefix, useFlatBlobListing, blobListingDetails, maxResults, continuationToken, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<BlobResultSegment>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blobContainer.EndListBlobsSegmented(result);
                    });
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
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
            this CloudBlobContainer blobContainer,
            List<IListBlobItem> cloudBlobs,
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return blobContainer
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

                            return ListBlobsImplAsync(blobContainer, cloudBlobs, prefix, useFlatBlobListing, blobListingDetails, maxResults, continuationToken, cancellationToken);
                        }

                        return TaskHelpers.FromResult(cloudBlobs);
                    });
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
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
            this CloudBlobContainer blobContainer,
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobContainer, new List<IListBlobItem>(), prefix, useFlatBlobListing, blobListingDetails, maxResults, null, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="prefix">The blob name prefix.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobContainer blobContainer,
            string prefix,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobContainer, new List<IListBlobItem>(), prefix, false, BlobListingDetails.None, null, null, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blobs in the container that are retrieved asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects that implement <see cref="T:Microsoft.WindowsAzure.Storage.Blob.IListBlobItem" /> that are retrieved.
        /// </returns>
        public static Task<List<IListBlobItem>> ListBlobsAsync(
            this CloudBlobContainer blobContainer,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListBlobsImplAsync(blobContainer, new List<IListBlobItem>(), null, false, BlobListingDetails.None, null, null, cancellationToken);
        }

        /// <summary>
        ///     Releases the lease on this container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container, including a required lease ID.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task ReleaseLeaseAsync(
            this CloudBlobContainer blobContainer,
            AccessCondition accessCondition,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginReleaseLease(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobContainer.EndReleaseLease(result);
                    });
        }

        /// <summary>
        ///     Renews a lease on this container.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container, including a required lease ID.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task RenewLeaseAsync(
            this CloudBlobContainer blobContainer,
            AccessCondition accessCondition,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginRenewLease(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobContainer.EndRenewLease(result);
                    });
        }

        /// <summary>
        ///     Sets the container's user-defined metadata asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SetMetadataAsync(
            this CloudBlobContainer blobContainer,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginSetMetadata(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobContainer.EndSetMetadata(result);
                    });
        }

        /// <summary>
        ///     Sets permissions for the container asynchronously.
        /// </summary>
        /// <param name="blobContainer">Cloud blob container.</param>
        /// <param name="permissions">The permissions to apply to the container.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the container. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SetPermissionsAsync(
            this CloudBlobContainer blobContainer,
            BlobContainerPermissions permissions,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blobContainer.BeginSetPermissions(permissions, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blobContainer.EndSetPermissions(result);
                    });
        }
    }
}