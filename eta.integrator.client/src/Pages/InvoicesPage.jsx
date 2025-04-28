import React, { useState } from "react";
import { Flex, Card, message } from "antd";

import InvoicesTable from "../Components/InvoicesTable";
import DateRangeSearch from "../Components/DateRangeSearch";

import { InvoicesTableColumns } from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";

const InvoicesPage = ({ isMobile }) => {
   const [loading, setLoading] = useState(false);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();

   const onSubmit = async (selectedRows) => {
      const loadingMessage = messageApi.open({
         type: "loading",
         content: "Action in progress..",
         duration: 0,
      });

      // Start loading
      setLoading(true);
      
      try {
         await InvoicesService.submitInvoices(selectedRows);
         messageApi.open({
            type: "success",
            content: "Selected rows saved successfully!",
            duration: 2,
         });
      } catch (error) {
         messageApi.error("Failed to save selected rows.");
         console.error("Error saving selected rows", error);
      } finally {
         loadingMessage(); // Close the loading message
         setLoading(false); // End loading
      }
   };

   const handleSearch = async (values) => {
      try {
         const response = await InvoicesService.getInvoicesAccordingToDateAsQueryParams(
            values
         );
         setTableData(response); // Update table data with the response
      } catch (error) {
         console.error("Failed to fetch invoices", error);
      }
   };

   return (
      <>
         {contextHolder}
         <Card style={{ width: "100%" }}>
            <Flex
               vertical
               gap="middle"
            >
               <DateRangeSearch
                  isMobile={isMobile}
                  handleSearch={handleSearch}
                  messageApi={messageApi}
               />

               <InvoicesTable
                  isMobile={isMobile}
                  tableData={tableData}
                  onSubmit={onSubmit}
                  loading={loading}
                  messageApi={messageApi}
                  tableType="W"
                  tableColumns={InvoicesTableColumns}
               />
            </Flex>
         </Card>
      </>
   );
};

export default InvoicesPage;
