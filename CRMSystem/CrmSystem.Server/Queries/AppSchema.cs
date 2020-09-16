using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using CrmSystem.Server.Services;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CrmSystem.Server.Queries
{
    public class AppSchema : Schema
    {
        public AppSchema(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender) : base()
        {
            Query = new AppQuery(unitOfWork, userManager);
            Mutation = new AppMutation(userManager, unitOfWork, configuration, emailSender);
        }
    }
}