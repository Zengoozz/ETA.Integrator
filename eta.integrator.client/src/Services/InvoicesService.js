import { InvoiceStatus } from "../Constants/Constants";
import GenericService from "./GenericService";

const getInvoicesAccordingToDateAsQueryParams = async (values) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         `/Invoices/GetProviderInvoices?fromDate=${values.DateFrom}&toDate=${values.DateTo}&invoiceType=${values.InvoiceType}`
      );

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const getNotesAccodingToDateAsQueryParams = async (values) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         `/Invoices/GetProviderNotes?fromDate=${values.DateFrom}&toDate=${values.DateTo}&invoiceType=${values.InvoiceType}`
      );

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const submitInvoices = async (
   invoices,
   invoiceType,
   isResubmit = false,
   invoicesIds = []
) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "POST",
         "/Invoices/SubmitDocuments",
         {
            Invoices: invoices,
            InvoiceType: invoiceType,
            isResubmit: isResubmit,
            invoicesIds: invoicesIds,
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
      const status = InvoiceStatus.find((r) => r.value === values.InvoiceStatus).label;
      const url = `/Invoices/SearchDocuments?submissionDateFrom=${
         values.DateFrom
      }&submissionDateTo=${values.DateTo}&status=${status}&receiverType=${
         values.InvoiceType == "I" ? "P" : "B"
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
   getNotesAccodingToDateAsQueryParams,
   submitInvoices,
   getSubmittedInvoices,
   searchDocumentsWithFilters,
};
