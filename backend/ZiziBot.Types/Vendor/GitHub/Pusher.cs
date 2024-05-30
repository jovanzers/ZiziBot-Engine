﻿using System.Text.Json.Serialization;

namespace ZiziBot.Types.Vendor.GitHub;

public partial class Pusher
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}