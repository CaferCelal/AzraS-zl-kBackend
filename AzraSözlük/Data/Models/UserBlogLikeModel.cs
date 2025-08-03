using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AzraSözlük.Constants;

namespace AzraSözlük.Data.Models;

public class UserBlogLikeModel
{
    [MaxLength(ProgramConstants.MaxIdLength)]
    public string Id { get; set; }
    
    [MaxLength(ProgramConstants.MaxIdLength)]
    public string UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;
    
    [MaxLength(ProgramConstants.MaxIdLength)]
    public string BlogId { get; set; }
    
    [ForeignKey(nameof(BlogId))]
    public BlogModel Blog { get; set; } = null!;
    
    public bool Like { get; set; }
    
    public bool Dislike { get; set; }
    
    public bool Neutral { get; set; }

    public UserBlogLikeModel(string userId, string blogId)
    {
        Id = Guid.NewGuid().ToString();
        UserId = userId;
        BlogId = blogId;
        Like = false;
        Dislike = false;
        Neutral = true;
    }
}