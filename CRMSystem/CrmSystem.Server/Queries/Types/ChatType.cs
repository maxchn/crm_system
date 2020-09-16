using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class ChatType : ObjectGraphType<Chat>
    {
        public ChatType()
        {
            Name = "Chat";

            Field(x => x.ChatId);
            Field(x => x.Name);
            Field<ApplicationUserType>("Owner");
        }
    }
}