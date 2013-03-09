using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzure.Blob.Extensions
{
    /// <summary>
    ///     Cloud block blob extensions.
    /// </summary>
    public static class CloudBlockBlobExtensions
    {
        /// <summary>
        ///     Creates a snapshot of the blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="metadata">A collection of name-value pairs defining the metadata of the snapshot.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A blob snapshot.
        /// </returns>
        public static Task<CloudBlockBlob> CreateSnapshotAsync(
            this CloudBlockBlob blockBlob,
            IDictionary<string, string> metadata = null,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blockBlob.BeginCreateSnapshot(metadata, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<CloudBlockBlob>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blockBlob.EndCreateSnapshot(result);
                    });
        }

        /// <summary>
        ///     Creates a snapshot of the blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A blob snapshot.
        /// </returns>
        public static Task<CloudBlockBlob> CreateSnapshotAsync(
            this CloudBlockBlob blockBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return CreateSnapshotAsync(blockBlob, null, null, cancellationToken);
        }

        /// <summary>
        ///     Returns an enumerable collection of the blob's blocks, using the specified block list filter asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="blockListingFilter">
        ///     One of the enumeration values that indicates whether to return
        ///     committed blocks, uncommitted blocks, or both.
        /// </param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects implementing <see cref="T:Microsoft.WindowsAzure.Storage.Blob.ListBlockItem" />.
        /// </returns>
        public static Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
            this CloudBlockBlob blockBlob,
            BlockListingFilter blockListingFilter = BlockListingFilter.Committed,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blockBlob.BeginDownloadBlockList(blockListingFilter, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<IEnumerable<ListBlockItem>>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blockBlob.EndDownloadBlockList(result);
                    });
        }

        /// <summary>
        ///     Returns an enumerable collection of the blob's blocks, using the specified block list filter asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of objects implementing <see cref="T:Microsoft.WindowsAzure.Storage.Blob.ListBlockItem" />.
        /// </returns>
        public static Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
            this CloudBlockBlob blockBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return DownloadBlockListAsync(blockBlob, BlockListingFilter.Committed, null, cancellationToken);
        }

        /// <summary>
        ///     Opens a stream for writing to the blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A stream to be used for writing to the blob.
        /// </returns>
        public static Task<Stream> OpenWriteAsync(
            this CloudBlockBlob blockBlob,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blockBlob.BeginOpenWrite(accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<Stream>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blockBlob.EndOpenWrite(result);
                    });
        }

        /// <summary>
        ///     Opens a stream for writing to the blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A stream to be used for writing to the blob.
        /// </returns>
        public static Task<Stream> OpenWriteAsync(
            this CloudBlockBlob blockBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return OpenWriteAsync(blockBlob, null, cancellationToken);
        }

        /// <summary>
        ///     Uploads a single block asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="blockId">A base64-encoded block ID that identifies the block.</param>
        /// <param name="blockData">A stream that provides the data for the block.</param>
        /// <param name="contentMd5">
        ///     An optional hash value that will be used to set the
        ///     <see
        ///         cref="P:Microsoft.WindowsAzure.Storage.Blob.BlobProperties.ContentMD5" />
        ///     property
        ///     on the blob. May be <c>null</c> or an empty string.
        /// </param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task PutBlockAsync(
            this CloudBlockBlob blockBlob,
            string blockId,
            Stream blockData,
            string contentMd5,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blockBlob.BeginPutBlock(blockId, blockData, contentMd5, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blockBlob.EndPutBlock(result);
                    });
        }

        /// <summary>
        ///     Uploads a single block asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="blockId">A base64-encoded block ID that identifies the block.</param>
        /// <param name="blockData">A stream that provides the data for the block.</param>
        /// <param name="contentMd5">
        ///     An optional hash value that will be used to set the
        ///     <see
        ///         cref="P:Microsoft.WindowsAzure.Storage.Blob.BlobProperties.ContentMD5" />
        ///     property
        ///     on the blob. May be <c>null</c> or an empty string.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task PutBlockAsync(
            this CloudBlockBlob blockBlob,
            string blockId,
            Stream blockData,
            string contentMd5,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return PutBlockAsync(blockBlob, blockId, blockData, contentMd5, null, cancellationToken);
        }

        /// <summary>
        ///     Uploads a list of blocks to a new or existing blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="blockList">An enumerable collection of block IDs, as base64-encoded strings.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task PutBlockListAsync(
            this CloudBlockBlob blockBlob,
            IEnumerable<string> blockList,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blockBlob.BeginPutBlockList(blockList, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        blockBlob.EndPutBlockList(result);
                    });
        }

        /// <summary>
        ///     Uploads a list of blocks to a new or existing blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="blockList">An enumerable collection of block IDs, as base64-encoded strings.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task PutBlockListAsync(
            this CloudBlockBlob blockBlob,
            IEnumerable<string> blockList,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return PutBlockListAsync(blockBlob, blockList, null, cancellationToken);
        }

        /// <summary>
        ///     Requests that the service start to copy a blob's contents, properties, and metadata to a new blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="source">The URI of a source blob.</param>
        /// <param name="sourceAccessCondition">
        ///     An object that represents the access conditions for the source blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="destAccessCondition">
        ///     An object that represents the access conditions for the destination blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The copy ID associated with the copy operation.
        /// </returns>
        /// <remarks>
        ///     This method fetches the blob's ETag, last modified time, and part of the copy state.
        ///     The copy ID and copy status fields are fetched, and the rest of the copy state is cleared.
        /// </remarks>
        public static Task<string> StartCopyFromBlobAsync(
            this CloudBlockBlob blockBlob,
            CloudBlockBlob source,
            AccessCondition sourceAccessCondition = null,
            AccessCondition destAccessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = blockBlob.BeginStartCopyFromBlob(source, sourceAccessCondition, destAccessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<string>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return blockBlob.EndStartCopyFromBlob(result);
                    });
        }

        /// <summary>
        ///     Requests that the service start to copy a blob's contents, properties, and metadata to a new blob asynchronously.
        /// </summary>
        /// <param name="blockBlob">Cloud block blob.</param>
        /// <param name="source">The URI of a source blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The copy ID associated with the copy operation.
        /// </returns>
        /// <remarks>
        ///     This method fetches the blob's ETag, last modified time, and part of the copy state.
        ///     The copy ID and copy status fields are fetched, and the rest of the copy state is cleared.
        /// </remarks>
        public static Task<string> StartCopyFromBlobAsync(
            this CloudBlockBlob blockBlob,
            CloudBlockBlob source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return StartCopyFromBlobAsync(blockBlob, source, null, null, cancellationToken);
        }
    }
}