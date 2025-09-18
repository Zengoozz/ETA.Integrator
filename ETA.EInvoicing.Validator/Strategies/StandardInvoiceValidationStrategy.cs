using ETA.EInvoicing.Validator.Interfaces;
namespace ETA.EInvoicing.Validator.Strategies;

internal class StandardInvoiceValidationStrategy : BaseInvoiceValidationStrategy
{
    public StandardInvoiceValidationStrategy() : base()
    {

    }

    protected override IEnumerable<IValidationStep> BuildSteps()
    {
        return new List<IValidationStep> {
        
        };
    }  
}
