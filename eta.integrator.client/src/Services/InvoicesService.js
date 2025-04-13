const getInvoices = async (values) => {
   const response = await fetch("HMS/Invoices/", {
      method: "POST",
      headers: {
         "Content-Type": "application/json",
      },
      body: JSON.stringify({
        fromDate: values.dateFrom,
         toDate: values.dateTo,
      }),
   });

   const data = await response.data();

   return data;
};

const getInvoicesAccordingToDateAsQueryParams = async (values) => {
   const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Invoices/?fromDate=${values.dateFrom}&toDate=${values.dateTo}`, {
      method: "GET",
      headers: {
         "Content-Type": "application/json",
      },
   });

   console.log(response);

   // const data = await response.data();

   return 0;
};

const getInvoiceById = async (id) => {
   const response = await fetch(`HMS/Invoices/${id}`, {
      method: "GET",
      headers: {
         "Content-Type": "application/json",
      },
   });

   const data = await response.data();

   return data;
};

export default { getInvoices, getInvoiceById, getInvoicesAccordingToDateAsQueryParams };
