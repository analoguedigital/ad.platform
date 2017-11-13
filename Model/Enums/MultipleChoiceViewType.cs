using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
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
