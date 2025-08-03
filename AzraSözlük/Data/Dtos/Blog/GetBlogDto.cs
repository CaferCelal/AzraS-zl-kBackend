namespace AzraSözlük.Data.Dtos.Blog;

public class GetBlogDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string BlogHeader { get; set; }
    public string BlogContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdateDate { get; set; }

    public int LikeCount { get; set; }

    public int DislikeCount { get; set; }

    public int CommentCount { get; set; }
    
    public bool IsUserLiked { get; set; }
    
    public bool IsUserDisliked { get; set; }
    
    public GetBlogDto(string id,string userName, string blogHeader, string blogContent, DateTime createdAt,
        DateTime lastUpdateDate, int likeCount, int dislikeCount, int commentCount, bool isUserLiked, bool isUserDisliked)
    {
        Id = id;
        UserName = userName;
        BlogHeader = blogHeader;
        BlogContent = blogContent;
        CreatedAt = createdAt;
        LastUpdateDate = lastUpdateDate;
        LikeCount = likeCount;
        DislikeCount = dislikeCount;
        CommentCount = commentCount;
        IsUserLiked = isUserLiked;
        IsUserDisliked = isUserDisliked;
    }
}