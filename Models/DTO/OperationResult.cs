using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.DTO
{
    public class OperationResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public List<ValidationResult> ValidationErrors { get; set; }

        public Guid? NewRecordId { get; set; }

        public object ReturnValue { get; set; }

        public OperationResult()
        {
            this.ValidationErrors = new List<ValidationResult>();
        }
    }
}
