using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace carRentals.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        public DateGreaterThanAttribute(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

        private string DateToCompareToFieldName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime earlierDate = (DateTime)value;

            DateTime laterDate = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);

            if (laterDate > earlierDate)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Please choose a valid date range.");
            }
        }
    }
    public class RentViewModel : BaseEntity
    {
        [HiddenInput]
        public long car_id { get; set; }
        [Required(ErrorMessage = "Please select a car.")]
        public string selection { get; set; }
        [DataType(DataType.Date), DateGreaterThan("DateTime.Now.ToString()")]
        public DateTime rented_at { get; set; }

        [DataType(DataType.Date), DateGreaterThan("rented_at")]
        public DateTime return_at { get; set; }

    }
}