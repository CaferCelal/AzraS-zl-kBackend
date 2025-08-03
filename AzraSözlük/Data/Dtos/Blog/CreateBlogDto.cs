namespace AzraSözlük.Data.Dtos.Blog;

public class CreateBlogDto
{
    public string BlogHeader { get; set; }
    public string BlogContent { get; set; }

    public CreateBlogDto(string blogHeader, string blogContent)
    {
        BlogHeader = blogHeader;
        BlogContent = blogContent;
    }
}