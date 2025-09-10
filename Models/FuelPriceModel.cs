using System;
using System.Text.Json.Serialization;

namespace FuelTrack_Malaysia.Models
{
    /// <summary>
    // Represents the data structure for a single fuel price record received from the external API. This model is used to deserialize the JSON response.
    /// </summary>
    public class FuelData
    {
        /// <summary>
        // The raw date string from the API (e.g., "2025-09-01") & The [JsonPropertyName("date")] attribute maps the JSON key "date" to this property.
        /// </summary>
        [JsonPropertyName("date")]
        public string? DateString { get; set; }

        /// <summary>
        // A computed property that safely converts the DateString into a nullable DateTime object. The [JsonIgnore] attribute prevents this property from being deserialized or serialized,
        /// </summary>
        [JsonIgnore]
        public DateTime? Date => DateTime.TryParse(DateString, out var date) ? date : null;

        /// <summary>
        // The price of RON 95 fuel. The [JsonPropertyName] attribute maps the JSON key to this property. It's a nullable decimal to handle cases where the price might be missing.
        /// </summary>
        [JsonPropertyName("ron95")]
        public decimal? Ron95 { get; set; }

        /// <summary>
        // The price of RON 97 fuel.
        /// </summary>
        [JsonPropertyName("ron97")]
        public decimal? Ron97 { get; set; }

        /// <summary>
        // The price of Diesel fuel.
        /// </summary>
        [JsonPropertyName("diesel")]
        public decimal? Diesel { get; set; }
        
        /// <summary>
        // A property for the series type, which may be included in the API response.
        /// </summary>
        [JsonPropertyName("series_type")]
        public string? SeriesType { get; set; }

        /// <summary>
        // The price of Diesel in East Malaysia.
        /// </summary>
        [JsonPropertyName("diesel_eastmsia")]
        public decimal? DieselEastMsia { get; set; }
    }
}