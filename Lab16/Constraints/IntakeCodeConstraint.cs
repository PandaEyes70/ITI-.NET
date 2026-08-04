using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Globalization;

namespace StudentPortalWeb.Constraints
{
    public class IntakeCodeConstraint : IRouteConstraint
    {
        private static readonly string AllowedIntake = "itiB"; // Lab ID 31 -> 31 mod 3 = 1 -> itiB

        public bool Match(
            HttpContext? httpContext,
            IRouter? route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            // Guard clause for missing or null value (no indexer to avoid exceptions)
            if (!values.TryGetValue(routeKey, out var value) || value == null)
            {
                return false;
            }

            // Convert value to string using invariant culture (machine rules)
            var code = Convert.ToString(value, CultureInfo.InvariantCulture);

            // Return true if the value matches the allowed intake code case-insensitively
            return string.Equals(code, AllowedIntake, StringComparison.OrdinalIgnoreCase);
        }
    }
}
