namespace Laundro.Core.BusinessRequirementsValidators;
public class ValidatorResponse
{
    public bool IsSatisfied { 
        get {
            return !ErrorMessages.Any();
        }
    }

    public List<string> ErrorMessages { get; set; } = new List<string>();

    public void AddError(string message)
    {
        ErrorMessages.Add(message);
    }
}
