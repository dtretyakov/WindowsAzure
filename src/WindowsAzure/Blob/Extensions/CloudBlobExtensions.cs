using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzure.Blob.Extensions
{
    /// <summary>
    ///     Extensions for a cloud blob.
    /// </summary>
    public static class CloudBlobExtensions
    {
        /// <summary>
        ///     Aborts an ongoing blob copy operation asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="copyId">A string identifying the copy operation.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task AbortCopyAsync(
            this ICloudBlob cloudBlob,
            string copyId,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginAbortCopy(copyId, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndAbortCopy(result);
                    });
        }

        /// <summary>
        ///     Aborts an ongoing blob copy operation asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="copyId">A string identifying the copy operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task AbortCopyAsync(
            this ICloudBlob cloudBlob,
            string copyId,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return AbortCopyAsync(cloudBlob, copyId, null, cancellationToken);
        }

        /// <summary>
        ///     Acquires a lease on this blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="leaseTime">
        ///     A <see cref="T:System.TimeSpan" /> representing the span of time for which to acquire the lease,
        ///     which will be rounded down to seconds.
        /// </param>
        /// <param name="proposedLeaseId">A string representing the proposed lease ID for the new lease.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The ID of the acquired lease.
        /// </returns>
        public static Task<string> AcquireLeaseAsync(
            this ICloudBlob cloudBlob,
            TimeSpan? leaseTime,
            string proposedLeaseId,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginAcquireLease(leaseTime, proposedLeaseId, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<string>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudBlob.EndAcquireLease(result);
                    });
        }

        /// <summary>
        ///     Acquires a lease on this blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="leaseTime">
        ///     A <see cref="T:System.TimeSpan" /> representing the span of time for which to acquire the lease,
        ///     which will be rounded down to seconds.
        /// </param>
        /// <param name="proposedLeaseId">A string representing the proposed lease ID for the new lease.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The ID of the acquired lease.
        /// </returns>
        public static Task<string> AcquireLeaseAsync(
            this ICloudBlob cloudBlob,
            TimeSpan? leaseTime,
            string proposedLeaseId,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return AcquireLeaseAsync(cloudBlob, leaseTime, proposedLeaseId, null, cancellationToken);
        }

        /// <summary>
        ///     Breaks the current lease on this blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="breakPeriod">
        ///     A <see cref="T:System.TimeSpan" /> representing the amount of time to allow the lease to remain,
        ///     which will be rounded down to seconds.
        /// </param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:System.TimeSpan" /> representing the amount of time before the lease ends, to the second.
        /// </returns>
        public static Task<TimeSpan> BreakLeaseAsync(
            this ICloudBlob cloudBlob,
            TimeSpan? breakPeriod = null,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginBreakLease(breakPeriod, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<TimeSpan>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudBlob.EndBreakLease(result);
                    });
        }

        /// <summary>
        ///     Breaks the current lease on this blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="breakPeriod">
        ///     A <see cref="T:System.TimeSpan" /> representing the amount of time to allow the lease to remain,
        ///     which will be rounded down to seconds.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:System.TimeSpan" /> representing the amount of time before the lease ends, to the second.
        /// </returns>
        public static Task<TimeSpan> BreakLeaseAsync(
            this ICloudBlob cloudBlob,
            TimeSpan? breakPeriod = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return BreakLeaseAsync(cloudBlob, breakPeriod, null, cancellationToken);
        }

        /// <summary>
        ///     Changes the lease ID on this blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="proposedLeaseId">A string representing the proposed lease ID for the new lease.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob, including a required lease ID.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The new lease ID.
        /// </returns>
        public static Task<string> ChangeLeaseAsync(
            this ICloudBlob cloudBlob,
            string proposedLeaseId, AccessCondition accessCondition,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginChangeLease(proposedLeaseId, accessCondition, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<string>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudBlob.EndChangeLease(result);
                    });
        }

        /// <summary>
        ///     Deletes the blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="deleteSnapshotsOption">Whether to only delete the blob, to delete the blob and all snapshots, or to only delete the snapshots.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task DeleteAsync(
            this ICloudBlob cloudBlob,
            DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginDelete(deleteSnapshotsOption, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndDelete(result);
                    });
        }

        /// <summary>
        ///     Deletes the blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task DeleteAsync(
            this ICloudBlob cloudBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return DeleteAsync(cloudBlob, DeleteSnapshotsOption.None, null, cancellationToken);
        }

        /// <summary>
        ///     Deletes the blob if it already exists asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="deleteSnapshotsOption">Whether to only delete the blob, to delete the blob and all snapshots, or to only delete the snapshots.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the blob did not already exist and was created; otherwise <c>false</c>.
        /// </returns>
        public static Task<bool> DeleteIfExistsAsync(
            this ICloudBlob cloudBlob,
            DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginDeleteIfExists(deleteSnapshotsOption, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudBlob.EndDeleteIfExists(result);
                    });
        }

        /// <summary>
        ///     Deletes the blob if it already exists asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the blob did not already exist and was created; otherwise <c>false</c>.
        /// </returns>
        public static Task<bool> DeleteIfExistsAsync(
            this ICloudBlob cloudBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return DeleteIfExistsAsync(cloudBlob, DeleteSnapshotsOption.None, null, cancellationToken);
        }

        /// <summary>
        ///     Downloads the contents of a blob to a stream asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="target">The target stream.</param>
        /// <param name="offset">The starting offset of the data range, in bytes.</param>
        /// <param name="length">The length of the data range, in bytes.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task DownloadRangeToStreamAsync(
            this ICloudBlob cloudBlob,
            Stream target,
            long? offset,
            long? length,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginDownloadRangeToStream(target, offset, length, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndDownloadRangeToStream(result);
                    });
        }

        /// <summary>
        ///     Downloads the contents of a blob to a stream asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="target">The target stream.</param>
        /// <param name="offset">The starting offset of the data range, in bytes.</param>
        /// <param name="length">The length of the data range, in bytes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task DownloadRangeToStreamAsync(
            this ICloudBlob cloudBlob,
            Stream target,
            long? offset,
            long? length,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return DownloadRangeToStreamAsync(cloudBlob, target, offset, length, null, cancellationToken);
        }

        /// <summary>
        ///     Downloads the contents of a blob to a stream asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="target">The target stream.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task DownloadToStreamAsync(
            this ICloudBlob cloudBlob,
            Stream target,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginDownloadToStream(target, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndDownloadToStream(result);
                    });
        }

        /// <summary>
        ///     Downloads the contents of a blob to a stream asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="target">The target stream.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task DownloadToStreamAsync(
            this ICloudBlob cloudBlob,
            Stream target,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return DownloadToStreamAsync(cloudBlob, target, null, cancellationToken);
        }

        /// <summary>
        ///     Checks existence of the blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the blob exists.
        /// </returns>
        public static Task<bool> ExistsAsync(
            this ICloudBlob cloudBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginExists(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudBlob.EndExists(result);
                    });
        }

        /// <summary>
        ///     Populates a blob's properties and metadata asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task FetchAttributesAsync(
            this ICloudBlob cloudBlob,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginFetchAttributes(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndFetchAttributes(result);
                    });
        }

        /// <summary>
        ///     Populates a blob's properties and metadata asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task FetchAttributesAsync(
            this ICloudBlob cloudBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return FetchAttributesAsync(cloudBlob, null, cancellationToken);
        }

        /// <summary>
        ///     Releases the lease on this blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task ReleaseLeaseAsync(
            this ICloudBlob cloudBlob,
            AccessCondition accessCondition,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginReleaseLease(accessCondition, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndReleaseLease(result);
                    });
        }

        /// <summary>
        ///     Renews a lease on this blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task RenewLeaseAsync(
            this ICloudBlob cloudBlob,
            AccessCondition accessCondition,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginRenewLease(accessCondition, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndRenewLease(result);
                    });
        }

        /// <summary>
        ///     Updates the blob's metadata asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SetMetadataAsync(
            this ICloudBlob cloudBlob,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginSetMetadata(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndSetMetadata(result);
                    });
        }

        /// <summary>
        ///     Updates the blob's metadata asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SetMetadataAsync(
            this ICloudBlob cloudBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return SetMetadataAsync(cloudBlob, null, cancellationToken);
        }

        /// <summary>
        ///     Updates the blob's properties asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SetPropertiesAsync(
            this ICloudBlob cloudBlob,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginSetProperties(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndSetProperties(result);
                    });
        }

        /// <summary>
        ///     Updates the blob's properties asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SetPropertiesAsync(
            this ICloudBlob cloudBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return SetPropertiesAsync(cloudBlob, null, cancellationToken);
        }

        /// <summary>
        ///     Requests that the service start to copy a blob's contents, properties, and metadata to a new blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="source">The URI of a source blob.</param>
        /// <param name="sourceAccessCondition">An object that represents the access conditions for the source blob.</param>
        /// <param name="destAccessCondition">An object that represents the access conditions for the destination blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The copy ID associated with the copy operation.
        /// </returns>
        public static Task<string> StartCopyFromBlobAsync(
            this ICloudBlob cloudBlob,
            Uri source,
            AccessCondition sourceAccessCondition = null,
            AccessCondition destAccessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginStartCopyFromBlob(source, sourceAccessCondition, destAccessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<string>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudBlob.EndStartCopyFromBlob(result);
                    });
        }

        /// <summary>
        ///     Requests that the service start to copy a blob's contents, properties, and metadata to a new blob asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="source">The URI of a source blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The copy ID associated with the copy operation.
        /// </returns>
        public static Task<string> StartCopyFromBlobAsync(
            this ICloudBlob cloudBlob,
            Uri source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return StartCopyFromBlobAsync(cloudBlob, source, null, null, cancellationToken);
        }

        /// <summary>
        ///     Uploads a stream to the Windows Azure Blob Service asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="source">The stream providing the blob content. Use a seek-able stream for optimal performance.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task UploadFromStreamAsync(
            this ICloudBlob cloudBlob,
            Stream source,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudBlob.BeginUploadFromStream(source, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudBlob.EndUploadFromStream(result);
                    });
        }

        /// <summary>
        ///     Uploads a stream to the Windows Azure Blob Service asynchronously.
        /// </summary>
        /// <param name="cloudBlob">Cloud blob client.</param>
        /// <param name="source">The stream providing the blob content. Use a seek-able stream for optimal performance.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task UploadFromStreamAsync(
            this ICloudBlob cloudBlob,
            Stream source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return UploadFromStreamAsync(cloudBlob, source, null, cancellationToken);
        }
    }
}