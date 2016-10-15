namespace TaskCat.Lib.Comments
{
    using System.Threading.Tasks;
    using Data.Entity;
    using Domain;
    using Data.Model.Operation;

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
        Task<QueryResult<Comment>> GetByRefId(string refId, string entityType, int page, int pageSize);

        /// <summary>
        /// Determines whether this entity type is valid for commenting
        /// </summary>
        /// <param name="entityType">Entity type for the comment reference.</param>
        /// <returns></returns>
        bool IsValidEntityTypeForComment(string entityType);
    }
}