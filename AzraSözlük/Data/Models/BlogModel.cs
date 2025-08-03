using System.ComponentModel.DataAnnotations.Schema;

namespace AzraSözlük.Data.Models;

public class BlogModel
{
    public string Id { get; set; }
    public string BlogHeader { get; set; }
    public string BlogContent { get; set; }
    public string UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;

    public string TagId { get; set; }
    
    [ForeignKey(nameof(TagId))]
    public TagModel Tag { get; set; } = null!;

    public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();

    public DateTime GeneratedDate { get; set; }
    
    public DateTime LastUpdateDate { get; set; }
    
    public ICollection<UserBlogLikeModel> UserBlogLikes { get; set; } = new List<UserBlogLikeModel>();
    
    public BlogModel(string blogHeader,string blogContent,string userId, string tagId)
    {
        Id = Guid.NewGuid().ToString();
        BlogHeader = blogHeader;
        BlogContent = blogContent;
        UserId = userId;
        TagId = tagId;
        GeneratedDate = DateTime.Today;
        LastUpdateDate = DateTime.Today;
    }
}