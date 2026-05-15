using FoodRoute.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodRoute.Controllers;

public class FoodImageController : Controller
{
    private readonly ApplicationDbContext _context;

    public FoodImageController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Show(int id)
    {
        var food = await _context.FoodItems
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new
            {
                f.ImageUrl
            })
            .FirstOrDefaultAsync();

        if (food is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(food.ImageUrl))
        {
            return Redirect(food.ImageUrl);
        }

        return Redirect("/images/placeholder-food.svg");
    }
}
