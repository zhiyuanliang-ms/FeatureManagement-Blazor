namespace BlazorServerApp.Services
{
    public class ContextProvider
    {
        public bool IsAuthenticated { get; set; }

        public string UserName { get; set; }

        public string SessionCount { get; set; }
    }
}
