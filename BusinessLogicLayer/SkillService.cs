using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogicLayer
{
    public class SkillService
    {
        
        private readonly SkillTrackerDBEntities db;

        public SkillService()
        {
            db = new SkillTrackerDBEntities(); // You may want to use dependency injection to inject SkillTrackerDBEntities
        }

        public List<SkillDTO> GetListofSkills()
        {
            DbSet<Skill> dalSkill = db.Skills; // return User from DAL.

            List<SkillDTO> skills = new List<SkillDTO>();

            foreach (var item in dalSkill)
            {
                SkillDTO skill = new SkillDTO();

                skill.ID = item.ID;

                skill.Name = item.Name;

                skills.Add(skill);               
           }
            return skills;
        }


        public void AddSkill(SkillDTO skillDTO)
        {
            // Check if the skill already exists
            var existingSkill = db.Skills.FirstOrDefault(s => s.Name == skillDTO.Name);

            if (existingSkill != null)
            {
                // Skill already exists, you may handle this case as per your requirements (e.g., return a message)
                return;
            }

            // Create a new Skill entity and map properties from SkillDTO
            var newSkill = new Skill
            {
                ID = skillDTO.ID,
                Name = skillDTO.Name
            };

            // Add the new skill to the database
             db.Skills.Add(newSkill);
             db.SaveChanges();
        }

        public void UpdateSkill(int skillId, SkillDTO skillDTO)
        {
            // Find the skill by its ID
            var existingSkill =  db.Skills.FirstOrDefault(s => s.ID == skillId);

            if (existingSkill != null)
            {
                // Update the skill name
                existingSkill.Name = skillDTO.Name;

                // Save changes to the database
                 db.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Skill not found");
            }
        }
    }
}
