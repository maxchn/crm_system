using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class ChatInputType : InputObjectGraphType
    {
        public ChatInputType()
        {
            Name = "ChatInput";

            Field<NonNullGraphType<StringGraphType>>("Name");
        }
    }
}