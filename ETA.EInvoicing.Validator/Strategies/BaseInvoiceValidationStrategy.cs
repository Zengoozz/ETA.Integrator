using ETA.EInvoicing.Validator.Interfaces;
using ETA.EInvoicing.Validator.Models;
using System.Text.Json;

namespace ETA.EInvoicing.Validator.Strategies;

internal abstract class BaseInvoiceValidationStrategy : IInvoiceValidationStrategy
{
    protected readonly IEnumerable<IValidationStep> _steps;

    protected BaseInvoiceValidationStrategy()
    {
        _steps = this.BuildSteps();
    }

    public virtual async Task<List<StepErrorDto>> ValidateAsync(string invoiceJson)
    {
        var allStepsErros = new List<StepErrorDto>();
        int stepNumber = 1;
        var jsonDoc = JsonDocument.Parse(invoiceJson);  
        foreach (var step in _steps)
        {
            var stepErrors = await step.ValidateAsync(jsonDoc , stepNumber);

            if (stepErrors.Errors.Any())
                allStepsErros.Add(stepErrors);

            stepNumber++;
        }
        return allStepsErros;
    }
    protected abstract IEnumerable<IValidationStep> BuildSteps();
}
