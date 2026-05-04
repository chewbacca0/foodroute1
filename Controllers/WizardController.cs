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

        HttpContext.Session.SetString("City", model.City);
        HttpContext.Session.SetInt32("Days", model.Days);
        HttpContext.Session.SetString("Categories", string.Join(",", model.Categories ?? new List<string>()));

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

        var swipedIds = await _context.UserSelections
            .Where(s => s.SessionId == sessionId)
            .Select(s => s.FoodItemId)
            .ToListAsync();

        var query = _context.FoodItems
            .Include(f => f.Restaurant)
            .Where(f => f.Restaurant.City == city && !swipedIds.Contains(f.Id));

        if (categories.Any())
        {
            query = query.Where(f => categories.Any(c => f.Tags.Contains(c)));
        }

        var foodItems = await query.ToListAsync();

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
    public Task<IActionResult> Like([FromBody] SwipeRequest request)
    {
        return SaveSwipeAsync(request, isLiked: true);
    }

    // POST: Wizard/Dislike (AJAX)
    [HttpPost]
    public Task<IActionResult> Dislike([FromBody] SwipeRequest request)
    {
        return SaveSwipeAsync(request, isLiked: false);
    }

    private async Task<IActionResult> SaveSwipeAsync(SwipeRequest? request, bool isLiked)
    {
        var sessionId = HttpContext.Session.GetString("SelectionSessionId");
        if (string.IsNullOrEmpty(sessionId))
        {
            return StatusCode(StatusCodes.Status401Unauthorized, new { success = false, message = "Session expired" });
        }

        if (request is null || request.FoodItemId <= 0)
        {
            return BadRequest(new { success = false, message = "Invalid food item." });
        }

        var city = HttpContext.Session.GetString("City");
        var foodItemExists = await _context.FoodItems
            .Include(f => f.Restaurant)
            .AnyAsync(f => f.Id == request.FoodItemId && (string.IsNullOrEmpty(city) || f.Restaurant.City == city));

        if (!foodItemExists)
        {
            return NotFound(new { success = false, message = "Food item was not found." });
        }

        var selection = await _context.UserSelections
            .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.FoodItemId == request.FoodItemId);

        if (selection is null)
        {
            selection = new UserSelection
            {
                SessionId = sessionId,
                FoodItemId = request.FoodItemId,
                IsLiked = isLiked,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSelections.Add(selection);
        }
        else
        {
            selection.IsLiked = isLiked;
            selection.CreatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var likedCount = await _context.UserSelections
            .Where(s => s.SessionId == sessionId && s.IsLiked)
            .CountAsync();

        return Json(new { success = true, likedCount });
    }
}

public class SwipeRequest
{
    public int FoodItemId { get; set; }
}
