import React, { useState, useEffect } from "react";
import { Card, message, notification } from "antd";
import { LeftCircleOutlined } from "@ant-design/icons";


import InvoicesTable from "../Components/InvoicesTable";

import { SubmittedInvoiceColumns } from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";
import { ROUTES } from "../Constants/Constants";
import { useNavigate } from "react-router-dom";
import CustomButton from "../Components/CustomButton";

const SubmittedInvoicesPage = ({ isMobile }) => {
   const [loading, setLoading] = useState(false);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const navigate = useNavigate();

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
            setTableData(response.result);
         } catch (error) {
            notificationApi.error({
               message: error.message,
               duration: 0,
            });
            console.error(error.detail);
         } finally {
            loadingMessage(); // Close the loading message
            setLoading(false); // End loading
         }
      };

      fetchTableData();
   }, [messageApi, notificationApi]);

   return (
      <>
         {contextHolder}
         {contextHolderNotification}
         <Card style={{ width: "100%" }}>
            <CustomButton
               name="Invoices"
               icon={<LeftCircleOutlined />}
               handleClick={() => navigate(ROUTES.COMPLETED)}
               type="link"
               style={{ padding: 0 }}
            />

            <InvoicesTable
               isMobile={isMobile}
               tableData={tableData}
               loading={loading}
               messageApi={messageApi}
               tableType="R"
               tableColumns={SubmittedInvoiceColumns}
            />
         </Card>
      </>
   );
};

export default SubmittedInvoicesPage;
