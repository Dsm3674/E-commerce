using Microsoft.AspNetCore.Mvc;
using NexShop.Data;
using NexShop.Models;

namespace NexShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = SeedData.GetProducts();

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] string? tag = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? sort = "featured",
        [FromQuery] bool? isTrending = null,
        [FromQuery] bool? isNew = null)
    {
        var query = _products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(tag))
            query = query.Where(p => p.Tags.Contains(tag.ToLower()));

        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
        if (isTrending.HasValue) query = query.Where(p => p.IsTrending == isTrending.Value);
        if (isNew.HasValue) query = query.Where(p => p.IsNew == isNew.Value);

        query = sort switch
        {
            "price_asc"   => query.OrderBy(p => p.Price),
            "price_desc"  => query.OrderByDescending(p => p.Price),
            "rating"      => query.OrderByDescending(p => p.Rating),
            "newest"      => query.OrderByDescending(p => p.CreatedAt),
            _             => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.ReviewCount)
        };

        return Ok(new { products = query.ToList(), total = query.Count() });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();
        var related = _products.Where(p => p.Category == product.Category && p.Id != id).Take(4).ToList();
        return Ok(new { product, related });
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var categories = _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        return Ok(categories);
    }

    [HttpGet("featured")]
    public IActionResult GetFeatured() =>
        Ok(_products.Where(p => p.IsFeatured).Take(6).ToList());

    [HttpGet("trending")]
    public IActionResult GetTrending() =>
        Ok(_products.Where(p => p.IsTrending).Take(8).ToList());

    [HttpGet("social-proof")]
    public IActionResult GetSocialProof() =>
        Ok(SeedData.GetSocialProofEvents());
}
