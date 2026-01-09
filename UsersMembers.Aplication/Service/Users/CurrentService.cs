using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UsersMembers.Application.Interface;

namespace UsersMembers.Application.Service.Users
{
    public class CurrentService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return userId ?? string.Empty;
            }
        }

        public string CorrelationId
        {
            get
            {
                // Getting correlation ID from header or generating a new one if not present
                // This is useful for tracing requests across microservices
                var context = _httpContextAccessor.HttpContext;
                if (context != null && context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
                {
                    return correlationId.ToString();
                }
                
                return Guid.NewGuid().ToString();
            }
        }
    }
}
