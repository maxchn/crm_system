using System;

namespace CrmSystem.Server.GraphQL
{
    public class Result
    {
        public bool Status { get; set; }

        public string Message { get; set; } = String.Empty;

        public string Value { get; set; } = String.Empty;
    }
}