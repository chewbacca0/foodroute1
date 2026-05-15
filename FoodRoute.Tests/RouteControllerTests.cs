using FoodRoute.Controllers;
using FoodRoute.Data;
using FoodRoute.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodRoute.Tests;

public class RouteControllerTests
{
    [Fact]
    public async Task OpenGoogleMaps_UsesOnlyRestaurantsForRequestedDay()
    {
        await using var context = CreateContext();
        await SeedLikedFoodItemAsync(context, "Breakfast", 10, 20, 1);
        await SeedLikedFoodItemAsync(context, "Coffee", 11, 21, 2);
        await SeedLikedFoodItemAsync(context, "Lunch", 12, 22, 3);
        await SeedLikedFoodItemAsync(context, "Dinner", 13, 23, 4);
        var controller = CreateController(context, days: 2);

        var result = await controller.OpenGoogleMaps(dayNumber: 1);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Contains("origin=10%2C20", redirect.Url);
        Assert.Contains("destination=11%2C21", redirect.Url);
        Assert.DoesNotContain("12%2C22", redirect.Url);
        Assert.DoesNotContain("13%2C23", redirect.Url);
    }

    [Fact]
    public async Task Generate_OrdersMealsWithinDayByExpectedMealOrder()
    {
        await using var context = CreateContext();
        await SeedLikedFoodItemAsync(context, "Dinner", 13, 23, 1);
        await SeedLikedFoodItemAsync(context, "Lunch", 12, 22, 2);
        await SeedLikedFoodItemAsync(context, "Coffee", 11, 21, 3);
        await SeedLikedFoodItemAsync(context, "Breakfast", 10, 20, 4);
        var controller = CreateController(context, days: 1);

        var result = await controller.Generate();

        var view = Assert.IsType<ViewResult>(result);
        var routeResult = Assert.IsType<RouteResult>(view.Model);
        var mealTypes = routeResult.Days.Single().Meals
            .Select(meal => meal.FoodItem.MealType)
            .ToList();

        Assert.Equal(new[] { "Breakfast", "Coffee", "Lunch", "Dinner" }, mealTypes);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static RouteController CreateController(ApplicationDbContext context, int days)
    {
        var session = new TestSession();
        session.SetString("SelectionSessionId", "test-session");
        session.SetString("City", "Istanbul");
        session.SetInt32("Days", days);

        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        return new RouteController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static async Task SeedLikedFoodItemAsync(
        ApplicationDbContext context,
        string mealType,
        double latitude,
        double longitude,
        int minutesFromStart)
    {
        var restaurant = new Restaurant
        {
            Name = $"{mealType} Restaurant",
            City = "Istanbul",
            Address = $"{mealType} Address",
            Latitude = latitude,
            Longitude = longitude
        };

        var foodItem = new FoodItem
        {
            Name = $"{mealType} Food",
            ImageUrl = "https://example.com/food.jpg",
            Tags = mealType,
            MealType = mealType,
            Restaurant = restaurant
        };

        context.UserSelections.Add(new UserSelection
        {
            SessionId = "test-session",
            FoodItem = foodItem,
            IsLiked = true,
            CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc).AddMinutes(minutesFromStart)
        });

        await context.SaveChangesAsync();
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _values = new();

        public IEnumerable<string> Keys => _values.Keys;
        public string Id { get; } = Guid.NewGuid().ToString();
        public bool IsAvailable => true;

        public void Clear()
        {
            _values.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _values.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _values[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _values.TryGetValue(key, out value!);
        }
    }
}
