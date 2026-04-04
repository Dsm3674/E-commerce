using Microsoft.AspNetCore.Mvc;
using NexShop.Data;
using NexShop.Models;
using System.Text;
using System.Text.Json;

namespace NexShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private static readonly List<Product> _products = SeedData.GetProducts();
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public AiController(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] AiChatRequest request)
    {
        var apiKey = _config["Anthropic:ApiKey"] ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
            return Ok(new { reply = GetFallbackReply(request.Message), products = GetRelevantProducts(request.Message) });

        var productContext = JsonSerializer.Serialize(_products.Select(p => new {
            p.Id, p.Name, p.Brand, p.Category, p.Price, p.Tags, p.Rating, p.Stock, p.IsNew, p.IsTrending
        }));

        var systemPrompt = $@"You are NexAI, the intelligent shopping assistant for NexShop — a premium AI-powered e-commerce store.
Be helpful, concise, and stylish. You have deep knowledge of our product catalog.

Available products: {productContext}

When users ask about products, suggest relevant ones by name. Keep replies under 80 words.
Format product suggestions clearly. Be warm, knowledgeable, and on-brand (luxury + accessible).
If asked about sizing, shipping, returns — be helpful and positive.";

        var messages = new List<object>();
        for (int i = 0; i < request.ConversationHistory.Length - 1; i += 2)
        {
            if (i + 1 < request.ConversationHistory.Length)
            {
                messages.Add(new { role = "user", content = request.ConversationHistory[i] });
                messages.Add(new { role = "assistant", content = request.ConversationHistory[i + 1] });
            }
        }
        messages.Add(new { role = "user", content = request.Message });

        var payload = new { model = "claude-sonnet-4-20250514", max_tokens = 300, system = systemPrompt, messages };
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var response = await client.PostAsync("https://api.anthropic.com/v1/messages",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
            return Ok(new { reply = GetFallbackReply(request.Message), products = GetRelevantProducts(request.Message) });

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var reply = doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString() ?? "";
        return Ok(new { reply, products = GetRelevantProducts(request.Message) });
    }

    [HttpGet("recommendations/{productId}")]
    public IActionResult GetRecommendations(int productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product == null) return NotFound();
        var similar = _products
            .Where(p => p.Id != productId && (p.Category == product.Category || p.Tags.Intersect(product.Tags).Any()))
            .OrderByDescending(p => p.Rating)
            .Take(4)
            .ToList();
        return Ok(similar);
    }

    [HttpGet("loyalty")]
    public IActionResult GetLoyalty() => Ok(new LoyaltyProfile());

    private List<Product> GetRelevantProducts(string message)
    {
        var lower = message.ToLower();
        return _products
            .Where(p =>
                p.Name.ToLower().Contains(lower) ||
                p.Category.ToLower().Contains(lower) ||
                p.Tags.Any(t => lower.Contains(t)) ||
                lower.Contains(p.Category.ToLower()))
            .Take(3)
            .ToList();
    }

    private static string GetFallbackReply(string message) => message.ToLower() switch
    {
        var m when m.Contains("shoe") || m.Contains("sneaker") || m.Contains("runner") =>
            "Our Apex Runner X9 is trending right now — elite cushioning, carbon plate, and three colorways. Would you like to see more footwear?",
        var m when m.Contains("watch") =>
            "The Obsidian Watch Mk II is stunning — Swiss movement, sapphire crystal, and currently 20% off. A true statement piece.",
        var m when m.Contains("gift") =>
            "Great choice! Our most-gifted items are the Lumina Perfume No.7, Obsidian Watch, and Cascade Skincare Set. Need gift wrapping?",
        var m when m.Contains("sale") || m.Contains("discount") =>
            "We have some fantastic deals right now — Apex Runner X9 (21% off), Velvet Cap Toe Oxfords (17% off), and more in our Sale section.",
        _ => "Welcome to NexShop! I'm here to help you find your perfect item. Try asking about a specific category, occasion, or budget — I'll curate the best options for you."
    };
}
