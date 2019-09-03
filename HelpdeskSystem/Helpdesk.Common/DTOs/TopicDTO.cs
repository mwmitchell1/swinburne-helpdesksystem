using System;
namespace Helpdesk.Common.DTOs
{
    public class TopicDTO
    {
        public int TopicId { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
