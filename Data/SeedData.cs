using System.Text.Json;
using FoodRoute.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodRoute.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var datasetPath = Path.Combine(environment.ContentRootPath, "Data", "restaurant-dataset.json");

        if (!File.Exists(datasetPath))
        {
            throw new FileNotFoundException("Restaurant seed dataset was not found.", datasetPath);
        }

        var json = File.ReadAllText(datasetPath);
        var seedItems = JsonSerializer.Deserialize<List<RestaurantSeedItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<RestaurantSeedItem>();

        foreach (var group in seedItems
            .Where(item => !string.IsNullOrWhiteSpace(item.RestaurantName))
            .GroupBy(item => new
            {
                Name = item.RestaurantName.Trim(),
                City = item.City?.Trim() ?? string.Empty,
                Address = item.Address?.Trim() ?? string.Empty
            }))
        {
            var first = group.First();
            var restaurant = context.Restaurants
                .Include(r => r.FoodItems)
                .FirstOrDefault(r =>
                    r.Name == group.Key.Name
                    && r.City == group.Key.City
                    && r.Address == group.Key.Address);

            if (restaurant is null)
            {
                restaurant = new Restaurant
                {
                    Name = group.Key.Name,
                    City = group.Key.City,
                    Address = group.Key.Address,
                    Latitude = first.Latitude,
                    Longitude = first.Longitude,
                    LocationUrl = first.LocationUrl,
                    Rating = first.Rating ?? 0,
                    ReviewCount = first.ReviewCount
                };

                context.Restaurants.Add(restaurant);
            }

            foreach (var item in group)
            {
                var foodName = string.IsNullOrWhiteSpace(item.FeaturedProductName)
                    ? restaurant.Name
                    : item.FeaturedProductName.Trim();
                var imageUrl = item.ProductImageUrl?.Trim() ?? string.Empty;

                if (restaurant.FoodItems.Any(food =>
                    food.Name == foodName
                    && food.ImageUrl == imageUrl))
                {
                    continue;
                }

                restaurant.FoodItems.Add(new FoodItem
                {
                    Name = foodName,
                    ImageUrl = imageUrl,
                    Tags = item.Tags?.Trim() ?? string.Empty,
                    MealType = string.IsNullOrWhiteSpace(item.MealType) ? "Lunch" : item.MealType.Trim()
                });
            }
        }

        context.SaveChanges();
    }

    private sealed class RestaurantSeedItem
    {
        public string RestaurantName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string FeaturedProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Rating { get; set; }
        public int ReviewCount { get; set; }
        public string LocationUrl { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
    }
}
