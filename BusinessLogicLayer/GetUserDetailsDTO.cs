using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class GetUserDetailsDTO
    {
        [Key]
        public int ID { get; set; }
        public string EmailID { get; set; }
        public string Password { get; set; }
        public bool Is_Admin { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<Int64> ContactNo { get; set; }
        public string Gender { get; set; }
        public List<UpdateUserSkillsDTO> Skills { get; set; }
    }
}
