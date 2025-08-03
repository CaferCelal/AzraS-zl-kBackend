using System.ComponentModel.DataAnnotations.Schema;

namespace AzraSözlük.Data.Models;

public class CommentModel
{
    public string Id { get; set; }
    
    public string BlogId { get; set; }

    [ForeignKey(nameof(BlogId))] 
    public BlogModel Blog { get; set; } = null!;
    
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))] 
    public UserModel User { get; set; } = null!;
    
    public string Content { get; set; }

    public DateTime GeneratedDate;
    
    public ICollection<UserCommentLikeModel> UserCommentLikes { get; set; } = new List<UserCommentLikeModel>();

    public CommentModel(string blogId, string userId, string content)
    {
        Id = Guid.NewGuid().ToString();
        BlogId = blogId;
        UserId = userId;
        Content = content;
        GeneratedDate = DateTime.Today;
    }
}