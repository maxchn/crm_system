using GraphQL.Authorization;
using System.Security.Claims;

namespace CrmSystem.Server.GraphQL
{
    public class GraphQLUserContext : IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}