// =====================================================================
// HonourBandConstraint — CARRIED FORWARD FROM SESSION 16 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
//
// Finished, unchanged since Session 16. Still guards students/honours/{band}.
// =====================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Globalization;
using System.Linq;

namespace StudentPortalWeb.Constraints
{
    public class HonourBandConstraint : IRouteConstraint
    {
        private static readonly string[] AllowedBands = { "first", "second", "pass" };

        public bool Match(
            HttpContext? httpContext,
            IRouter? route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey , out var value) || value == null)
            {
                return false;
            }

            var band = Convert.ToString(value , CultureInfo.InvariantCulture);

            return AllowedBands.Contains(band, StringComparer.OrdinalIgnoreCase);
        }
    }
}
