using System;
using System.Collections.Generic;


namespace BusinessLogicLayer
{
    public class UserDTO
    {
       
        public int ID { get; set; }
        public string EmailID { get; set; }
        public string Password { get; set; }
        public bool Is_Admin { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<decimal> ContactNo { get; set; }
        public string Gender { get; set; }

        public List<UserSkillDTO> UserSkills { get; internal set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}
