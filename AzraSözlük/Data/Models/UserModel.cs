using Microsoft.AspNetCore.Identity;

namespace AzraSözlük.Data.Models;

public class UserModel: IdentityUser
{
    public ICollection<TagModel> Tags { get; set; } = new List<TagModel>();
    
    public ICollection<BlogModel> Blogs { get; set; } = new List<BlogModel>();

    public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
    
    public ICollection<UserBlogLikeModel> UserBlogLikes { get; set; } = new List<UserBlogLikeModel>();
    
    public ICollection<UserCommentLikeModel> UserCommentLikes { get; set; } = new List<UserCommentLikeModel>();
}