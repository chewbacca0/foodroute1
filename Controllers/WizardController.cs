using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodRoute.Data;
using FoodRoute.Models;

namespace FoodRoute.Controllers;

public class WizardController : Controller
{
    private readonly ApplicationDbContext _context;

    public WizardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Wizard/Preferences
    public IActionResult Preferences()
    {
        return View(new UserPreferences());
    }

    // GET: Wizard/Reset
    public async Task<IActionResult> Reset()
    {
        var sessionId = HttpContext.Session.GetString("SelectionSessionId");

        if (!string.IsNullOrEmpty(sessionId))
        {
            var previousSelections = await _context.UserSelections
                .Where(s => s.SessionId == sessionId)
                .ToListAsync();

            if (previousSelections.Any())
            {
                _context.UserSelections.RemoveRange(previousSelections);
                await _context.SaveChangesAsync();
            }
        }

        HttpContext.Session.Remove("City");
        HttpContext.Session.Remove("Days");
        HttpContext.Session.Remove("Categories");
        HttpContext.Session.Remove("SelectionSessionId");

        return RedirectToAction("Preferences");
    }

    // POST: Wizard/Preferences
    [HttpPost]
    public IActionResult Preferences(UserPreferences model)
    {
        if (string.IsNullOrEmpty(model.City))
        {
            ModelState.AddModelError("City", "Lütfen bir şehir seçin.");
            return View(model);
        }

        // Store preferences in session
        HttpContext.Session.SetString("City", model.City);
        HttpContext.Session.SetInt32("Days", model.Days);
        HttpContext.Session.SetString("Categories", string.Join(",", model.Categories ?? new List<string>()));
        
        // Generate a unique session ID for selections
        var sessionId = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("SelectionSessionId", sessionId);

        return RedirectToAction("Swipe");
    }

    // GET: Wizard/Swipe
    public async Task<IActionResult> Swipe()
    {
        var city = HttpContext.Session.GetString("City");
        var categoriesStr = HttpContext.Session.GetString("Categories");
        var sessionId = HttpContext.Session.GetString("SelectionSessionId");

        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(sessionId))
        {
            return RedirectToAction("Preferences");
        }

        var categories = string.IsNullOrEmpty(categoriesStr) 
            ? new List<string>() 
            : categoriesStr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        // Get already swiped food items
        var swipedIds = await _context.UserSelections
            .Where(s => s.SessionId == sessionId)
            .Select(s => s.FoodItemId)
            .ToListAsync();

        // Get food items matching city and categories
        var query = _context.FoodItems
            .Include(f => f.Restaurant)
            .Where(f => f.Restaurant.City == city && !swipedIds.Contains(f.Id));

        // Filter by categories if any selected
        if (categories.Any())
        {
            query = query.Where(f => categories.Any(c => f.Tags.Contains(c)));
        }

        var foodItems = await query.ToListAsync();

        // Get liked count for UI
        var likedCount = await _context.UserSelections
            .Where(s => s.SessionId == sessionId && s.IsLiked)
            .CountAsync();

        ViewBag.LikedCount = likedCount;
        ViewBag.SessionId = sessionId;
        ViewBag.City = city;

        return View(foodItems);
    }

    // POST: Wizard/Like (AJAX)
    [HttpPost]
    public async Task<IActionResult> Like([FromBody] SwipeRequest request)
    {
        var sessionId = HttpContext.Session.GetString("SelectionSessionId");
        if (string.IsNullOrEmpty(sessionId))
        {
            return Json(new { success = false, message = "Session expired" });
        }

        var selection = new UserSelection
        {
            SessionId = sessionId,
            FoodItemId = request.FoodItemId,
            IsLiked = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserSelections.Add(selection);
        await _context.SaveChangesAsync();

        var likedCount = await _context.UserSelections
            .Where(s => s.SessionId == sessionId && s.IsLiked)
            .CountAsync();

        return Json(new { success = true, likedCount });
    }

    // POST: Wizard/Dislike (AJAX)
    [HttpPost]
    public async Task<IActionResult> Dislike([FromBody] SwipeRequest request)
    {
        var sessionId = HttpContext.Session.GetString("SelectionSessionId");
        if (string.IsNullOrEmpty(sessionId))
        {
            return Json(new { success = false, message = "Session expired" });
        }

        var selection = new UserSelection
        {
            SessionId = sessionId,
            FoodItemId = request.FoodItemId,
            IsLiked = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserSelections.Add(selection);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}

public class SwipeRequest
{
    public int FoodItemId { get; set; }
}
