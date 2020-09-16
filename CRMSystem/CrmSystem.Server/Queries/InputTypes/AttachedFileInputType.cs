using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class AttachedFileInputType : InputObjectGraphType
    {
        public AttachedFileInputType()
        {
            Name = "AttachedFileInput";

            Field<IntGraphType>("AttachedFileId");
            Field<StringGraphType>("Name");
            Field<StringGraphType>("Path");
        }
    }
}