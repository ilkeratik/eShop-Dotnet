using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace eShop.ServiceDefaults.Processor;

public class EnrichLogsWithUsernameProcessor : BaseProcessor<LogRecord>
{
    private readonly string name;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EnrichLogsWithUsernameProcessor(IHttpContextAccessor httpContextAccessor, string name = "EnrichLogsWithUsernameProcessor")
    {
        this.name = name;
        _httpContextAccessor = httpContextAccessor;
    }

    public override void OnEnd(LogRecord record)
    {
        var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
        record.Attributes = record!.Attributes!.Append(new KeyValuePair<string, object>("Username", string.IsNullOrEmpty(username) ? "AnonymousZirrrrrttt" : username)).ToList()!;
    }
}

public class MyFilteringProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity activity)
    {
        var tagsToMask = new List<string>();

        // Iterate through activity tags
        foreach (var keyValuePair in activity.Tags)
        {
            var key = keyValuePair.Key;

            if (key.Contains("id", StringComparison.OrdinalIgnoreCase))
            {
                tagsToMask.Add(key);
            }
        }
        
        foreach (var tag in tagsToMask)
        {
            activity.SetTag(tag, "******masked_value******");
        }

    }
}

class MyEnrichingProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity activity)
    {
        // Enrich activity with additional tags.
        activity.SetTag("AGlobalTag", "justAddingToAllEvents");

        // Enriching from Baggage.
        // The below snippet adds every Baggage item.
        foreach (var baggage in Baggage.GetBaggage())
        {
            activity.SetTag(baggage.Key, baggage.Value);
        }

        // The below snippet adds specific Baggage item.
        var deviceTypeFromBaggage = Baggage.GetBaggage("device.type");
        if (deviceTypeFromBaggage != null)
        {
            activity.SetTag("device.type", deviceTypeFromBaggage);
        }
    }
}
