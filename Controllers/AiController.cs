using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NexShop.Data;
using NexShop.Models;

namespace NexShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private static readonly List<Product> Products = SeedData.GetProducts();
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AiController> _logger;

    public AiController(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<AiController> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] AiChatRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new ApiErrorResponse
            {
                Error = "Message is required"
            });
        }

        var message = request.Message.Trim();
        var relevantProducts = GetRelevantProducts(message);
        var fallbackReply = GetFallbackReply(message);

        var apiKey = _configuration["Anthropic:ApiKey"] ??
                     Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Ok(new ChatResponse
            {
                Reply = fallbackReply,
                Products = relevantProducts,
                UsedFallback = true
            });
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Anthropic");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var productContext = JsonSerializer.Serialize(
                Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Brand,
                    p.Category,
                    p.Price,
                    p.OriginalPrice,
                    p.Tags,
                    p.Rating,
                    p.Stock,
                    p.IsNew,
                    p.IsTrending,
                    p.IsFeatured
                }));

            var systemPrompt = $"""
                You are NexAI, the intelligent shopping assistant for NexShop.
                Be concise, helpful, stylish, and product-aware.
                Keep replies under 100 words.
                Prefer mentioning relevant products by name when helpful.
                Do not invent products outside the catalog.

                Available products:
                {productContext}
                """;

            var history = request.ConversationHistory?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .TakeLast(10)
                .ToArray() ?? Array.Empty<string>();

            var messages = new List<object>();

            for (var i = 0; i < history.Length - 1; i += 2)
            {
                messages.Add(new
                {
                    role = "user",
                    content = history[i]
                });

                messages.Add(new
                {
                    role = "assistant",
                    content = history[i + 1]
                });
            }

            messages.Add(new
            {
                role = "user",
                content = message
            });

            var payload = new
            {
                model = "claude-sonnet-4-20250514",
                max_tokens = 300,
                system = systemPrompt,
                messages
            };

            using var httpContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            using var response = await client.PostAsync(
                "https://api.anthropic.com/v1/messages",
                httpContent,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Anthropic request failed with status {StatusCode}. Body: {Body}", response.StatusCode, errorBody);

                return Ok(new ChatResponse
                {
                    Reply = fallbackReply,
                    Products = relevantProducts,
                    UsedFallback = true
                });
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var reply = ExtractAnthropicText(doc) ?? fallbackReply;

            return Ok(new ChatResponse
            {
                Reply = reply,
                Products = relevantProducts,
                UsedFallback = false
            });
        }
        catch (OperationCanceledException)
        {
            return Ok(new ChatResponse
            {
                Reply = fallbackReply,
                Products = relevantProducts,
                UsedFallback = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI chat request failed.");

            return Ok(new ChatResponse
            {
                Reply = fallbackReply,
                Products = relevantProducts,
                UsedFallback = true
            });
        }
    }

    [HttpGet("recommendations/{productId:int}")]
    public IActionResult GetRecommendations(int productId)
    {
        var product = Products.FirstOrDefault(p => p.Id == productId);
        if (product is null)
        {
            return NotFound(new ApiErrorResponse
            {
                Error = "Product not found"
            });
        }

        var similar = Products
            .Where(p => p.Id != productId)
            .Select(p => new
            {
                Product = p,
                Score =
                    (p.Category.Equals(product.Category, StringComparison.OrdinalIgnoreCase) ? 3 : 0) +
                    p.Tags.Intersect(product.Tags, StringComparer.OrdinalIgnoreCase).Count() +
                    (p.IsTrending ? 1 : 0)
            })
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Product.Rating)
            .ThenByDescending(x => x.Product.ReviewCount)
            .Take(4)
            .Select(x => x.Product)
            .ToList();

        return Ok(similar);
    }

    [HttpGet("loyalty")]
    public IActionResult GetLoyalty()
    {
        return Ok(new LoyaltyProfile());
    }

    private static string? ExtractAnthropicText(JsonDocument doc)
    {
        if (!doc.RootElement.TryGetProperty("content", out var contentElement) ||
            contentElement.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var item in contentElement.EnumerateArray())
        {
            if (item.TryGetProperty("text", out var textElement))
            {
                var text = textElement.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text.Trim();
                }
            }
        }

        return null;
    }

    private static List<Product> GetRelevantProducts(string message)
    {
        var terms = message
            .ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', ':', ';', '!', '?', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Distinct()
            .ToHashSet();

        return Products
            .Select(p => new
            {
                Product = p,
                Score =
                    ScoreTextMatch(p.Name, terms) * 4 +
                    ScoreTextMatch(p.Brand, terms) * 2 +
                    ScoreTextMatch(p.Category, terms) * 3 +
                    p.Tags.Sum(t => terms.Contains(t.ToLowerInvariant()) ? 3 : 0) +
                    ScoreTextMatch(p.Description, terms)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Product.IsTrending)
            .ThenByDescending(x => x.Product.Rating)
            .Take(3)
            .Select(x => x.Product)
            .ToList();
    }

    private static int ScoreTextMatch(string text, HashSet<string> terms)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        var lowered = text.ToLowerInvariant();
        return terms.Count(term => lowered.Contains(term));
    }

    private static string GetFallbackReply(string message)
    {
        var m = message.ToLowerInvariant();

        if (m.Contains("shoe") || m.Contains("sneaker") || m.Contains("runner"))
        {
            return "Our Apex Runner X9 is trending right now—elite cushioning, carbon plate, and premium comfort. Want more footwear picks?";
        }

        if (m.Contains("watch"))
        {
            return "The Obsidian Watch Mk II stands out with Swiss movement, sapphire crystal, and a refined luxury feel.";
        }

        if (m.Contains("gift"))
        {
            return "Top gift picks right now include Lumina Perfume No.7, the Obsidian Watch Mk II, and the Cascade Skincare Set.";
        }

        if (m.Contains("sale") || m.Contains("discount"))
        {
            return "Current value picks include Apex Runner X9, Aura Wireless Buds, and Velvet Cap Toe Oxfords.";
        }

        return "Welcome to NexShop. Tell me a category, style, budget, or occasion, and I’ll suggest the best matches.";
    }
}
