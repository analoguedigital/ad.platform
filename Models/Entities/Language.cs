using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class Language : Entity, IEnumEntity
    {
        [ScaffoldColumn(false)]
        public int Order { get; set; }

        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public string SystemName { get; set; }

        public string Calture { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
