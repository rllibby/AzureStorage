/*
 *  Copyright © 2016, Russell Libby 
 */

namespace AzureStorage.Models
{
    /// <summary>
    /// Enumeration for the container types.
    /// </summary>
    public enum ContainerType
    {
        /// <summary>
        /// Azure table.
        /// </summary>
        Table,

        /// <summary>
        /// Azure queue.
        /// </summary>
        Queue,

        /// <summary>
        /// Azure blob container.
        /// </summary>
        BlobContainer
    }
}
