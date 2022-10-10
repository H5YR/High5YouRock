namespace h5yr.ViewComponents
{
    public class TweetModel
    {
        public string? Content { get; set; }
        public string? Username { get; set; }
        public string? ScreenName { get; set; }
        public string? Avatar { get; set; }
        public DateTimeOffset TweetedOn { get; set; }
        public int NumberOfTweets { get; set; }
        public string? ReplyToTweet { get; set; }
        public string? Url { get; set; }

    }

    public class TweetsModel
    {
        public int NumberOfTweets { get; set; } 
        public IEnumerable<TweetModel> Tweets { get; set; }
    }
}