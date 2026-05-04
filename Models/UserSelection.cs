namespace FoodRoute.Models;

public class UserSelection
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public int FoodItemId { get; set; }
    public bool IsLiked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public FoodItem FoodItem { get; set; } = null!;
}
