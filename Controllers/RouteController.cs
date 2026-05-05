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
        var restaurants = GetOrderedRouteRestaurants(routeResult);
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

        var routeResultForAllDays = BuildRoute(likedSelections, HttpContext.Session.GetInt32("Days") ?? 1, "");
        var restaurants = GetOrderedRouteRestaurants(routeResultForAllDays);

        // If dayNumber is specified, filter restaurants for that day
        if (dayNumber > 0)
        {
            if (dayNumber <= routeResultForAllDays.Days.Count)
            {
                var dayMeals = routeResultForAllDays.Days[dayNumber - 1].Meals;
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

        var mealOrder = new[] { "Breakfast", "Lunch", "Coffee", "Dinner", "Dessert" };
        var originalPools = mealOrder.ToDictionary(
            mealType => mealType,
            mealType => selections
                .Where(selection => NormalizeMealType(selection.FoodItem.MealType) == mealType)
                .OrderBy(selection => selection.CreatedAt)
                .ThenBy(selection => selection.Id)
                .ToList());

        var remainingPools = originalPools.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToList());
        var usedRouteRestaurantIds = new HashSet<int>();

        for (int day = 1; day <= days; day++)
        {
            var routeDay = new RouteDay
            {
                DayNumber = day,
                DayLabel = $"Gün {day}",
                Meals = new List<RouteMeal>()
            };

            var usedRestaurantIds = new HashSet<int>();
            Restaurant? previousRestaurant = null;

            foreach (var mealType in mealOrder)
            {
                var selection = PickRouteSelection(
                    remainingPools[mealType],
                    previousRestaurant,
                    usedRestaurantIds,
                    usedRouteRestaurantIds);

                if (selection is null)
                {
                    continue;
                }

                remainingPools[mealType].Remove(selection);
                usedRestaurantIds.Add(selection.FoodItem.RestaurantId);
                usedRouteRestaurantIds.Add(selection.FoodItem.RestaurantId);
                previousRestaurant = selection.FoodItem.Restaurant;

                routeDay.Meals.Add(new RouteMeal
                {
                    MealType = GetMealLabel(mealType),
                    FoodItem = selection.FoodItem,
                    Restaurant = selection.FoodItem.Restaurant
                });
            }

            result.Days.Add(routeDay);
        }

        return result;
    }

    private static string NormalizeMealType(string? mealType)
    {
        return mealType switch
        {
            "Breakfast" => "Breakfast",
            "Coffee" => "Coffee",
            "Dinner" => "Dinner",
            "Dessert" => "Dessert",
            "Lunch" or "StreetFood" => "Lunch",
            _ => "Lunch"
        };
    }

    private static string GetMealLabel(string mealType)
    {
        return mealType switch
        {
            "Breakfast" => "Kahvaltı 🌅",
            "Coffee" => "Kahve Molası ☕",
            "Dinner" => "Akşam Yemeği 🌙",
            "Dessert" => "Tatlı 🍰",
            _ => "Öğle Yemeği 🍽️"
        };
    }

    private static UserSelection? PickRouteSelection(
        List<UserSelection> remainingPool,
        Restaurant? previousRestaurant,
        HashSet<int> usedRestaurantIds,
        HashSet<int> usedRouteRestaurantIds)
    {
        var candidates = remainingPool
            .Where(selection => !usedRouteRestaurantIds.Contains(selection.FoodItem.RestaurantId))
            .ToList();

        if (!candidates.Any())
        {
            return null;
        }

        var uniqueRestaurantCandidates = candidates
            .Where(selection => !usedRestaurantIds.Contains(selection.FoodItem.RestaurantId))
            .ToList();

        if (uniqueRestaurantCandidates.Any())
        {
            candidates = uniqueRestaurantCandidates;
        }

        if (previousRestaurant is null || !HasCoordinates(previousRestaurant))
        {
            return candidates
                .OrderBy(selection => selection.CreatedAt)
                .ThenBy(selection => selection.Id)
                .FirstOrDefault();
        }

        return candidates
            .OrderBy(selection => DistanceInKilometers(previousRestaurant, selection.FoodItem.Restaurant))
            .ThenBy(selection => selection.CreatedAt)
            .ThenBy(selection => selection.Id)
            .FirstOrDefault();
    }

    private static List<Restaurant> GetOrderedRouteRestaurants(RouteResult routeResult)
    {
        return routeResult.Days
            .SelectMany(day => day.Meals)
            .Select(meal => meal.Restaurant)
            .DistinctBy(restaurant => restaurant.Id)
            .ToList();
    }

    private static bool HasCoordinates(Restaurant restaurant)
    {
        return restaurant.Latitude != 0 && restaurant.Longitude != 0;
    }

    private static double DistanceInKilometers(Restaurant from, Restaurant to)
    {
        if (!HasCoordinates(from) || !HasCoordinates(to))
        {
            return double.MaxValue;
        }

        const double earthRadiusKilometers = 6371;
        var latitudeDistance = ToRadians(to.Latitude - from.Latitude);
        var longitudeDistance = ToRadians(to.Longitude - from.Longitude);
        var fromLatitude = ToRadians(from.Latitude);
        var toLatitude = ToRadians(to.Latitude);

        var a = Math.Sin(latitudeDistance / 2) * Math.Sin(latitudeDistance / 2)
            + Math.Cos(fromLatitude) * Math.Cos(toLatitude)
            * Math.Sin(longitudeDistance / 2) * Math.Sin(longitudeDistance / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKilometers * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
