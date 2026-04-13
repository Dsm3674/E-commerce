using Microsoft.AspNetCore.Mvc;
using NexShop.Data;
using NexShop.Models;

namespace NexShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> Products = SeedData.GetProducts();

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] string? tag = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string sort = "featured",
        [FromQuery] bool? isTrending = null,
        [FromQuery] bool? isNew = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 12;
        if (pageSize > 50) pageSize = 50;

        var query = Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Brand.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Tags.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var normalizedTag = tag.Trim();

            query = query.Where(p =>
                p.Tags.Any(t => t.Equals(normalizedTag, StringComparison.OrdinalIgnoreCase)));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (isTrending.HasValue)
        {
            query = query.Where(p => p.IsTrending == isTrending.Value);
        }

        if (isNew.HasValue)
        {
            query = query.Where(p => p.IsNew == isNew.Value);
        }

        query = sort.Trim().ToLowerInvariant() switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.Rating).ThenByDescending(p => p.ReviewCount),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.IsFeatured)
                      .ThenByDescending(p => p.IsTrending)
                      .ThenByDescending(p => p.ReviewCount)
        };

        var total = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new ProductListResponse
        {
            Products = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound(new ApiErrorResponse
            {
                Error = "Product not found"
            });
        }

        var related = Products
            .Where(p => p.Id != id && p.Category.Equals(product.Category, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(p => p.Rating)
            .ThenByDescending(p => p.ReviewCount)
            .Take(4)
            .ToList();

        return Ok(new ProductDetailsResponse
        {
            Product = product,
            Related = related
        });
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var categories = Products
            .Select(p => p.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        return Ok(categories);
    }

    [HttpGet("featured")]
    public IActionResult GetFeatured()
    {
        var featured = Products
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.Rating)
            .ThenByDescending(p => p.ReviewCount)
            .Take(6)
            .ToList();

        return Ok(featured);
    }

    [HttpGet("trending")]
    public IActionResult GetTrending()
    {
        var trending = Products
            .Where(p => p.IsTrending)
            .OrderByDescending(p => p.Rating)
            .ThenByDescending(p => p.ReviewCount)
            .Take(8)
            .ToList();

        return Ok(trending);
    }

    [HttpGet("social-proof")]
    public IActionResult GetSocialProof()
    {
        return Ok(SeedData.GetSocialProofEvents());
    }
}
