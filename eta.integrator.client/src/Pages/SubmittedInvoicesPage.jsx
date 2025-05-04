import React, { useState, useEffect } from "react";
import { Card, message } from "antd";
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
