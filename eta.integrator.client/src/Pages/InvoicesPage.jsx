import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Flex, Card, message, notification } from "antd";
import { RightCircleOutlined } from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";
import InvoiceSearchForm from "../Components/InvoiceSearchForm";
import CustomButton from "../Components/CustomButton";

import { InvoicesTableColumns } from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";
import { ROUTES } from "../Constants/Constants";
import useSearchColumn from "../Hooks/useSearchColumn";

const InvoicesPage = ({ isMobile }) => {
   // const [loading, setLoading] = useState(false);
   const [searchKey, setSearchKey] = useState(1);

   const [searchValues, setSearchValues] = useState({
      dateFrom: null,
      dateTo: null,
      invoiceType: "I",
   });
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const { getColumnSearchProps, filteredData } = useSearchColumn(tableData || []);
   
   const navigate = useNavigate();
   const tableColumns = InvoicesTableColumns(getColumnSearchProps);

   const onSubmit = async (selectedRows) => {
      try {
         return await InvoicesService.submitInvoices(
            selectedRows,
            searchValues.invoiceType
         );
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
         setSearchValues(values);
         setTableData(response); // Update table data with the response

         setSearchKey(searchKey + 1); // Force re-render of InvoicesTable by changing key
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
                  key={searchKey} // Use searchKey to force re-render
                  isMobile={isMobile}
                  tableData={filteredData}
                  messageApi={messageApi}
                  notificationApi={notificationApi}
                  tableType="W"
                  tableColumns={tableColumns}
                  onSubmit={onSubmit}
                  submissionCallBack={() => handleSearch(searchValues)}
               />
            </Flex>
         </Card>
      </>
   );
};

export default InvoicesPage;
