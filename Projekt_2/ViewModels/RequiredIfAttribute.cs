using System.ComponentModel.DataAnnotations;

namespace Projekt_2.ViewModels;

public class RequiredIfAttribute(string dependentProperty, object targetValue) : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(dependentProperty);
        if (property == null)
            return new ValidationResult($"Property {dependentProperty} not found");

        var propertyValue = property.GetValue(validationContext.ObjectInstance);

        if (propertyValue.Equals(targetValue))
        {
            if (value == null || (value is string stringValue && string.IsNullOrWhiteSpace(stringValue)))
                return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}