using Newtonsoft.Json;

namespace CrmSystem.DAL.Entities
{
    public class ChatParticipant
    {
        public int ChatId { get; set; }
        [JsonIgnore]
        public Chat Chat { get; set; }

        public string Id { get; set; }
        public ApplicationUser User { get; set; }
    }
}