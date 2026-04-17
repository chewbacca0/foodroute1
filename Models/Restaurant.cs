namespace FoodRoute.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? LocationUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
}
