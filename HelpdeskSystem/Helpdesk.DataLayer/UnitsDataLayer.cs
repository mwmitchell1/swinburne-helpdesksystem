using System;
using System.Collections.Generic;
using Helpdesk.Data.Models;
using NLog;
using Helpdesk.Common.DTOs;
using System.Linq;
using Helpdesk.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Common.Requests.Units;

namespace Helpdesk.DataLayer
{
    public class UnitsDataLayer
    {
        private static Logger s_Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Adds a unit to the database using provided unit request.
        /// </summary>
        /// <param name="request">The request containing the information to add a unit.</param>
        /// <returns>The id of the unit.</returns>
        public int? AddUnit(AddUpdateUnitRequest request)
        {
            int? unitId = null;
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        Unit newUnit = new Unit()
                        {
                            Code = request.Code,
                            IsDeleted = false,
                            Name = request.Name
                        };

                        context.Add(newUnit);

                        context.SaveChanges();

                        unitId = newUnit.UnitId;

                        if (!(unitId.HasValue || unitId.Value == 0))
                        {
                            trans.Rollback();
                            throw new Exception("Unable to add unit");
                        }

                        foreach (string topic in request.Topics)
                        {
                            context.Topic.Add(new Topic()
                            {
                                Name = topic,
                                UnitId = unitId.Value,
                                IsDeleted = false
                            });
                        }

                        context.Helpdeskunit.Add(new Helpdeskunit()
                        {
                            HelpdeskId = request.HelpdeskID,
                            UnitId = unitId.Value
                        });

                        context.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }

            return unitId;
        }

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
        /// Retrieves a unit from the database using provided unit name.
        /// </summary>
        /// <param name="name">The name of the unit to retrieve.</param>
        /// <returns>The unit DTO</returns>
        public UnitDTO GetUnitByNameAndHelpdeskId(string name, int helpdeskId)
        {
            UnitDTO dto = null;
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unitIds = context.Helpdeskunit.Where(hu => hu.HelpdeskId == helpdeskId).Select(u => u.UnitId).ToList();

                var unit = context.Unit.Include("Helpdeskunit").FirstOrDefault(u => u.Name.Equals(name) && unitIds.Contains(u.UnitId));
                if (unit != null)
                {
                    dto = DAO2DTO(unit);
                }
            }

            return dto;
        }

        /// <summary>
        /// Retrieves a unit from the database using provided unit code.
        /// </summary>
        /// <param name="code">The name of the unit to retrieve.</param>
        /// <returns>The unit DTO</returns>
        public UnitDTO GetUnitByCodeAndHelpdeskId(string code, int helpdeskId)
        {
            UnitDTO dto = null;
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unitIds = context.Helpdeskunit.Where(hu => hu.HelpdeskId == helpdeskId).Select(u => u.UnitId).ToList();

                var unit = context.Unit.Include("Helpdeskunit").FirstOrDefault(u => u.Code.Equals(code) && unitIds.Contains(u.UnitId));
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
                var helpdeskUnits = context.Helpdeskunit.Where(hu => hu.HelpdeskId == id).ToList();

                foreach (Helpdeskunit helpdeskUnit in helpdeskUnits)
                {
                    var unit = context.Unit.FirstOrDefault(u => u.UnitId == helpdeskUnit.UnitId);

                    if (!unit.IsDeleted)
                    {
                        unitDTOs.Add(DAO2DTO(unit));
                    }
                }
            }

            return unitDTOs;
        }

        /// <summary>
        /// Updates a specific unit
        /// </summary>
        /// <param name="id">ID of the unit to be deleted</param>
        /// <returns>Indication of the result of the operation</returns>
        public bool UpdateUnit(int id, AddUpdateUnitRequest request)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == id);

                        if (unit == null)
                            throw new NotFoundException("Unable to find unit!");

                        unit.Name = request.Name;
                        unit.IsDeleted = request.IsDeleted;
                        unit.Code = request.Code;

                        foreach (Topic topic in unit.Topic)
                        {
                            if (!request.Topics.Contains(topic.Name))
                            {
                                topic.IsDeleted = true;
                            }
                        }

                        foreach (string topic in request.Topics)
                        {
                            var existingTopic = unit.Topic.FirstOrDefault(t => t.Name == topic);

                            if (existingTopic != null)
                            {
                                if (existingTopic.IsDeleted)
                                {
                                    existingTopic.IsDeleted = false;
                                }
                            }
                            else
                            {
                                unit.Topic.Add(new Topic()
                                {
                                    IsDeleted = false,
                                    Name = topic,
                                    UnitId = unit.UnitId
                                });
                            }
                        }

                        context.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            return true;
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
            unitDTO.IsDeleted = unit.IsDeleted;

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
            unit.IsDeleted = unitDTO.IsDeleted;

            return unit;
        }
    }
}
