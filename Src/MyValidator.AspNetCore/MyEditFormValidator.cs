using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
namespace MyValidator.AspNetCore;

public class MyEditFormValidator : ComponentBase
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter] private EditContext? _currentEditContext { get; set; } = default!;
    [Parameter] public EditForm OtherEditForm { get; set; } = default!;
    [Inject] IServiceProvider _services { get; set; } = default!;

    protected override void OnInitialized()
    {
        this.configureEditContext(OtherEditForm?.EditContext ?? _currentEditContext);
    }

    private void configureEditContext(EditContext editContext)
    {
        if (editContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(MyEditFormValidator)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(MyEditFormValidator)} " +
                $"inside an {nameof(EditForm)}.");
        }

        this._messageStore = new(editContext);

        editContext.OnValidationRequested += (s, e) =>
        {
            var currentEditContext = s as EditContext;
            this._messageStore?.Clear();
            var model = currentEditContext!.Model;
            var type = typeof(ValidatorBuilder<>).MakeGenericType(model.GetType());
            var validator = this._services.GetService(type);
            if (validator == null)
                return;
            var validateMethod = validator.GetType().GetMethod("Validate");
            var results = validateMethod?.Invoke(validator, new object[] { model }) as List<ValidationResult>;

            foreach (var result in results ?? [])
            {
                foreach (var error in result.Errors)
                {
                    var field = currentEditContext.Field(error.Path);
                    this._messageStore!.Add(field, error.Message);
                }
            }

            currentEditContext.NotifyValidationStateChanged();
        };

        editContext.OnFieldChanged += (s, e) =>
            this._messageStore?.Clear(e.FieldIdentifier);
    }
}