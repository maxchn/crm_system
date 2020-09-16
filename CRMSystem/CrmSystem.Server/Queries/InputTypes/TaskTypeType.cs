using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.InputTypes
{
    public class TaskTypeType : EnumerationGraphType<TaskType>
    {
        public TaskTypeType()
        {
            Name = "TaskType";
        }
    }
}