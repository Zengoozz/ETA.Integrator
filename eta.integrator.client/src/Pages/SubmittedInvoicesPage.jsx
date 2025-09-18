import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { Card, message, notification, Flex } from "antd";
import { LeftCircleOutlined } from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";
import CustomButton from "../Components/CustomButton";
import InvoiceSearchForm from "../Components/InvoiceSearchForm";

import useSearchColumn from "../Hooks/useSearchColumn";
import InvoicesService from "../Services/InvoicesService";
import { SubmittedInvoiceColumns } from "../Constants/ConstantsComponents";
import { ROUTES } from "../Constants/Constants";

const SubmittedInvoicesPage = ({ isMobile }) => {
   const [searchKey, setSearchKey] = useState(1);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const { getColumnSearchProps, filteredData } = useSearchColumn(tableData || []);

   const navigate = useNavigate();
   const tableColumns = SubmittedInvoiceColumns(getColumnSearchProps);

   const handleSearch = async (values) => {
      try {
         const response = await InvoicesService.searchDocumentsWithFilters(values);
         setTableData(response.result);
         setSearchKey(searchKey + 1);
      } catch (error) {
         notificationApi.error({
            message: error.detail,
            duration: 0,
         });
         console.error(error.message);
      }
   };

   // const handleSubmit =

   return (
      <>
         {contextHolder}
         {contextHolderNotification}
         <Card style={{ width: "100%" }}>
            <Flex justify="space-between">
               <InvoiceSearchForm
                  isMobile={isMobile}
                  handleSearch={handleSearch}
                  messageApi={messageApi}
                  notificationApi={notificationApi}
                  isStatusIncluded={true}
               />

               <CustomButton
                  name="Invoices"
                  icon={<LeftCircleOutlined />}
                  handleClick={() => navigate(ROUTES.COMPLETED)}
                  type="link"
                  style={{ padding: 0 }}
               />
            </Flex>

            <InvoicesTable
               key={searchKey} // Use searchKey to force re-render when it changes
               isMobile={isMobile}
               tableData={filteredData}
               messageApi={messageApi}
               notificationApi={notificationApi}
               tableType="W"
               tableColumns={tableColumns}
               isSubmittedInvoicesTable={true}
               // onSubmit={onSubmit}
               // submissionCallBack={() => handleSearch(searchValues)}
            />
         </Card>
      </>
   );
};

export default SubmittedInvoicesPage;
