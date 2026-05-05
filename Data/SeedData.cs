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
        EnsureDatabaseColumns(context);

        var dataPath = Path.Combine(environment.ContentRootPath, "Data");
        var datasetPath = Path.Combine(dataPath, "restaurant-dataset.json");

        if (!File.Exists(datasetPath))
        {
            throw new FileNotFoundException("Restaurant seed dataset was not found.", datasetPath);
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var seedItems = new List<RestaurantSeedItem>();
        foreach (var path in new[]
                 {
                     datasetPath,
                     Path.Combine(dataPath, "restaurant-dataset-antalya-extra.json")
                 }.Where(File.Exists))
        {
            var json = File.ReadAllText(path);
            seedItems.AddRange(JsonSerializer.Deserialize<List<RestaurantSeedItem>>(json, jsonOptions)
                ?? new List<RestaurantSeedItem>());
        }

        var seedGroups = seedItems
            .Where(item => !string.IsNullOrWhiteSpace(item.RestaurantName))
            .GroupBy(item => new
            {
                Name = item.RestaurantName.Trim(),
                City = item.City?.Trim() ?? string.Empty,
                Address = item.Address?.Trim() ?? string.Empty
            })
            .ToList();

        var seedRestaurantKeys = seedGroups
            .Select(group => group.Key)
            .ToHashSet();

        var staleRestaurants = context.Restaurants
            .Include(r => r.FoodItems)
            .AsEnumerable()
            .Where(restaurant => !seedRestaurantKeys.Contains(new
            {
                Name = restaurant.Name.Trim(),
                City = restaurant.City?.Trim() ?? string.Empty,
                Address = restaurant.Address?.Trim() ?? string.Empty
            }))
            .ToList();

        if (staleRestaurants.Count > 0)
        {
            context.Restaurants.RemoveRange(staleRestaurants);
        }

        foreach (var group in seedGroups)
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
                    Cuisine = first.Cuisine?.Trim() ?? string.Empty,
                    Latitude = first.Latitude,
                    Longitude = first.Longitude,
                    LocationUrl = first.LocationUrl,
                    Rating = first.Rating ?? 0,
                    ReviewCount = first.ReviewCount
                };

                context.Restaurants.Add(restaurant);
            }
            else
            {
                restaurant.Cuisine = first.Cuisine?.Trim() ?? restaurant.Cuisine;
                restaurant.Latitude = first.Latitude;
                restaurant.Longitude = first.Longitude;
                restaurant.LocationUrl = first.LocationUrl;
                restaurant.Rating = first.Rating ?? restaurant.Rating;
                restaurant.ReviewCount = first.ReviewCount;
            }

            var seedFoodNames = group
                .Select(item => string.IsNullOrWhiteSpace(item.FeaturedProductName)
                    ? restaurant.Name
                    : item.FeaturedProductName.Trim())
                .ToHashSet();

            var staleFoodItems = restaurant.FoodItems
                .Where(food => !seedFoodNames.Contains(food.Name))
                .ToList();

            foreach (var staleFoodItem in staleFoodItems)
            {
                restaurant.FoodItems.Remove(staleFoodItem);
            }

            foreach (var item in group)
            {
                var foodName = string.IsNullOrWhiteSpace(item.FeaturedProductName)
                    ? restaurant.Name
                    : item.FeaturedProductName.Trim();
                var imageUrl = item.ProductImageUrl?.Trim() ?? string.Empty;
                var imageInfo = LoadImage(environment.WebRootPath, imageUrl, item.SourceImageFileName);

                var existingFood = restaurant.FoodItems.FirstOrDefault(food => food.Name == foodName);
                if (existingFood is not null)
                {
                    existingFood.ImageUrl = imageUrl;
                    if (imageInfo.Data is { Length: > 0 })
                    {
                        existingFood.ImageFileName = imageInfo.FileName;
                        existingFood.ImageContentType = imageInfo.ContentType;
                        existingFood.ImageData = imageInfo.Data;
                    }
                    else if (string.IsNullOrWhiteSpace(existingFood.ImageFileName))
                    {
                        existingFood.ImageFileName = imageInfo.FileName;
                        existingFood.ImageContentType = imageInfo.ContentType;
                    }

                    existingFood.Description = item.FeaturedProductDescription?.Trim() ?? string.Empty;
                    existingFood.Tags = item.Tags?.Trim() ?? string.Empty;
                    existingFood.MealType = string.IsNullOrWhiteSpace(item.MealType) ? "Lunch" : item.MealType.Trim();
                    continue;
                }

                restaurant.FoodItems.Add(new FoodItem
                {
                    Name = foodName,
                    ImageUrl = imageUrl,
                    ImageFileName = imageInfo.FileName,
                    ImageContentType = imageInfo.ContentType,
                    ImageData = imageInfo.Data,
                    Description = item.FeaturedProductDescription?.Trim() ?? string.Empty,
                    Tags = item.Tags?.Trim() ?? string.Empty,
                    MealType = string.IsNullOrWhiteSpace(item.MealType) ? "Lunch" : item.MealType.Trim()
                });
            }
        }

        context.SaveChanges();
    }

    private static void EnsureDatabaseColumns(ApplicationDbContext context)
    {
        context.Database.ExecuteSqlRaw("""
            IF COL_LENGTH('Restaurants', 'Cuisine') IS NULL
                ALTER TABLE Restaurants ADD Cuisine nvarchar(100) NOT NULL CONSTRAINT DF_Restaurants_Cuisine DEFAULT '';

            IF COL_LENGTH('FoodItems', 'Description') IS NULL
                ALTER TABLE FoodItems ADD Description nvarchar(1000) NOT NULL CONSTRAINT DF_FoodItems_Description DEFAULT '';

            IF COL_LENGTH('FoodItems', 'ImageFileName') IS NULL
                ALTER TABLE FoodItems ADD ImageFileName nvarchar(260) NOT NULL CONSTRAINT DF_FoodItems_ImageFileName DEFAULT '';

            IF COL_LENGTH('FoodItems', 'ImageContentType') IS NULL
                ALTER TABLE FoodItems ADD ImageContentType nvarchar(100) NOT NULL CONSTRAINT DF_FoodItems_ImageContentType DEFAULT '';

            IF COL_LENGTH('FoodItems', 'ImageData') IS NULL
                ALTER TABLE FoodItems ADD ImageData varbinary(max) NULL;
            """);
    }

    private static ImageSeedInfo LoadImage(string webRootPath, string imageUrl, string sourceImageFileName)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return new ImageSeedInfo(string.Empty, string.Empty, null);
        }

        var fileName = Path.GetFileName(imageUrl);
        var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var imagePath = Path.Combine(webRootPath, relativePath);

        if (!File.Exists(imagePath))
        {
            var desktopImagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "antalyayemek",
                string.IsNullOrWhiteSpace(sourceImageFileName) ? fileName : sourceImageFileName);

            if (File.Exists(desktopImagePath))
            {
                return new ImageSeedInfo(fileName, GetContentType(desktopImagePath), File.ReadAllBytes(desktopImagePath));
            }

            return new ImageSeedInfo(fileName, GetContentType(fileName), null);
        }

        return new ImageSeedInfo(fileName, GetContentType(fileName), File.ReadAllBytes(imagePath));
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" or ".jfif" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }

    private sealed record ImageSeedInfo(string FileName, string ContentType, byte[]? Data);

    private sealed class RestaurantSeedItem
    {
        public string RestaurantName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string FeaturedProductName { get; set; } = string.Empty;
        public string FeaturedProductDescription { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public string SourceImageFileName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Rating { get; set; }
        public int ReviewCount { get; set; }
        public string LocationUrl { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public string Cuisine { get; set; } = string.Empty;
    }
}
