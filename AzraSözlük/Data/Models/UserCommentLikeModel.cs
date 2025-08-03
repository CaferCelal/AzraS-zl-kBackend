using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AzraSözlük.Constants;

namespace AzraSözlük.Data.Models;

public class UserCommentLikeModel
{
    [MaxLength(ProgramConstants.MaxIdLength)]
    public string Id { get; set; }
    
    [MaxLength(ProgramConstants.MaxIdLength)]
    public string UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;
    
    [MaxLength(ProgramConstants.MaxIdLength)]
    public string CommentId { get; set; }
    
    [ForeignKey(nameof(CommentId))]
    public CommentModel Comment { get; set; } = null!;
    
    public bool Like { get; set; }
    
    public bool Dislike { get; set; }
    
    public bool Neutral { get; set; }

    public UserCommentLikeModel(string userId, string commentId)
    {
        Id = Guid.NewGuid().ToString();
        UserId = userId;
        CommentId = commentId;
        Like = false;
        Dislike = false;
        Neutral = true;
    }
}