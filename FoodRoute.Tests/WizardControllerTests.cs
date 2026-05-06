using FoodRoute.Controllers;
using FoodRoute.Data;
using FoodRoute.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodRoute.Tests;

public class WizardControllerTests
{
    [Fact]
    public async Task Like_ReturnsNotFound_WhenFoodItemDoesNotExist()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Like(new SwipeRequest { FoodItemId = 999 });

        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Empty(context.UserSelections);
    }

    [Fact]
    public async Task Like_DoesNotCreateDuplicateSelection_WhenSameFoodItemIsLikedTwice()
    {
        await using var context = CreateContext();
        var foodItem = await SeedFoodItemAsync(context);
        var controller = CreateController(context);

        var firstResult = await controller.Like(new SwipeRequest { FoodItemId = foodItem.Id });
        var secondResult = await controller.Like(new SwipeRequest { FoodItemId = foodItem.Id });

        Assert.IsType<JsonResult>(firstResult);
        var json = Assert.IsType<JsonResult>(secondResult);
        Assert.Equal(1, await context.UserSelections.CountAsync());
        Assert.True(await context.UserSelections.AnyAsync(selection => selection.IsLiked));
        Assert.Equal(1, GetJsonValue<int>(json.Value!, "likedCount"));
    }

    [Fact]
    public async Task Dislike_UpdatesExistingLike_InsteadOfCreatingDuplicateSelection()
    {
        await using var context = CreateContext();
        var foodItem = await SeedFoodItemAsync(context);
        var controller = CreateController(context);

        await controller.Like(new SwipeRequest { FoodItemId = foodItem.Id });
        var result = await controller.Dislike(new SwipeRequest { FoodItemId = foodItem.Id });

        var json = Assert.IsType<JsonResult>(result);
        var selection = await context.UserSelections.SingleAsync();
        Assert.False(selection.IsLiked);
        Assert.Equal(0, GetJsonValue<int>(json.Value!, "likedCount"));
    }

    [Fact]
    public async Task Like_RejectsFoodItemFromAnotherCity()
    {
        await using var context = CreateContext();
        var foodItem = await SeedFoodItemAsync(context, city: "Bursa");
        var controller = CreateController(context, city: "İstanbul");

        var result = await controller.Like(new SwipeRequest { FoodItemId = foodItem.Id });

        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Empty(context.UserSelections);
    }

    [Fact]
    public async Task Swipe_IncludesFoodItem_WhenCategoryMatchesMealType()
    {
        await using var context = CreateContext();
        var dinner = await SeedFoodItemAsync(context, tags: "Steak", mealType: "Dinner");
        var lunch = await SeedFoodItemAsync(context, tags: "Steak", mealType: "Lunch");
        var controller = CreateController(context, categories: "Dinner");

        var result = await controller.Swipe();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<FoodItem>>(view.Model);
        Assert.Contains(model, item => item.Id == dinner.Id);
        Assert.DoesNotContain(model, item => item.Id == lunch.Id);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static WizardController CreateController(
        ApplicationDbContext context,
        string city = "İstanbul",
        string categories = "")
    {
        var session = new TestSession();
        session.SetString("SelectionSessionId", "test-session");
        session.SetString("City", city);
        session.SetString("Categories", categories);

        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        return new WizardController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static async Task<FoodItem> SeedFoodItemAsync(
        ApplicationDbContext context,
        string city = "İstanbul",
        string tags = "Dinner",
        string mealType = "Dinner")
    {
        var restaurant = new Restaurant
        {
            Name = "Test Restaurant",
            City = city,
            Address = "Test Address"
        };

        var foodItem = new FoodItem
        {
            Name = "Test Food",
            ImageUrl = "https://example.com/food.jpg",
            Tags = tags,
            MealType = mealType,
            Restaurant = restaurant
        };

        context.FoodItems.Add(foodItem);
        await context.SaveChangesAsync();

        return foodItem;
    }

    private static T GetJsonValue<T>(object value, string propertyName)
    {
        var property = value.GetType().GetProperty(propertyName);

        Assert.NotNull(property);
        return Assert.IsType<T>(property.GetValue(value));
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
