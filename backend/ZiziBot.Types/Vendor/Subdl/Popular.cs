﻿using System.Text.Json.Serialization;

namespace ZiziBot.Types.Vendor.Subdl;

public partial class Popular
{
    [JsonPropertyName("results")]
    public List<Result>? Results { get; set; }
}

public partial class Result
{
    [JsonPropertyName("type")]
    [JsonConverter(converterType: typeof(JsonStringEnumConverter))]
    public MovieType MovieType { get; set; }

    [JsonPropertyName("sd_id")]
    public string SdId { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("poster_url")]
    public string PosterUrl { get; set; }

    public long Year { get; set; }

    [JsonPropertyName("total_season")]
    public long TotalSeasons { get; set; }

    [JsonPropertyName("subtitles_count")]
    public long SubtitlesCount { get; set; }
}