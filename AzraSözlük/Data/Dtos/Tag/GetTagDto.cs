namespace AzraSözlük.Data.Dtos;

public class GetTagDto
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTime GeneratedDate { get; set; }

    public int BlogCount { get; set; }

    public GetTagDto(string id, string name, DateTime generatedDate, int blogCount)
    {
        Id = id;
        Name = name;
        GeneratedDate = generatedDate;
        BlogCount = blogCount;
    }
}