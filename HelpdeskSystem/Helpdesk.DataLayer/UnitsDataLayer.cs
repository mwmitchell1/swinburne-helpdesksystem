using System;
using System.Collections.Generic;
using Helpdesk.Data.Models;
using NLog;
using Helpdesk.Common.DTOs;
using System.Linq;
using Helpdesk.Common.Extensions;

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
        /// Retrieves all units under a specific helpdesk id
        /// </summary>
        /// <param name="id">ID of the helpdesk to retrieve from</param>
        /// <returns>A list of unit DTOs</returns>
        public List<UnitDTO> GetUnitsByHelpdeskID(int id)
        {
            List<UnitDTO> unitDTOs = new List<UnitDTO>();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdeskUnits = context.Helpdeskunit.ToList();

                foreach (Helpdeskunit helpdeskUnit in helpdeskUnits)
                {
                    if (helpdeskUnit != null && helpdeskUnit.HelpdeskId == id)
                    {
                        unitDTOs.Add(DAO2DTO(context.Unit.FirstOrDefault(u => u.UnitId == helpdeskUnit.UnitId)));
                    }
                }
            }

            return unitDTOs;
        }

        /// <summary>
        /// Deletes a specific unit
        /// </summary>
        /// <param name="id">ID of the unit to be deleted</param>
        /// <returns>Indication of the result of the operation</returns>
        public bool DeleteUnit(int id)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.FirstOrDefault(u => u.UnitId == id);

                if (unit == null)
                    throw new NotFoundException("Unable to find unit!");

                context.Unit.Remove(unit);
                context.SaveChanges();
            }
            return true;
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
