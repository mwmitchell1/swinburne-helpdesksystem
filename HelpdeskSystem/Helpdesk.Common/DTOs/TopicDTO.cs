using System;
namespace Helpdesk.Common.DTOs
{
    /// <summary>
    /// Data transfer object used to represent a topic
    /// </summary>
    public class TopicDTO
    {
        public int TopicId { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
