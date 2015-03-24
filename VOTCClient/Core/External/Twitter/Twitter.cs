using TweetSharp;

namespace VOTCClient.Core.External.Twitter
{
    public static class Twitter
    {
        public static void Tweet(string txt) => Kernel.TwitterClient.SendTweet(new SendTweetOptions { Status = txt });
    }
}
