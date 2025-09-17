import React, { useState, useEffect } from "react";
import { Card, message, notification } from "antd";
import { LeftCircleOutlined } from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";

import { SubmittedInvoiceColumns } from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";
import { ROUTES } from "../Constants/Constants";
import { useNavigate } from "react-router-dom";
import CustomButton from "../Components/CustomButton";
import useSearchColumn from "../Hooks/useSearchColumn";

const SubmittedInvoicesPage = ({ isMobile }) => {
   const [loading, setLoading] = useState(false);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const { getColumnSearchProps, filteredData } = useSearchColumn(tableData || []);

   const navigate = useNavigate();
   const tableColumns = SubmittedInvoiceColumns(getColumnSearchProps);

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
               message: error.detail,
               duration: 0,
            });
            console.error(error.message);
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
               tableData={filteredData}
               loading={loading}
               messageApi={messageApi}
               tableType="R"
               tableColumns={tableColumns}
            />
         </Card>
      </>
   );
};

export default SubmittedInvoicesPage;
