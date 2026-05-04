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

        if (context.Restaurants.Any())
        {
            return;
        }

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
            var restaurant = new Restaurant
            {
                Name = group.Key.Name,
                City = group.Key.City,
                Address = group.Key.Address,
                Latitude = first.Latitude,
                Longitude = first.Longitude,
                LocationUrl = first.LocationUrl
            };

            foreach (var item in group)
            {
                restaurant.FoodItems.Add(new FoodItem
                {
                    Name = string.IsNullOrWhiteSpace(item.FeaturedProductName) ? restaurant.Name : item.FeaturedProductName.Trim(),
                    ImageUrl = item.ProductImageUrl?.Trim() ?? string.Empty,
                    Tags = item.Tags?.Trim() ?? string.Empty,
                    MealType = string.IsNullOrWhiteSpace(item.MealType) ? "Lunch" : item.MealType.Trim()
                });
            }

            context.Restaurants.Add(restaurant);
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
        public string LocationUrl { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
    }
}
