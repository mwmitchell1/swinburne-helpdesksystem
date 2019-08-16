using System;

namespace Helpdesk.Common.DTOs
{
    public class HelpdeskSettingsDTO
    {
        public int HelpdeskId { get; set; }
        public string Name { get; set; }
        public bool HasCheckIn { get; set; }
        public bool HasQueue { get; set; }
    }
}
