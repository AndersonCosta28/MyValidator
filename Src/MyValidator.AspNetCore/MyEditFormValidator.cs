using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
namespace MyValidator.AspNetCore;

public class MyEditFormValidator : ComponentBase
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter] private EditContext? _currentEditContext { get; set; }
    [Inject] IServiceProvider _services { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (this._currentEditContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(MyEditFormValidator)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(MyEditFormValidator)} " +
                $"inside an {nameof(EditForm)}.");
        }

        this._messageStore = new(this._currentEditContext);

        this._currentEditContext.OnValidationRequested += (s, e) =>
        {
            var editContext = s as EditContext;
            this._messageStore?.Clear();
            var model = editContext!.Model;
            var type = typeof(ValidatorBuilder<>).MakeGenericType(model.GetType());
            var validator = this._services.GetRequiredService(type);
            var validateMethod = validator.GetType().GetMethod("Validate");
            var results = validateMethod?.Invoke(validator, new object[] { model }) as List<ValidationResult>;

            foreach (var result in results ?? [])
            {
                List<string> errorsMessage = [];
                foreach (var error in result.Errors)
                {
                    var field = editContext.Field(error.Path);
                    this._messageStore!.Add(field, error.Message);
                }
            }

            editContext.NotifyValidationStateChanged();
        };


        this._currentEditContext.OnFieldChanged += (s, e) =>
            this._messageStore?.Clear(e.FieldIdentifier);
    }

    public void DisplayErrors(Dictionary<string, List<string>> errors)
    {
        if (this._currentEditContext is not null)
        {
            foreach (var err in errors)
            {
                this._messageStore?.Add(this._currentEditContext.Field(err.Key), err.Value);
            }

            this._currentEditContext.NotifyValidationStateChanged();
        }
    }

    public void ClearErrors()
    {
        this._messageStore?.Clear();
        this._currentEditContext?.NotifyValidationStateChanged();
    }
}