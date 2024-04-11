using Microsoft.AspNetCore.Authorization;

namespace RentalProperties.Controllers
{
    public static class RentalWebsite
    {
        public static async Task<bool> UserHasPolicy(HttpContext _httpContext, string policyName)
        {
            var currentUser = _httpContext.User;
            var authorizationService = _httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser, null, policyName);
            return authorizationResult.Succeeded;
        }
    }
}
