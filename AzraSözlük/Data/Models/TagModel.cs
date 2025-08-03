using System.ComponentModel.DataAnnotations.Schema;

namespace AzraSözlük.Data.Models;

public class TagModel
{
    public string Id { get; set; }
    public string Name { get; set; }

    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))] 
    public UserModel User { get; set; } = null!;
    
    public ICollection<BlogModel> Blogs { get; set; } = new List<BlogModel>();

    public DateTime GeneratedDate;
    
    public TagModel(string name, string userId)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        UserId = userId;
        GeneratedDate = DateTime.Today;
    }
    
}