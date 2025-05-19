import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Flex, Card, message } from "antd";
import { RightCircleOutlined } from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";
import InvoiceSearchForm from "../Components/InvoiceSearchForm";
import CustomButton from "../Components/CustomButton";

import { InvoicesTableColumns } from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";
import { ROUTES } from "../Constants/Constants";

const InvoicesPage = ({ isMobile }) => {
   const [loading, setLoading] = useState(false);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();
   const navigate = useNavigate();

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
               <Flex 
               justify="space-between"
               >
                  <InvoiceSearchForm
                     isMobile={isMobile}
                     handleSearch={handleSearch}
                     messageApi={messageApi}
                  />

                  <CustomButton
                     name="Submitted Invoices"
                     icon={<RightCircleOutlined />}
                     handleClick={() => navigate(ROUTES.SUBMITTED)}
                     type="link"
                     style={{ padding: 0 }}
                  />
               </Flex>

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
