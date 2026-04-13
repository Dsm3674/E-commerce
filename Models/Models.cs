using System.ComponentModel.DataAnnotations;

namespace NexShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal Price { get; set; }

    [Range(0, 999999)]
    public decimal? OriginalPrice { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    public string[] Tags { get; set; } = Array.Empty<string>();
    public string ImageUrl { get; set; } = string.Empty;
    public string[] ImageGallery { get; set; } = Array.Empty<string>();

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Range(0, 5)]
    public double Rating { get; set; }

    [Range(0, int.MaxValue)]
    public int ReviewCount { get; set; }

    public bool IsNew { get; set; }
    public bool IsTrending { get; set; }
    public bool IsFeatured { get; set; }

    public string[] Colors { get; set; } = Array.Empty<string>();
    public string[] Sizes { get; set; } = Array.Empty<string>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CartItem
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Range(1, 999)]
    public int Quantity { get; set; }

    public string SelectedColor { get; set; } = string.Empty;
    public string SelectedSize { get; set; } = string.Empty;
}

public class Cart
{
    public string SessionId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();

    public decimal Subtotal => Items.Sum(i => (i.Product?.Price ?? 0m) * i.Quantity);
    public decimal Shipping => Subtotal > 150m ? 0m : 12.99m;
    public decimal Total => Subtotal + Shipping;
    public int ItemCount => Items.Sum(i => i.Quantity);
}

public class AddToCartRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 99)]
    public int Quantity { get; set; } = 1;

    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
}

public class UpdateCartQuantityRequest
{
    [Range(0, 99)]
    public int Quantity { get; set; }
}

public class RemoveCartItemRequest
{
    [Required]
    public string Color { get; set; } = string.Empty;

    [Required]
    public string Size { get; set; } = string.Empty;
}

public class AiChatRequest
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;

    public string[] ConversationHistory { get; set; } = Array.Empty<string>();
}

public class ProductListResponse
{
    public List<Product> Products { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class ProductDetailsResponse
{
    public Product? Product { get; set; }
    public List<Product> Related { get; set; } = new();
}

public class ChatResponse
{
    public string Reply { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = new();
    public bool UsedFallback { get; set; }
}

public class ApiErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Details { get; set; }
}

public class LoyaltyProfile
{
    public string UserId { get; set; } = "guest";
    public int Points { get; set; } = 340;
    public string Tier { get; set; } = "Silver";
    public int PointsToNextTier { get; set; } = 160;
    public string NextTier { get; set; } = "Gold";
    public string[] Badges { get; set; } = { "Early Adopter", "Style Maven" };
    public int TotalOrders { get; set; } = 7;
}

public class SocialProofEvent
{
    public string UserName { get; set; } = string.Empty;
    public string UserLocation { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Action { get; set; } = "purchased";
    public int SecondsAgo { get; set; }
}
