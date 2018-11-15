using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Enums
{
    public enum MultipleChoiceViewType
    {
        [Display(Name = "Radio buttons")]
        RadioButtonList = 1,

        [Display(Name = "Check boxes")]
        CheckBoxList = 2,

        [Display(Name = "Drop down")]
        DropDown = 3,
    }
}
