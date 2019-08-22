using System;
namespace Helpdesk.Common.DTOs
{
    public class UnitDTO
    {
        public int UnitId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
