namespace AzraSözlük.Data.Dtos.Comment;

public class GetCommentDto
{
    public string Id { get; set; }
    
    public string UserName { get; set; }
    
    public string Content { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public int LikeCount { get; set; }

    public int DislikeCount { get; set; }

    
    public bool IsUserLiked { get; set; }
    
    public bool IsUserDisliked { get; set; }

    public GetCommentDto(string id, string userName, string content, DateTime createdAt,
        int likeCount, int dislikeCount, bool isUserLiked, bool isUserDisliked)
    {
        Id = id;
        UserName = userName;
        Content = content;
        CreatedAt = createdAt;
        LikeCount = likeCount;
        DislikeCount = dislikeCount;
        IsUserLiked = isUserLiked;
        IsUserDisliked = isUserDisliked;
    }
}