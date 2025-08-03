namespace AzraSözlük.Data.Dtos.Comment;

public class CreateCommentDto
{
    public string CommentContent { get; set; }

    public CreateCommentDto(string commentContent)
    {
        CommentContent = commentContent;
    }
}