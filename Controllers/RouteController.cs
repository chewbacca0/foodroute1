using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodRoute.Data;
using FoodRoute.Models;
using System.Text;

namespace FoodRoute.Controllers;

public class RouteController : Controller
{
    private readonly ApplicationDbContext _context;

    public RouteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Route/Generate
    public async Task<IActionResult> Generate()
    {
        var sessionId = HttpContext.Session.GetString("SelectionSessionId");
        var city = HttpContext.Session.GetString("City");
        var days = HttpContext.Session.GetInt32("Days") ?? 1;

        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(city))
        {
            return RedirectToAction("Preferences", "Wizard");
        }

        // Get liked food items with restaurant info
        var likedSelections = await _context.UserSelections
            .Include(s => s.FoodItem)
            .ThenInclude(f => f.Restaurant)
            .Where(s => s.SessionId == sessionId && s.IsLiked)
            .OrderBy(s => s.CreatedAt)
            .ThenBy(s => s.Id)
            .ToListAsync();

        if (!likedSelections.Any())
        {
            TempData["Error"] = "Lütfen en az bir mekan beğenin.";
            return RedirectToAction("Swipe", "Wizard");
        }

        // Build the route
        var routeResult = BuildRoute(likedSelections, days, city);
        
        // Generate Google Maps URL and store in ViewBag
        var restaurants = likedSelections
            .Select(s => s.FoodItem.Restaurant)
            .DistinctBy(r => r.Id)
            .ToList();
        ViewBag.LikedRestaurants = restaurants;
        ViewBag.GoogleMapsUrl = GenerateRouteUrl(restaurants);

        return View("Result", routeResult);
    }

    /// <summary>
    /// Belirli bir gün için Google Maps rotası oluşturur
    /// </summary>
    /// <param name="dayNumber">Gün numarası (1'den başlar)</param>
    /// <param name="travelMode">Seyahat modu: walking, driving, bicycling, transit</param>
    public async Task<IActionResult> OpenGoogleMaps(int dayNumber = 0, string travelMode = "walking")
    {
        var sessionId = HttpContext.Session.GetString("SelectionSessionId");
        
        if (string.IsNullOrEmpty(sessionId))
        {
            return RedirectToAction("Preferences", "Wizard");
        }

        // Get liked selections with restaurant info
        var likedSelections = await _context.UserSelections
            .Include(s => s.FoodItem)
            .ThenInclude(f => f.Restaurant)
            .Where(s => s.SessionId == sessionId && s.IsLiked)
            .OrderBy(s => s.CreatedAt)
            .ThenBy(s => s.Id)
            .ToListAsync();

        if (!likedSelections.Any())
        {
            return RedirectToAction("Swipe", "Wizard");
        }

        // Get unique restaurants
        var restaurants = likedSelections
            .Select(s => s.FoodItem.Restaurant)
            .DistinctBy(r => r.Id)
            .ToList();

        // If dayNumber is specified, filter restaurants for that day
        if (dayNumber > 0)
        {
            var days = HttpContext.Session.GetInt32("Days") ?? 1;
            var routeResult = BuildRoute(likedSelections, days, "");
            
            if (dayNumber <= routeResult.Days.Count)
            {
                var dayMeals = routeResult.Days[dayNumber - 1].Meals;
                restaurants = dayMeals
                    .Select(m => m.Restaurant)
                    .DistinctBy(r => r.Id)
                    .ToList();
            }
        }

        // Generate Google Maps URL
        var url = GenerateRouteUrl(restaurants, travelMode);
        
        return Redirect(url);
    }

    /// <summary>
    /// Restoran ID listesi ile Google Maps URL'i oluşturur
    /// </summary>
    /// <param name="restaurantIds">Restoran ID listesi</param>
    /// <param name="travelMode">Seyahat modu: walking, driving, bicycling, transit</param>
    public async Task<IActionResult> OpenGoogleMapsById([FromQuery] List<int> restaurantIds, string travelMode = "walking")
    {
        if (restaurantIds == null || !restaurantIds.Any())
        {
            return BadRequest("Restoran ID listesi boş olamaz.");
        }

        // Fetch restaurants from database
        var restaurants = await _context.Restaurants
            .Where(r => restaurantIds.Contains(r.Id))
            .ToListAsync();

        if (!restaurants.Any())
        {
            return NotFound("Belirtilen restoranlar bulunamadı.");
        }

        // Sort by the order in restaurantIds
        restaurants = restaurantIds
            .Select(id => restaurants.FirstOrDefault(r => r.Id == id))
            .Where(r => r != null)
            .ToList()!;

        // Generate Google Maps URL
        var url = GenerateRouteUrl(restaurants, travelMode);
        
        return Redirect(url);
    }

    /// <summary>
    /// Google Maps Directions URL oluşturur (API Key gerekmez)
    /// Format: https://www.google.com/maps/dir/?api=1&origin=...&destination=...&waypoints=...|...&travelmode=walking
    /// </summary>
    /// <param name="restaurants">Sıralı restoran listesi</param>
    /// <param name="travelMode">Seyahat modu: walking, driving, bicycling, transit</param>
    /// <returns>Google Maps Directions URL</returns>
    private string GenerateRouteUrl(List<Restaurant> restaurants, string travelMode = "walking")
    {
        if (restaurants == null || !restaurants.Any())
        {
            // Fallback: Google Maps ana sayfası
            return "https://www.google.com/maps";
        }

        // Tek restoran varsa, sadece o konumu göster
        if (restaurants.Count == 1)
        {
            var single = restaurants[0];
            var location = FormatLocation(single);
            return $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(location)}";
        }

        // Birden fazla restoran varsa, rota oluştur
        var origin = restaurants.First();
        var destination = restaurants.Last();
        var waypoints = restaurants.Skip(1).Take(restaurants.Count - 2).ToList();

        var urlBuilder = new StringBuilder();
        urlBuilder.Append("https://www.google.com/maps/dir/?api=1");
        
        // Origin (başlangıç noktası)
        urlBuilder.Append($"&origin={Uri.EscapeDataString(FormatLocation(origin))}");
        
        // Destination (varış noktası)
        urlBuilder.Append($"&destination={Uri.EscapeDataString(FormatLocation(destination))}");
        
        // Waypoints (ara duraklar) - pipe (|) ile ayrılır
        if (waypoints.Any())
        {
            var waypointStr = string.Join("|", waypoints.Select(r => FormatLocation(r)));
            urlBuilder.Append($"&waypoints={Uri.EscapeDataString(waypointStr)}");
        }
        
        // Travel mode
        var validModes = new[] { "walking", "driving", "bicycling", "transit" };
        if (!validModes.Contains(travelMode.ToLower()))
        {
            travelMode = "walking";
        }
        urlBuilder.Append($"&travelmode={travelMode.ToLower()}");

        return urlBuilder.ToString();
    }

    /// <summary>
    /// Restoran için Google Maps'te kullanılacak konum stringi oluşturur
    /// Koordinatlar varsa koordinatları, yoksa adres kullanır
    /// </summary>
    private string FormatLocation(Restaurant restaurant)
    {
        // Koordinatlar geçerli mi kontrol et (0,0 değilse)
        if (restaurant.Latitude != 0 && restaurant.Longitude != 0)
        {
            return $"{restaurant.Latitude},{restaurant.Longitude}";
        }
        
        // Koordinat yoksa, restoran adı ve şehir bilgisi kullan
        return $"{restaurant.Name}, {restaurant.City}";
    }

    private RouteResult BuildRoute(List<UserSelection> selections, int days, string city)
    {
        var result = new RouteResult
        {
            City = city,
            TotalRestaurants = selections.Select(s => s.FoodItem.RestaurantId).Distinct().Count()
        };

        // Group food items by meal type
        var breakfastItems = selections.Where(s => s.FoodItem.MealType == "Breakfast").ToList();
        var lunchItems = selections.Where(s => s.FoodItem.MealType == "Lunch" || s.FoodItem.MealType == "StreetFood").ToList();
        var dinnerItems = selections.Where(s => s.FoodItem.MealType == "Dinner").ToList();
        var coffeeItems = selections.Where(s => s.FoodItem.MealType == "Coffee").ToList();
        var dessertItems = selections.Where(s => s.FoodItem.MealType == "Dessert").ToList();

        // Items without specific meal type go to lunch
        var otherItems = selections
            .Where(s => !new[] { "Breakfast", "Lunch", "Dinner", "Coffee", "Dessert", "StreetFood" }.Contains(s.FoodItem.MealType))
            .ToList();
        lunchItems.AddRange(otherItems);

        for (int day = 1; day <= days; day++)
        {
            var routeDay = new RouteDay
            {
                DayNumber = day,
                DayLabel = $"Gün {day}",
                Meals = new List<RouteMeal>()
            };

            // Assign breakfast
            if (breakfastItems.Any())
            {
                var breakfast = breakfastItems[(day - 1) % breakfastItems.Count];
                routeDay.Meals.Add(new RouteMeal
                {
                    MealType = "Kahvaltı 🌅",
                    FoodItem = breakfast.FoodItem,
                    Restaurant = breakfast.FoodItem.Restaurant
                });
            }

            // Assign coffee
            if (coffeeItems.Any())
            {
                var coffee = coffeeItems[(day - 1) % coffeeItems.Count];
                routeDay.Meals.Add(new RouteMeal
                {
                    MealType = "Kahve Molası ☕",
                    FoodItem = coffee.FoodItem,
                    Restaurant = coffee.FoodItem.Restaurant
                });
            }

            // Assign lunch
            if (lunchItems.Any())
            {
                var lunch = lunchItems[(day - 1) % lunchItems.Count];
                routeDay.Meals.Add(new RouteMeal
                {
                    MealType = "Öğle Yemeği 🍽️",
                    FoodItem = lunch.FoodItem,
                    Restaurant = lunch.FoodItem.Restaurant
                });
            }

            // Assign dessert
            if (dessertItems.Any())
            {
                var dessert = dessertItems[(day - 1) % dessertItems.Count];
                routeDay.Meals.Add(new RouteMeal
                {
                    MealType = "Tatlı 🍰",
                    FoodItem = dessert.FoodItem,
                    Restaurant = dessert.FoodItem.Restaurant
                });
            }

            // Assign dinner
            if (dinnerItems.Any())
            {
                var dinner = dinnerItems[(day - 1) % dinnerItems.Count];
                routeDay.Meals.Add(new RouteMeal
                {
                    MealType = "Akşam Yemeği 🌙",
                    FoodItem = dinner.FoodItem,
                    Restaurant = dinner.FoodItem.Restaurant
                });
            }

            result.Days.Add(routeDay);
        }

        return result;
    }
}
