namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class provides support to all API clients.
    /// </summary>
    public static class ApiClient
    {
        public static string BaseUrl
        {
            get
            {
#if PRODUCTION
                return "https://lts-api.com/";
#else
                return "https://lts-api.com/dev/";
#endif
            }
        }
    }
}