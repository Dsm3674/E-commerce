using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using NexShop.Data;
using NexShop.Models;

namespace NexShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, Cart> Carts = new();
    private static readonly List<Product> Products = SeedData.GetProducts();

    private Cart GetOrCreateCart(string sessionId)
    {
        var normalizedSessionId = string.IsNullOrWhiteSpace(sessionId)
            ? "guest"
            : sessionId.Trim();

        return Carts.GetOrAdd(normalizedSessionId, id => new Cart
        {
            SessionId = id
        });
    }

    [HttpGet("{sessionId}")]
    public IActionResult GetCart(string sessionId)
    {
        return Ok(GetOrCreateCart(sessionId));
    }

    [HttpPost("{sessionId}/add")]
    public IActionResult AddItem(string sessionId, [FromBody] AddToCartRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var product = Products.FirstOrDefault(p => p.Id == request.ProductId);
        if (product is null)
        {
            return NotFound(new ApiErrorResponse
            {
                Error = "Product not found"
            });
        }

        var selectedColor = NormalizeVariant(request.Color);
        var selectedSize = NormalizeVariant(request.Size);

        if (!IsValidVariant(product.Colors, selectedColor))
        {
            return BadRequest(new ApiErrorResponse
            {
                Error = "Invalid color selection"
            });
        }

        if (!IsValidVariant(product.Sizes, selectedSize))
        {
            return BadRequest(new ApiErrorResponse
            {
                Error = "Invalid size selection"
            });
        }

        var cart = GetOrCreateCart(sessionId);

        lock (cart)
        {
            var existing = cart.Items.FirstOrDefault(i =>
                i.ProductId == request.ProductId &&
                string.Equals(i.SelectedColor, selectedColor, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(i.SelectedSize, selectedSize, StringComparison.OrdinalIgnoreCase));

            var requestedTotalQuantity = (existing?.Quantity ?? 0) + request.Quantity;

            if (requestedTotalQuantity > product.Stock)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Error = "Insufficient stock",
                    Details = $"Only {product.Stock} units available for this product."
                });
            }

            if (existing is not null)
            {
                existing.Quantity = requestedTotalQuantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = request.Quantity,
                    SelectedColor = selectedColor,
                    SelectedSize = selectedSize
                });
            }
        }

        return Ok(cart);
    }

    [HttpPut("{sessionId}/update/{productId}")]
    public IActionResult UpdateQuantity(
        string sessionId,
        int productId,
        [FromBody] UpdateCartQuantityRequest request,
        [FromQuery] string color,
        [FromQuery] string size)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var cart = GetOrCreateCart(sessionId);
        var normalizedColor = NormalizeVariant(color);
        var normalizedSize = NormalizeVariant(size);

        lock (cart)
        {
            var item = cart.Items.FirstOrDefault(i =>
                i.ProductId == productId &&
                string.Equals(i.SelectedColor, normalizedColor, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(i.SelectedSize, normalizedSize, StringComparison.OrdinalIgnoreCase));

            if (item is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Error = "Cart item not found"
                });
            }

            if (request.Quantity == 0)
            {
                cart.Items.Remove(item);
                return Ok(cart);
            }

            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Error = "Product not found"
                });
            }

            if (request.Quantity > product.Stock)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Error = "Insufficient stock",
                    Details = $"Only {product.Stock} units available for this product."
                });
            }

            item.Quantity = request.Quantity;
        }

        return Ok(cart);
    }

    [HttpDelete("{sessionId}/remove/{productId}")]
    public IActionResult RemoveItem(
        string sessionId,
        int productId,
        [FromQuery] string color,
        [FromQuery] string size)
    {
        var cart = GetOrCreateCart(sessionId);
        var normalizedColor = NormalizeVariant(color);
        var normalizedSize = NormalizeVariant(size);

        lock (cart)
        {
            var item = cart.Items.FirstOrDefault(i =>
                i.ProductId == productId &&
                string.Equals(i.SelectedColor, normalizedColor, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(i.SelectedSize, normalizedSize, StringComparison.OrdinalIgnoreCase));

            if (item is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Error = "Cart item not found"
                });
            }

            cart.Items.Remove(item);
        }

        return Ok(cart);
    }

    [HttpDelete("{sessionId}/clear")]
    public IActionResult ClearCart(string sessionId)
    {
        var cart = GetOrCreateCart(sessionId);

        lock (cart)
        {
            cart.Items.Clear();
        }

        return Ok(cart);
    }

    [HttpGet("{sessionId}/recommendations")]
    public IActionResult GetRecommendations(string sessionId)
    {
        var cart = GetOrCreateCart(sessionId);
        var cartProductIds = cart.Items.Select(i => i.ProductId).ToHashSet();

        var cartCategories = cart.Items
            .Select(i => i.Product?.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var recommendations = Products
            .Where(p => !cartProductIds.Contains(p.Id))
            .Where(p => cartCategories.Count == 0 || cartCategories.Contains(p.Category, StringComparer.OrdinalIgnoreCase))
            .OrderByDescending(p => p.IsTrending)
            .ThenByDescending(p => p.Rating)
            .ThenByDescending(p => p.ReviewCount)
            .Take(3)
            .ToList();

        if (!recommendations.Any())
        {
            recommendations = Products
                .Where(p => !cartProductIds.Contains(p.Id))
                .OrderByDescending(p => p.IsTrending)
                .ThenByDescending(p => p.Rating)
                .Take(3)
                .ToList();
        }

        return Ok(recommendations);
    }

    private static string NormalizeVariant(string? value)
    {
        return (value ?? string.Empty).Trim();
    }

    private static bool IsValidVariant(string[] options, string selected)
    {
        if (options.Length == 0)
        {
            return true;
        }

        return options.Any(o => string.Equals(o, selected, StringComparison.OrdinalIgnoreCase));
    }
}
