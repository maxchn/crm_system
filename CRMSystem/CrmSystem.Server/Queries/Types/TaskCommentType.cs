using CrmSystem.DAL.Entities;
using GraphQL.Types;

namespace CrmSystem.Server.Queries.Types
{
    public class TaskCommentType : ObjectGraphType<TaskComment>
    {
        public TaskCommentType()
        {
            Name = "TaskComment";

            Field(x => x.TaskCommentId);
            Field(x => x.Text);
            Field(x => x.Date);
            Field(x => x.AuthorId);           
            Field(x => x.IsAccessOnDeleting);
            Field<TaskType>("Task");
            Field<ApplicationUserType>("Author");
        }
    }
}