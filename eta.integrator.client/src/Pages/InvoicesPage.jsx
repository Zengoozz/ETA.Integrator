import React, { useState } from "react";

import InvoicesTable from "../Componenets/InvoicesTable";
import SaveButton from "../Componenets/SaveButton";
import DateRangeSearch from "../Componenets/DateRangeSearch";

const InvoicesPage = () => {
   const [loading, setLoading] = useState(false);

   const onSave = async () => {
      setLoading(true); // Start loading
      //TODO: Simulate save operation (e.g., API call)
      await new Promise((resolve) => setTimeout(resolve, 2000)); // 2 seconds delay
      setLoading(false); // End loading
   };

   return (
      <>
         <DateRangeSearch />
         <SaveButton
            loading={loading}
            onSave={onSave}
         />
         <InvoicesTable />
      </>
   );
};

export default InvoicesPage;
