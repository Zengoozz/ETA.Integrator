import GenericService from "./GenericService";

const getInvoicesAccordingToDateAsQueryParams = async (values) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         `/Invoices?fromDate=${values.dateFrom}&toDate=${values.dateTo}&invoiceType=${values.invoiceType}`
      );
      console.log(response);
      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const submitInvoices = async (invoices) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "POST",
         "/Invoices/SubmitInvoice",
         invoices
      );
      console.log(response);
      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const getSubmittedInvoices = async () => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         "/Invoices/GetRecent"
      );
      console.log(response);
      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

export default {
   getInvoicesAccordingToDateAsQueryParams,
   submitInvoices,
   getSubmittedInvoices,
};
