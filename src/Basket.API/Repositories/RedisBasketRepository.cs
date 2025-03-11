using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;
using eShop.Basket.API.Model;

namespace eShop.Basket.API.Repositories;

public class RedisBasketRepository(ILogger<RedisBasketRepository> logger, 
    IConnectionMultiplexer redis,
    Counter<int> itemsAddedCounter) : IBasketRepository
{
    private readonly IDatabase _database = redis.GetDatabase();
    private static readonly ActivitySource ActivitySource = new("Basket.API.Redis");
    private static readonly Meter BasketMeter = new("Basket.API.Count", "1.0.0");
    
    // implementation:

    // - /basket/{id} "string" per unique basket
    private static RedisKey BasketKeyPrefix = "/basket/"u8.ToArray();
    // note on UTF8 here: library limitation (to be fixed) - prefixes are more efficient as blobs

    private static RedisKey GetBasketKey(string userId) => BasketKeyPrefix.Append(userId);

    public async Task<bool> DeleteBasketAsync(string id)
    {
        using var activity = ActivitySource.StartActivity("DeleteBasket");
        activity?.SetTag("basket.id", id);
        var result = await _database.KeyDeleteAsync(GetBasketKey(id));
        activity?.SetTag("delete.result", result);

        return result;
    }

    public async Task<CustomerBasket> GetBasketAsync(string customerId)
    {
        using var activity = ActivitySource.StartActivity("GetBasket");
        activity?.SetTag("customer.id", customerId);

        using var data = await _database.StringGetLeaseAsync(GetBasketKey(customerId));

        if (data is null || data.Length == 0)
        {
            activity?.SetTag("basket.found", false);
            return null;
        }

        activity?.SetTag("basket.found", true);
        return JsonSerializer.Deserialize(data.Span, BasketSerializationContext.Default.CustomerBasket);
    }
    
    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        using var activity = ActivitySource.StartActivity("UpdateBasket");
        activity?.SetTag("basket.buyerId", basket.BuyerId);
        activity?.SetTag("basket.items", string.Join(", ", basket.Items.Select(i => $"{i.ProductId}:{i.Quantity}")));
        
        CustomerBasket current = await GetBasketAsync(basket.BuyerId);
        
        var json = JsonSerializer.SerializeToUtf8Bytes(basket, BasketSerializationContext.Default.CustomerBasket);
        var created = await _database.StringSetAsync(GetBasketKey(basket.BuyerId), json);
        
        int? diff = basket.Items.Sum(i => i.Quantity) - current?.Items.Sum(i => i.Quantity);
        if (diff is > 0)
        {
            itemsAddedCounter.Add(diff.Value, new KeyValuePair<string, object>("buyerId", basket.BuyerId));
        }
        
        if (!created)
        {
            logger.LogInformation("Problem occurred persisting the item.");
            activity?.SetStatus(ActivityStatusCode.Error, "Failed to persist basket");
            return null;
        }

        logger.LogInformation("Basket item persisted successfully.");
        activity?.SetStatus(ActivityStatusCode.Ok);
        return await GetBasketAsync(basket.BuyerId);
    }
}

[JsonSerializable(typeof(CustomerBasket))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class BasketSerializationContext : JsonSerializerContext
{

}
