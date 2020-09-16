using CrmSystem.Server.GraphQL;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class ResultType : ObjectGraphType<Result>
    {
        public ResultType()
        {
            Name = "Result";

            Field(x => x.Status);
            Field(x => x.Message);
            Field(x => x.Value);
        }
    }
}