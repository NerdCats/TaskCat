namespace TaskCat.Data.Model
{
    using System.ComponentModel.DataAnnotations;

    public class CommentUpdateModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id is not provided")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CommentText not provided")]
        public string CommentText { get; set; }
    }
}
