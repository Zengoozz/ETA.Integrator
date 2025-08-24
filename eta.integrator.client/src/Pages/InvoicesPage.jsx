import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Flex, Card, message, notification } from "antd";
import { RightCircleOutlined } from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";
import InvoiceSearchForm from "../Components/InvoiceSearchForm";
import CustomButton from "../Components/CustomButton";

import { InvoicesTableColumns } from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";
import { ROUTES } from "../Constants/Constants";

const InvoicesPage = ({ isMobile }) => {
   // const [loading, setLoading] = useState(false);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const navigate = useNavigate();

   const onSubmit = async (selectedRows) => {
      try {
         await InvoicesService.submitInvoices(selectedRows);
      } catch (error) {
         console.error(error.detail);
         throw error;
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
         throw error;
      }
   };

   return (
      <>
         {contextHolder}
         {contextHolderNotification}
         <Card style={{ width: "100%" }}>
            <Flex
               vertical
               gap="middle"
            >
               <Flex justify="space-between">
                  <InvoiceSearchForm
                     isMobile={isMobile}
                     handleSearch={handleSearch}
                     messageApi={messageApi}
                     notificationApi={notificationApi}
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
                  // loading={loading}
                  messageApi={messageApi}
                  notificationApi={notificationApi}
                  tableType="W"
                  tableColumns={InvoicesTableColumns}
               />
            </Flex>
         </Card>
      </>
   );
};

export default InvoicesPage;
