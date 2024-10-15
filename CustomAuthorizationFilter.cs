using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Identity_final_attempt
{

    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _role;
        private readonly string _policy;

        public CustomAuthorizationFilter(string role = null, string policy = null)
        {
            _role = role;
            _policy = policy;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult(); // This should redirect properly
            }
            else if (!context.HttpContext.User.IsInRole("Admin"))
            {
                context.Result = new ForbidResult(); // This should show the AccessDenied page without looping
            }


            // If a policy is specified, apply policy-based authorization (optional)
            if (!string.IsNullOrEmpty(_policy))
            {
                var policyService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
                var policyResult = policyService.AuthorizeAsync(context.HttpContext.User, _policy).Result;

                if (!policyResult.Succeeded)
                {
                    context.Result = new ForbidResult(); // Forbid if policy authorization fails
                    return;
                }
            }
        }
    }


}
