using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Students;
using Helpdesk.Data.Models;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// Used to handle CRUD for student records in the database
    /// </summary>
    public class StudentDatalayer
    {
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
            nicknameDTO.SID = nickname.Sid;

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
