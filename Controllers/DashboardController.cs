using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using FuelTrack_Malaysia.Models; 
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;

public class DashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DashboardController> _logger;
    private readonly IDistributedCache _cache;

    public DashboardController(IHttpClientFactory httpClientFactory, ILogger<DashboardController> logger, IDistributedCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _cache = cache;
    }

    // This action method serves the main dashboard view.
    public IActionResult Index()
    {
        return View();
    }

    // This HTTP GET action fetches fuel data, either from cache or a public API.
    // It's decorated with [HttpGet] so it responds to GET requests.
    [HttpGet]
    public async Task<IActionResult> GetFuelData()
    {
        // Define a key for caching the fuel price data.
        var cacheKey = "FuelPriceData";
        
        // Try to retrieve data from the distributed cache.
        string? cachedData = await _cache.GetStringAsync(cacheKey);

        // A list to hold the processed fuel data, flattened for easier use in the frontend.
        List<object> flattenedFuelData = new List<object>();

        // Check if data was found in the cache.
        if (!string.IsNullOrEmpty(cachedData))
        {
            // If yes, deserialize the cached string back into a list of objects.
            flattenedFuelData = JsonSerializer.Deserialize<List<object>>(cachedData)!;
        }
        else
        {
            // If no, create an HttpClient instance using the factory.
            var httpClient = _httpClientFactory.CreateClient();
            // Define the API URL for the fuel price data from data.gov.my.
            var apiUrl = "https://api.data.gov.my/data-catalogue?id=fuelprice";

            try
            {
                // Send an asynchronous GET request to the API.
                var response = await httpClient.GetAsync(apiUrl);
                
                // Throw an exception if the HTTP response status is not successful.
                response.EnsureSuccessStatusCode();

                // Read the response stream asynchronously.
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                // Deserialize the JSON data from the stream into a list of FuelData objects.
                var apiData = await JsonSerializer.DeserializeAsync<List<FuelData>>(responseStream);

                // Process the fetched data if it's not null.
                if (apiData != null)
                {
                    // Use LINQ's SelectMany to flatten the data.
                    // The API returns a list of daily entries, each containing prices for different fuel types.
                    // This code transforms it into a flat list of individual fuel price records.
                    flattenedFuelData = apiData.SelectMany(item =>
                    {
                        var prices = new List<object>();
                        if (item.Ron95.HasValue && item.Ron95.Value > 0)
                        {
                            prices.Add(new { date = item.Date, fuel_type = "ron95", price = item.Ron95.Value });
                        }
                        if (item.Ron97.HasValue && item.Ron97.Value > 0)
                        {
                            prices.Add(new { date = item.Date, fuel_type = "ron97", price = item.Ron97.Value });
                        }
                        if (item.Diesel.HasValue && item.Diesel.Value > 0)
                        {
                            prices.Add(new { date = item.Date, fuel_type = "diesel", price = item.Diesel.Value });
                        }
                        return prices;
                    }).ToList();
                }

                // Define caching options, setting an absolute expiration of 6 hours.
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(6));
                
                // Serialize the flattened data and save it to the cache.
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(flattenedFuelData), options);
            }
            catch (HttpRequestException ex)
            {
                // Log an error if the API request fails (e.g., network issues, 404, 500).
                _logger.LogError(ex, "Failed to fetch fuel data from the API.");
            }
            catch (JsonException ex)
            {
                // Log an error if the JSON deserialization fails (e.g., invalid data format).
                _logger.LogError(ex, "Failed to deserialize JSON response from the API.");
            }
        }

        // Generate dummy data for charts. This is separate from the real fuel price data.
        var dummyData = new
        {
            comboChart = GenerateDummyComboChartData(),
            verticalBarChart = GenerateDummyVerticalBarChartData(),
            lineChart = GenerateDummyLineChartData()
        };

        // Return a JSON response containing both the real fuel price data and the dummy chart data.
        return Json(new
        {
            fuelPrices = flattenedFuelData,
            dummyData = dummyData
        });
    }

    // Static helper methods to generate dummy data for chart examples.
    private static List<object> GenerateDummyComboChartData()
    {
        var data = new List<object>();
        var baseDate = new DateTime(2025, 9, 8);
        var comboValues1 = new int[] { 48, 86, 19, 23, 57, 79, 19 };
        var comboValues2 = new int[] { 27, 82, 69, 30, 62, 10, 97 };
        var comboValues3 = new int[] { 53, 70, 67, 87, 85, 66, 96 };
        for (int i = 0; i < 7; i++)
        {
            data.Add(new
            {
                date = baseDate.AddDays(i).ToString("yyyy-MM-dd"),
                dataset1_value = comboValues1[i],
                dataset2_value = comboValues2[i],
                dataset3_value = comboValues3[i]
            });
        }
        return data;
    }

    private static List<object> GenerateDummyVerticalBarChartData()
    {
        return new List<object>
        {
            new { month = "January", dataset1_value = 58, dataset2_value = 24 },
            new { month = "February", dataset1_value = -32, dataset2_value = 42 },
            new { month = "March", dataset1_value = 95, dataset2_value = -2 },
            new { month = "April", dataset1_value = -28, dataset2_value = 18 },
            new { month = "May", dataset1_value = -35, dataset2_value = 10 },
            new { month = "June", dataset1_value = 90, dataset2_value = 42 },
            new { month = "July", dataset1_value = -3, dataset2_value = 6 }
        };
    }

    private static List<object> GenerateDummyLineChartData()
    {
        return new List<object>
        {
            new { month = "January", dataset1_value = -82, dataset2_value = 95 },
            new { month = "February", dataset1_value = -70, dataset2_value = -65 },
            new { month = "March", dataset1_value = 8, dataset2_value = 40 },
            new { month = "April", dataset1_value = -84, dataset2_value = 5 },
            new { month = "May", dataset1_value = 22, dataset2_value = 62 },
            new { month = "June", dataset1_value = 6, dataset2_value = 100 },
            new { month = "July", dataset1_value = 68, dataset2_value = 28 }
        };
    }
}