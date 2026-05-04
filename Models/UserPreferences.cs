namespace FoodRoute.Models;

public class UserPreferences
{
    public int Days { get; set; } = 1;
    public string City { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new List<string>();
}
