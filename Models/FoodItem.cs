namespace FoodRoute.Models;

public class FoodItem
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty; // Comma-separated: "Vegan,Dessert,FineDining"
    public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Dinner
    
    public Restaurant Restaurant { get; set; } = null!;
}
