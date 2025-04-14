import GenericService from "./GenericService";

const getInvoicesAccordingToDateAsQueryParams = async (values) => {
   try {
      const response = await GenericService.makeRequest(
         "GET",
         `/Invoices?fromDate=${values.dateFrom}&toDate=${values.dateTo}`
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
      const response = await GenericService.makeRequest(
         "POST",
         "/Invoices/Submit",
         invoices
      );
      console.log(response);
      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

export default { getInvoicesAccordingToDateAsQueryParams, submitInvoices };
