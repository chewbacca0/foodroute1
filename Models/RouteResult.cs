namespace FoodRoute.Models;

public class RouteDay
{
    public int DayNumber { get; set; }
    public string DayLabel { get; set; } = string.Empty;
    public List<RouteMeal> Meals { get; set; } = new List<RouteMeal>();
}

public class RouteMeal
{
    public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Dinner
    public FoodItem FoodItem { get; set; } = null!;
    public Restaurant Restaurant { get; set; } = null!;
}

public class RouteResult
{
    public List<RouteDay> Days { get; set; } = new List<RouteDay>();
    public string City { get; set; } = string.Empty;
    public int TotalRestaurants { get; set; }
}
