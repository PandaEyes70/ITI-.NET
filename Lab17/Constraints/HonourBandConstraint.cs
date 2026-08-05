// =====================================================================
// HonourBandConstraint — SESSION PROJECT (Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 16 — Block 4
//
// This is the file where the framework calls YOUR code to make its own
// routing decision. Everything in the route table until now has been you
// asking the framework a question. This one line of the pattern makes
// the framework ask you.
//
// ⚠️ AS SHIPPED, THIS CONSTRAINT REFUSES EVERY URL. That is deliberate,
//    not a bug: the project has to build and run before any TODO is done
//    (Rule 39), and a method that returns a bool has to return
//    something. TODO 5 replaces the refusal with a real decision. Until
//    it does, any route using this constraint will 404 — which is worth
//    seeing once, on purpose, before you fix it.
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
        // The three band names the university actually uses. Kept in one
        // place, in one casing, so the constraint and the controller can
        // never drift apart.
        private static readonly string[] AllowedBands = { "first", "second", "pass" };

        // The framework calls this once per candidate route, while it is
        // still deciding. Return true and this route is allowed to match;
        // return false and routing moves on to the next route in the
        // table as though this one did not exist.
        public bool Match(
            HttpContext? httpContext,
            IRouter? route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            // TODO 5: Replace the single `return false;` line below with
            //         the real decision, in three steps.
            //         Step one — a guard clause. Try to pull this
            //         parameter's value out of the values dictionary
            //         using the routeKey you were handed, and if it is
            //         not there, or it is null, refuse immediately by
            //         returning false. Never index the dictionary
            //         directly here: routing calls this method
            //         speculatively, and a missing key is a normal answer,
            //         not an exceptional one.
            //         Step two — convert what you got to text. It arrives
            //         as the most general object type there is, because
            //         routing does not know or care what shape you wanted.
            //         Use the framework conversion helper that takes an
            //         object and a culture, and pass the culture that
            //         means "machine rules, not the user's local rules" —
            //         URLs are not written in anyone's local language,
            //         and using the local culture here is a bug that only
            //         appears on someone else's laptop.
            //         Step three — return whether the allowed-bands array
            //         above contains that text, compared WITHOUT case
            //         sensitivity, by passing the ordinal-ignore-case
            //         string comparer as the second argument. A user who
            //         types First with a capital F is not making a
            //         mistake worth a 404.
            //         ⚠️ Notice what you are NOT doing: touching the
            //         database. This method runs on every candidate route
            //         of every request. A query in here would be a
            //         database round-trip on requests that never even
            //         reach a controller.

            if (!values.TryGetValue(routeKey , out var value) || value == null)
            {
                return false;
            }

            var band = Convert.ToString(value , CultureInfo.InvariantCulture);

            return AllowedBands.Contains(band, StringComparer.OrdinalIgnoreCase);


        }
    }
}
