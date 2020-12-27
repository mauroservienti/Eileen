using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Eileen.Security
{
    internal class GoogleAuthEvents : OAuthEvents
    {
        const string NAME_IDENTIFIER = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private readonly string[] _allowedIdentifiers;

        public GoogleAuthEvents(string[] allowedIdentifiers)
        {
            _allowedIdentifiers = allowedIdentifiers;
        }

        public override Task TicketReceived(TicketReceivedContext context)
        {
            var nameIdentifierClaim = context?.Principal?.Claims.FirstOrDefault(c => c.Type == NAME_IDENTIFIER);
            var nameIdentifier = nameIdentifierClaim?.Value.ToLowerInvariant();
            
            if(!_allowedIdentifiers.Contains(nameIdentifier))
            {
                context.Response.StatusCode = 403; // or redirect somewhere
                context.HandleResponse();
            }

            return base.TicketReceived(context);
        }
    }
}