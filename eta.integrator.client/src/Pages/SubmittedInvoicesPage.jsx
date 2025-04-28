import React, { useState, useEffect } from "react";
import { Card, message } from "antd";

import InvoicesTable from "../Components/InvoicesTable";

import { InvoicesTableColumns } from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";

const SubmittedInvoicesPage = ({ isMobile }) => {
   const [loading, setLoading] = useState(false);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();

   useEffect(() => {
      const fetchTableData = async () => {
         const loadingMessage = messageApi.open({
            type: "loading",
            content: "Action in progress..",
            duration: 0,
         });

         setLoading(true);

         try {
            const response = await InvoicesService.getSubmittedInvoices();
            setTableData(response);
         } catch (err) {
            messageApi.error("Failed to fetch recent documents");
            console.log("Failed to fetch recent documents", err);
         } finally {
            loadingMessage(); // Close the loading message
            setLoading(false); // End loading
         }
      };

      fetchTableData();
   }, [messageApi]);

   return (
      <>
         {contextHolder}
         <Card style={{ width: "100%" }}>
            <InvoicesTable
               isMobile={isMobile}
               tableData={tableData}
               loading={loading}
               messageApi={messageApi}
               tableType="R"
               tableColumns={InvoicesTableColumns}
            />
         </Card>
      </>
   );
};

export default SubmittedInvoicesPage;
