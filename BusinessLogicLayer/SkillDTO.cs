using DataAccessLayer;
using System.Collections.Generic;

namespace BusinessLogicLayer
{
    public partial class SkillDTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SkillDTO()
        {
            this.UserSkills = new HashSet<UserSkill>();
        }

        public int ID { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}
