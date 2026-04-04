using NexShop.Models;

namespace NexShop.Data;

public static class SeedData
{
    public static List<Product> GetProducts() => new()
    {
        new Product { Id = 1, Name = "Apex Runner X9", Brand = "VeloStride", Price = 189.99m, OriginalPrice = 240m,
            Category = "Footwear", Tags = new[]{"running","sport","popular"}, Stock = 24, Rating = 4.8, ReviewCount = 312,
            IsNew = false, IsTrending = true, IsFeatured = true,
            Colors = new[]{"#0a0a0b","#1a56db","#e02424"}, Sizes = new[]{"7","8","9","10","11","12"},
            Description = "Elite performance runner with AI-optimized cushioning and carbon fiber plate. Engineered for sub-3-hour marathons.",
            ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=600&q=80","https://images.unsplash.com/photo-1608231387042-66d1773070a5?w=600&q=80"} },

        new Product { Id = 2, Name = "Noir Oversized Blazer", Brand = "Maison Vex", Price = 349.00m,
            Category = "Clothing", Tags = new[]{"luxury","blazer","trending"}, Stock = 8, Rating = 4.9, ReviewCount = 87,
            IsNew = true, IsTrending = true, IsFeatured = true,
            Colors = new[]{"#1a1a2e","#f5f5dc","#8b6914"}, Sizes = new[]{"XS","S","M","L","XL"},
            Description = "Structured double-breasted blazer in premium wool-cashmere blend. Architectural shoulders, satin lapels, relaxed fit.",
            ImageUrl = "https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=600&q=80"} },

        new Product { Id = 3, Name = "Obsidian Watch Mk II", Brand = "Chronex", Price = 599.00m, OriginalPrice = 749m,
            Category = "Accessories", Tags = new[]{"watch","luxury","gift"}, Stock = 5, Rating = 4.7, ReviewCount = 203,
            IsNew = false, IsTrending = false, IsFeatured = true,
            Colors = new[]{"#1a1a1a","#c0c0c0","#8b6914"}, Sizes = new[]{"One Size"},
            Description = "Swiss-movement automatic timepiece with sapphire crystal glass, 100m water resistance, and exhibition caseback.",
            ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&q=80"} },

        new Product { Id = 4, Name = "Cloud Nine Hoodie", Brand = "SoftForm", Price = 89.99m,
            Category = "Clothing", Tags = new[]{"casual","cozy","bestseller"}, Stock = 47, Rating = 4.6, ReviewCount = 541,
            IsNew = false, IsTrending = true, IsFeatured = false,
            Colors = new[]{"#f0e6d3","#4a4e69","#2d6a4f"}, Sizes = new[]{"XS","S","M","L","XL","XXL"},
            Description = "400gsm heavyweight French terry cotton. Brushed interior, kangaroo pocket, ribbed cuffs. Pre-shrunk and enzyme-washed.",
            ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f15732ce?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1556821840-3a63f15732ce?w=600&q=80"} },

        new Product { Id = 5, Name = "Lumina Perfume No.7", Brand = "Scentara", Price = 145.00m,
            Category = "Beauty", Tags = new[]{"fragrance","luxury","gift"}, Stock = 30, Rating = 4.9, ReviewCount = 178,
            IsNew = true, IsTrending = false, IsFeatured = true,
            Colors = new[]{"#f5c842"}, Sizes = new[]{"30ml","50ml","100ml"},
            Description = "Oriental floral with top notes of neroli & bergamot, heart of jasmine & rose, base of sandalwood & amber.",
            ImageUrl = "https://images.unsplash.com/photo-1541643600914-78b084683702?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1541643600914-78b084683702?w=600&q=80"} },

        new Product { Id = 6, Name = "UrbanPack Pro 28L", Brand = "Nomadix", Price = 129.00m, OriginalPrice = 160m,
            Category = "Bags", Tags = new[]{"backpack","travel","work"}, Stock = 19, Rating = 4.5, ReviewCount = 394,
            IsNew = false, IsTrending = true, IsFeatured = false,
            Colors = new[]{"#2d3436","#0984e3","#00b894"}, Sizes = new[]{"One Size"},
            Description = "Weatherproof 28L daypack with laptop sleeve, hidden pockets, and luggage passthrough. TSA-friendly.",
            ImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=600&q=80"} },

        new Product { Id = 7, Name = "Aura Wireless Buds", Brand = "SoundCore", Price = 219.99m, OriginalPrice = 279m,
            Category = "Tech", Tags = new[]{"audio","wireless","anc"}, Stock = 55, Rating = 4.7, ReviewCount = 829,
            IsNew = false, IsTrending = true, IsFeatured = true,
            Colors = new[]{"#ffffff","#1a1a1a","#6c5ce7"}, Sizes = new[]{"One Size"},
            Description = "40hr battery, adaptive ANC, spatial audio. Custom 11mm dynamic drivers. IPX5 sweat resistant.",
            ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&q=80"} },

        new Product { Id = 8, Name = "Cascade Skincare Set", Brand = "Botaniq", Price = 98.00m,
            Category = "Beauty", Tags = new[]{"skincare","natural","gift"}, Stock = 22, Rating = 4.8, ReviewCount = 267,
            IsNew = true, IsTrending = false, IsFeatured = false,
            Colors = new[]{"#a8d5a2"}, Sizes = new[]{"One Size"},
            Description = "3-step morning ritual: vitamin C serum, hyaluronic moisturizer, SPF50 sunscreen. Vegan & cruelty-free.",
            ImageUrl = "https://images.unsplash.com/photo-1556228578-8c89e6adf883?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1556228578-8c89e6adf883?w=600&q=80"} },

        new Product { Id = 9, Name = "Phantom Slim Wallet", Brand = "Veltro", Price = 65.00m,
            Category = "Accessories", Tags = new[]{"wallet","minimalist","gift"}, Stock = 40, Rating = 4.6, ReviewCount = 155,
            IsNew = false, IsTrending = false, IsFeatured = false,
            Colors = new[]{"#2d2d2d","#8b6914","#c0392b"}, Sizes = new[]{"One Size"},
            Description = "Full-grain leather cardholder with RFID blocking. Holds 8 cards + cash fold. 6mm thin profile.",
            ImageUrl = "https://images.unsplash.com/photo-1627123424574-724758594e93?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1627123424574-724758594e93?w=600&q=80"} },

        new Product { Id = 10, Name = "Drift Linen Trousers", Brand = "Maison Vex", Price = 175.00m,
            Category = "Clothing", Tags = new[]{"linen","summer","luxury"}, Stock = 14, Rating = 4.4, ReviewCount = 92,
            IsNew = true, IsTrending = false, IsFeatured = false,
            Colors = new[]{"#f5f5dc","#4a4e69","#2d6a4f"}, Sizes = new[]{"28","30","32","34","36"},
            Description = "Wide-leg pure Irish linen. Elasticated back waist, side pockets, front pleats. Dry clean recommended.",
            ImageUrl = "https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=600&q=80"} },

        new Product { Id = 11, Name = "Kinetic Smart Ring", Brand = "PulseWear", Price = 299.00m,
            Category = "Tech", Tags = new[]{"fitness","health","wearable"}, Stock = 11, Rating = 4.5, ReviewCount = 341,
            IsNew = true, IsTrending = true, IsFeatured = false,
            Colors = new[]{"#1a1a1a","#c0c0c0","#8b6914"}, Sizes = new[]{"6","7","8","9","10","11"},
            Description = "Track heart rate, sleep, HRV, SpO2, temperature. 7-day battery. Titanium shell, 50m waterproof.",
            ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&q=80"} },

        new Product { Id = 12, Name = "Velvet Cap Toe Oxfords", Brand = "Archetti", Price = 425.00m, OriginalPrice = 510m,
            Category = "Footwear", Tags = new[]{"dress","luxury","formal"}, Stock = 7, Rating = 4.9, ReviewCount = 64,
            IsNew = false, IsTrending = false, IsFeatured = false,
            Colors = new[]{"#2d2d2d","#8b6914","#800000"}, Sizes = new[]{"7","8","9","10","11","12"},
            Description = "Goodyear-welted construction. Calf leather upper, leather sole. Made in Florence. Resoleable.",
            ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=600&q=80",
            ImageGallery = new[]{"https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=600&q=80"} },
    };

    public static List<SocialProofEvent> GetSocialProofEvents() => new()
    {
        new() { UserName = "Maya R.", UserLocation = "Austin, TX", ProductName = "Apex Runner X9", Action = "purchased", SecondsAgo = 12 },
        new() { UserName = "James K.", UserLocation = "London, UK", ProductName = "Obsidian Watch Mk II", Action = "added to wishlist", SecondsAgo = 34 },
        new() { UserName = "Sofia M.", UserLocation = "Paris, FR", ProductName = "Noir Oversized Blazer", Action = "purchased", SecondsAgo = 67 },
        new() { UserName = "Aiden T.", UserLocation = "Toronto, CA", ProductName = "Aura Wireless Buds", Action = "purchased", SecondsAgo = 89 },
        new() { UserName = "Priya N.", UserLocation = "Mumbai, IN", ProductName = "Lumina Perfume No.7", Action = "added to wishlist", SecondsAgo = 120 },
        new() { UserName = "Lucas B.", UserLocation = "Berlin, DE", ProductName = "Cloud Nine Hoodie", Action = "purchased", SecondsAgo = 180 },
    };
}
