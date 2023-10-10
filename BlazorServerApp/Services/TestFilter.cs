using Microsoft.FeatureManagement;

namespace BlazorServerApp.Services
{
    public class TestingFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ContextProvider _contextProvider; // scoped service cannot be injected into a singleton service

        [FilterAlias("Testing")]
        public TestingFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureEvaluationContext)
        {
            var sessionCount = _httpContextAccessor.HttpContext.Session.GetString("Count");

            Console.WriteLine($"Filter gets Count from session in _httpContextAccessor.HttpContext: {sessionCount}");

            if (string.Equals(sessionCount, "0"))
            {
                Console.WriteLine("sessionCount equals to '0'. The feature is enabled.");

                return Task.FromResult(true);
            }
            else
            {
                Console.WriteLine("sessionCount does not equal to '0'. The feature is disabled.");

                return Task<bool>.FromResult(false);
            }
        }
    }
}
