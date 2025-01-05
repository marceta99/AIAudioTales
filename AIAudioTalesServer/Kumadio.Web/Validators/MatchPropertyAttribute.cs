using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class MatchPropertyAttribute : ValidationAttribute
{
    private readonly string _otherPropertyName;

    public MatchPropertyAttribute(string otherPropertyName)
    {
        _otherPropertyName = otherPropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 1) Get the type that contains the property we're comparing to
        var objectType = validationContext.ObjectType;

        // 2) Find the property (e.g. "Password") using reflection
        var otherProperty = objectType.GetProperty(_otherPropertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (otherProperty == null)
        {
            return new ValidationResult($"Property '{_otherPropertyName}' not found on {objectType.Name}.");
        }

        // 3) Get the comparison property value using reflection
        var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance);

        // 4) Compare the two values (the current property vs. the other)
        if (!object.Equals(value, otherPropertyValue))
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must match {_otherPropertyName}.");
        }

        return ValidationResult.Success;
    }
}
