using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.DomainModels
{
    public class TransitAccountDebit
    {
        [Key]
        public int TransitAccountDebitId { get; set; }

        [Display(Name = "Сумма")]
        //[AmountLessThan("Account", ErrorMessage = "Not valid")]
        public decimal Amount { get; set; }

        public int TransitAccountId { get; set; }
        [ForeignKey("TransitAccountId")]
        public virtual TransitAccount Account { get; set; }

        public DateTime OperationDateTime { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
    }

    public class AmountLessThan : ValidationAttribute, IClientModelValidator
    {
        private readonly string _comparisonProperty;
        private decimal _balance;

        public AmountLessThan(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (decimal)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var a = property.GetValue(validationContext.ObjectInstance);
            var b = (TransitAccount)a;
            var c = b.Balance;
            _balance = ((TransitAccount)property.GetValue(validationContext.ObjectInstance)).Balance;

            if (currentValue > _balance)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-lessthan"] = error;
            context.Attributes["data-val-lessthan-balance"] = _balance.ToString(CultureInfo.InvariantCulture);
        }
    }
}
