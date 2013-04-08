using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzure.Blob.Extensions
{
    /// <summary>
    ///     Cloud page blob extensions.
    /// </summary>
    public static class CloudPageBlobExtensions
    {
        /// <summary>
        ///     Clears pages from a page blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="startOffset">The offset at which to begin clearing pages, in bytes. The offset must be a multiple of 512.</param>
        /// <param name="length">The length of the data range to be cleared, in bytes. The length must be a multiple of 512.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task ClearPagesAsync(
            this CloudPageBlob pageBlob,
            long startOffset,
            long length,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = pageBlob.BeginClearPages(startOffset, length, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        pageBlob.EndClearPages(result);
                    });
        }

        /// <summary>
        ///     Clears pages from a page blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="startOffset">The offset at which to begin clearing pages, in bytes. The offset must be a multiple of 512.</param>
        /// <param name="length">The length of the data range to be cleared, in bytes. The length must be a multiple of 512.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task ClearPagesAsync(
            this CloudPageBlob pageBlob,
            long startOffset,
            long length,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ClearPagesAsync(pageBlob, startOffset, length, null, cancellationToken);
        }

        /// <summary>
        ///     Creates a page blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="size">The maximum size of the page blob, in bytes.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task CreateAsync(
            this CloudPageBlob pageBlob,
            long size,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = pageBlob.BeginCreate(size, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        pageBlob.EndCreate(result);
                    });
        }

        /// <summary>
        ///     Creates a page blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="size">The maximum size of the page blob, in bytes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task CreateAsync(
            this CloudPageBlob pageBlob,
            long size,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return CreateAsync(pageBlob, size, null, cancellationToken);
        }

        /// <summary>
        ///     Creates a snapshot of the blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="metadata">A collection of name-value pairs defining the metadata of the snapshot.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A blob snapshot.
        /// </returns>
        public static Task<CloudPageBlob> CreateSnapshotAsync(
            this CloudPageBlob pageBlob,
            IDictionary<string, string> metadata = null,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = pageBlob.BeginCreateSnapshot(metadata, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<CloudPageBlob>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return pageBlob.EndCreateSnapshot(result);
                    });
        }

        /// <summary>
        ///     Creates a snapshot of the blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A blob snapshot.
        /// </returns>
        public static Task<CloudPageBlob> CreateSnapshotAsync(
            this CloudPageBlob pageBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return CreateSnapshotAsync(pageBlob, null, null, cancellationToken);
        }

        /// <summary>
        ///     Gets a collection of page ranges and their starting and ending bytes asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="offset">The starting offset of the data range, in bytes. Must be a multiple of 512.</param>
        /// <param name="length">The length of the data range, in bytes. Must be a multiple of 512.</param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of page ranges.
        /// </returns>
        public static Task<IEnumerable<PageRange>> GetPageRangesAsync(
            this CloudPageBlob pageBlob,
            long? offset = null,
            long? length = null,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = pageBlob.BeginGetPageRanges(offset, length, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<IEnumerable<PageRange>>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return pageBlob.EndGetPageRanges(result);
                    });
        }

        /// <summary>
        ///     Gets a collection of page ranges and their starting and ending bytes asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of page ranges.
        /// </returns>
        public static Task<IEnumerable<PageRange>> GetPageRangesAsync(
            this CloudPageBlob pageBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return GetPageRangesAsync(pageBlob, null, null, null, cancellationToken);
        }

        /// <summary>
        ///     Opens a stream for writing to the blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="size">
        ///     The size of the page blob, in bytes. The size must be a multiple of 512. If <c>null</c>, the page blob must already exist.
        /// </param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A stream to be used for writing to the blob.
        /// </returns>
        public static Task<Stream> OpenWriteAsync(
            this CloudPageBlob pageBlob,
            long? size,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = pageBlob.BeginOpenWrite(size, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<Stream>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return pageBlob.EndOpenWrite(result);
                    });
        }

        /// <summary>
        ///     Opens a stream for writing to the blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A stream to be used for writing to the blob.
        /// </returns>
        public static Task<Stream> OpenWriteAsync(
            this CloudPageBlob pageBlob,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return OpenWriteAsync(pageBlob, null, null, cancellationToken);
        }

        /// <summary>
        ///     Requests that the service start to copy a blob's contents, properties, and metadata to a new blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
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
            this CloudPageBlob pageBlob,
            CloudPageBlob source,
            AccessCondition sourceAccessCondition = null,
            AccessCondition destAccessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = pageBlob.BeginStartCopyFromBlob(source, sourceAccessCondition, destAccessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<string>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return pageBlob.EndStartCopyFromBlob(result);
                    });
        }

        /// <summary>
        ///     Requests that the service start to copy a blob's contents, properties, and metadata to a new blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
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
            this CloudPageBlob pageBlob,
            CloudPageBlob source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return StartCopyFromBlobAsync(pageBlob, source, null, null, cancellationToken);
        }

        /// <summary>
        ///     Writes pages to a page blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="pageData">A stream providing the page data.</param>
        /// <param name="startOffset">The offset at which to begin writing, in bytes. The offset must be a multiple of 512.</param>
        /// <param name="contentMd5">
        ///     An optional hash value that will be used to set the
        ///     <see cref="P:Microsoft.WindowsAzure.Storage.Blob.BlobProperties.ContentMD5" />
        ///     property on the blob. May be <c>null</c> or an empty string.
        /// </param>
        /// <param name="accessCondition">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.AccessCondition" /> object that represents the access conditions for the blob. If <c>null</c>, no condition is used.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task WritePagesAsync(
            this CloudPageBlob pageBlob,
            Stream pageData,
            long startOffset,
            string contentMd5 = null,
            AccessCondition accessCondition = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = pageBlob.BeginWritePages(pageData, startOffset, contentMd5, accessCondition, null, null, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        pageBlob.EndWritePages(result);
                    });
        }

        /// <summary>
        ///     Writes pages to a page blob asynchronously.
        /// </summary>
        /// <param name="pageBlob">Cloud page blob.</param>
        /// <param name="pageData">A stream providing the page data.</param>
        /// <param name="startOffset">The offset at which to begin writing, in bytes. The offset must be a multiple of 512.</param>
        /// <param name="contentMd5">
        ///     An optional hash value that will be used to set the
        ///     <see cref="P:Microsoft.WindowsAzure.Storage.Blob.BlobProperties.ContentMD5" />
        ///     property on the blob. May be <c>null</c> or an empty string.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task WritePagesAsync(
            this CloudPageBlob pageBlob,
            Stream pageData,
            long startOffset,
            string contentMd5 = null,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return WritePagesAsync(pageBlob, pageData, startOffset, contentMd5, null, cancellationToken);
        }
    }
}