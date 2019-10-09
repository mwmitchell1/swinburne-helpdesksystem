using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Students;
using Helpdesk.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// Used to handle CRUD for student records in the database
    /// </summary>
    public class StudentDatalayer
    {
        /// <summary>
        /// used to retreive all nicknames in the database
        /// </summary>
        /// <returns>The list of nicknames as DTOs</returns>
        public List<NicknameDTO> GetAllNicknames()
        {
            List<NicknameDTO> nicknameDTOs = new List<NicknameDTO>();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var nicknames = context.Nicknames.ToList();

                foreach (var nickname in nicknames)
                {
                    nicknameDTOs.Add(DAO2DTO(nickname));
                }
            }

            return nicknameDTOs;
        }

        /// <summary>
        /// Used to get a student nickname by the nickname
        /// </summary>
        /// <param name="nickname">The nickname to look up</param>
        /// <returns>The nickname</returns>
        public NicknameDTO GetStudentNicknameByNickname(string nickname)
        {
            NicknameDTO nicknameDTO = null;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var nicknameDAO = context.Nicknames.FirstOrDefault(p => p.NickName == nickname);

                if (nicknameDAO == null)
                    return null;

                nicknameDTO = DAO2DTO(nicknameDAO);
            }

            return nicknameDTO;
        }

        /// <summary>
        /// Used to get a student nickname by their studentId
        /// </summary>
        /// <param name="studentId">The studentId to look up</param>
        /// <returns>The nickname</returns>
        public NicknameDTO GetStudentNicknameByStudentID(string studentId)
        {
            NicknameDTO nicknameDTO = null;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var nicknameDAO = context.Nicknames.FirstOrDefault(p => p.Sid == studentId);

                if (nicknameDAO == null)
                    return null;

                nicknameDTO = DAO2DTO(nicknameDAO);
            }

            return nicknameDTO;
        }

        /// <summary>
        /// Used to get a datatable with all of the helpdesk records
        /// </summary>
        /// <returns>Datatable with the helpdesk records</returns>
        public DataTable GetStudentsAsDataTable()
        {
            DataTable nicknames = new DataTable();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                DbConnection conn = context.Database.GetDbConnection();
                ConnectionState state = conn.State;

                try
                {
                    if (state != ConnectionState.Open)
                        conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "GetAllNicknames";
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            nicknames.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (state != ConnectionState.Closed)
                        conn.Close();
                }
            }

            return nicknames;
        }

        /// <summary>
        /// Used to add a nickname to the database
        /// </summary>
        /// <param name="request">The nickname information</param>
        /// <returns>The id of the nickname added</returns>
        public int AddStudentNickname(AddStudentRequest request)
        {

            Nicknames nickname = new Nicknames()
            {
                NickName = request.Nickname,
                Sid = request.SID
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Nicknames.Add(nickname);

                context.SaveChanges();
            }

            return nickname.StudentId;
        }

        /// <summary>
        /// Used to edit the specified student's nickname in the databse with the request's information
        /// </summary>
        /// <param name="id">The StudentID of the student to be updated</param>
        /// <param name="request">The request that contains the student's new nickname</param>
        /// <returns>A boolean that indicates whether the operation was a success</returns>
        public bool EditStudentNickname(int id, EditStudentNicknameRequest request)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Nicknames nickname = context.Nicknames.FirstOrDefault(n => n.StudentId == id);

                if (nickname == null)
                    return false;
    
                nickname.NickName = request.Nickname;

                context.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Converts the nickname DAO to a DTO to send to the front end
        /// </summary>
        /// <param name="nickname">The DAO for the nickname</param>
        /// <returns>The DTO for the nickname</returns>
        private NicknameDTO DAO2DTO(Nicknames nickname)
        {
            NicknameDTO nicknameDTO = null;

            nicknameDTO = new NicknameDTO();
            nicknameDTO.ID = nickname.StudentId;
            nicknameDTO.Nickname = nickname.NickName;
            nicknameDTO.StudentID = nickname.Sid;

            return nicknameDTO;
        }

        /// <summary>
        /// Converts the nickname DTO to a DAO to interact with the database
        /// </summary>
        /// <param name="nickname">The DTO for the nickname</param>
        /// <returns>The DAO for the nickname</returns>
        private Nicknames DTO2DAO(Nicknames nicknameDTO)
        {
            Nicknames nickname = null;
            nickname = new Nicknames()
            {
                NickName = nicknameDTO.NickName,
                Sid = nicknameDTO.Sid,
                StudentId = nickname.StudentId
            };

            return nickname;
        }
    }
}
