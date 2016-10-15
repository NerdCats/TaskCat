namespace TaskCat.Lib.Comments
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using Domain;

    /// <summary>
    /// Default implementation for Comment repository
    /// </summary>
    public interface ICommentService: IRepository<Comment>
    {
        /// <summary>
        /// Get a comment feed based on a reference id and an entity type.
        /// </summary>
        /// <param name="refId">Reference Id for the comment.</param>
        /// <param name="entityType">Entity type for the comment reference.</param>
        /// <param name="page">Page number to be fetched.</param>
        /// <param name="pageSize">Page size to be used.</param>
        /// <returns></returns>
        Task<IEnumerable<Comment>> GetByRefId(string refId, string entityType, int page, int pageSize);
    }
}