using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class ExecutionResultType : ObjectGraphType<ExecutionResult>
    {
        public ExecutionResultType()
        {
            Name = "ExecutionResult";

            Field(x => x.Status);
            Field(x => x.IsExecution);
            Field(x => x.TotalTime);
        }
    }
}