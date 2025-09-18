import { InvoiceStatus } from "../Constants/Constants";
import GenericService from "./GenericService";

const getInvoicesAccordingToDateAsQueryParams = async (values) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         `/Invoices?fromDate=${values.dateFrom}&toDate=${values.dateTo}&invoiceType=${values.invoiceType}`
      );

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const submitInvoices = async (invoices, invoiceType) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "POST",
         "/Invoices/SubmitDocuments",
         {
            Invoices: invoices,
            InvoiceType: invoiceType,
         }
      );

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

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const searchDocumentsWithFilters = async (values) => {
   try {
      const status = InvoiceStatus.find((r) => r.value === values.invoiceStatus).label;
      const url = `/Invoices/SearchDocuments?submissionDateFrom=${
         values.dateFrom
      }&submissionDateTo=${values.dateTo}&status=${status}&receiverType=${
         values.invoiceType == "I" ? "P" : "B"
      }`;

      const response = await GenericService.makeRequestFactory("GET", url);

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
   searchDocumentsWithFilters,
};
