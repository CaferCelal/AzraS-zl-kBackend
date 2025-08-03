namespace AzraSözlük.Data.Dtos;

public class DetailedTagDto
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string UserName { get; set; }
    
    public DateTime GeneratedDate { get; set; }

    public DetailedTagDto(string id, string name, string userName,DateTime generatedDate)
    {
        Id = id;
        Name = name;
        UserName = userName;
        GeneratedDate = generatedDate;
    }
}