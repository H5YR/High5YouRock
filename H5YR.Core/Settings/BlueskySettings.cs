namespace H5YR.Core.Settings;

public class BlueskySettings
{
    public string ServiceUrl { get; set; } = "https://bsky.social";
    public string Hashtag { get; set; } = "h5yr";

    /// <summary>Handle or DID of the account used to authenticate (e.g. h5yr.bsky.social).</summary>
    public string? Identifier { get; set; }

    /// <summary>Bluesky app password. Store as a secret, never in source control.</summary>
    public string? AppPassword { get; set; }
}
