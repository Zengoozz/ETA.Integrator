using ETA.EInvoicing.Validator.Interfaces;
namespace ETA.EInvoicing.Validator.Strategies;

internal class FinancialNoteValidationStrategy : BaseInvoiceValidationStrategy
{
    public FinancialNoteValidationStrategy() : base()
    {

    }

    protected override IEnumerable<IValidationStep> BuildSteps()
    {
        return new List<IValidationStep> {
        
        };
    }  
}
