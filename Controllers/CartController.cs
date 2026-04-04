using Microsoft.AspNetCore.Mvc;
using NexShop.Data;
using NexShop.Models;

namespace NexShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private static readonly Dictionary<string, Cart> _carts = new();
    private static readonly List<Product> _products = SeedData.GetProducts();

    private Cart GetCart(string sessionId)
    {
        if (!_carts.ContainsKey(sessionId))
            _carts[sessionId] = new Cart { SessionId = sessionId };
        return _carts[sessionId];
    }

    [HttpGet("{sessionId}")]
    public IActionResult GetCart(string sessionId) => Ok(GetCart(sessionId));

    [HttpPost("{sessionId}/add")]
    public IActionResult AddItem(string sessionId, [FromBody] AddToCartRequest request)
    {
        var cart = GetCart(sessionId);
        var product = _products.FirstOrDefault(p => p.Id == request.ProductId);
        if (product == null) return NotFound("Product not found");
        if (product.Stock < request.Quantity) return BadRequest("Insufficient stock");

        var existing = cart.Items.FirstOrDefault(i =>
            i.ProductId == request.ProductId &&
            i.SelectedColor == request.Color &&
            i.SelectedSize == request.Size);

        if (existing != null)
            existing.Quantity += request.Quantity;
        else
            cart.Items.Add(new CartItem
            {
                ProductId = request.ProductId,
                Product = product,
                Quantity = request.Quantity,
                SelectedColor = request.Color,
                SelectedSize = request.Size
            });

        return Ok(cart);
    }

    [HttpDelete("{sessionId}/remove/{productId}")]
    public IActionResult RemoveItem(string sessionId, int productId)
    {
        var cart = GetCart(sessionId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null) cart.Items.Remove(item);
        return Ok(cart);
    }

    [HttpPut("{sessionId}/update/{productId}")]
    public IActionResult UpdateQuantity(string sessionId, int productId, [FromBody] int quantity)
    {
        var cart = GetCart(sessionId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return NotFound();
        if (quantity <= 0) cart.Items.Remove(item);
        else item.Quantity = quantity;
        return Ok(cart);
    }

    [HttpDelete("{sessionId}/clear")]
    public IActionResult ClearCart(string sessionId)
    {
        if (_carts.ContainsKey(sessionId)) _carts[sessionId].Items.Clear();
        return Ok(GetCart(sessionId));
    }

    [HttpGet("{sessionId}/recommendations")]
    public IActionResult GetRecommendations(string sessionId)
    {
        var cart = GetCart(sessionId);
        var cartCategories = cart.Items.Select(i => i.Product?.Category).Distinct().ToList();
        var recommendations = _products
            .Where(p => cartCategories.Contains(p.Category) && !cart.Items.Any(i => i.ProductId == p.Id))
            .OrderByDescending(p => p.Rating)
            .Take(3)
            .ToList();
        if (!recommendations.Any())
            recommendations = _products.Where(p => p.IsTrending).Take(3).ToList();
        return Ok(recommendations);
    }
}
