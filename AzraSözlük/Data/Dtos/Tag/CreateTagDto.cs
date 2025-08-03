namespace AzraSözlük.Data.Dtos;

public class CreateTagDto
{
    public string Name { get; set; }

    public CreateTagDto(string name)
    {
        Name = name;
    }
}