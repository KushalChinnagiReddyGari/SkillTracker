using DataAccessLayer;
using System;

namespace BusinessLogicLayer
{
    public class UserSkillDTO
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> SkillID { get; set; }
        public string Proficiency { get; set; }

        public virtual SkillDTO Skill { get; set; }
        public virtual User User { get; set; }
    }
}