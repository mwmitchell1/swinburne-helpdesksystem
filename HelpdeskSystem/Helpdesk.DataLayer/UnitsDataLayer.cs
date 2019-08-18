using System;
using Helpdesk.Data.Models;
using NLog;
using Helpdesk.Common.DTOs;
using System.Linq;

namespace Helpdesk.DataLayer
{
    public class UnitsDataLayer
    {
        private static Logger s_Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Retrieves a unit from the database using provided unit id.
        /// </summary>
        /// <param name="id">The id of the unit to retrieve.</param>
        /// <returns></returns>
        public UnitDTO GetUnit(int id)
        {
            UnitDTO dto = null;
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.FirstOrDefault(u => u.UnitId == id);
                if (unit != null)
                {
                    dto = DAO2DTO(unit);
                }
            }

            return dto;
        }

        /// <summary>
        /// Converts the unit DAO to a DTO to send to the front end
        /// </summary>
        /// <param name="unit">Unit DAO object to be converted.</param>
        /// <returns></returns>
        private UnitDTO DAO2DTO(Unit unit)
        {
            UnitDTO unitDTO = new UnitDTO();
            unitDTO.UnitId = unit.UnitId;
            unitDTO.Code = unit.Code;
            unitDTO.Name = unit.Name;

            return unitDTO;
        }

        /// <summary>
        /// Converts the unit DTO to a DAO to interact with the database
        /// </summary>
        /// <param name="unitDTO">Unit DTO object to be converted.</param>
        /// <returns></returns>
        private Unit DTO2DAO(UnitDTO unitDTO)
        {
            Unit unit = new Unit();
            unit.UnitId = unitDTO.UnitId;
            unit.Code = unitDTO.Code;
            unit.Name = unitDTO.Name;

            return unit;
        }
    }
}
