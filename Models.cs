namespace NexShop.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Category { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string ImageUrl { get; set; } = string.Empty;
    public string[] ImageGallery { get; set; } = Array.Empty<string>();
    public int Stock { get; set; }
    public double Rating { get; set; }
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
    public int Quantity { get; set; }
    public string SelectedColor { get; set; } = string.Empty;
    public string SelectedSize { get; set; } = string.Empty;
}

public class Cart
{
    public string SessionId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();
    public decimal Subtotal => Items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity);
    public decimal Shipping => Subtotal > 150 ? 0 : 12.99m;
    public decimal Total => Subtotal + Shipping;
    public int ItemCount => Items.Sum(i => i.Quantity);
}

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
}

public class AiChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string[] ConversationHistory { get; set; } = Array.Empty<string>();
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
