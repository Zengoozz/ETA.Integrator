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
import ActionModal from "../Components/ActionModal";

const InvoicesPage = ({ isMobile }) => {
   const [searchKey, setSearchKey] = useState(1);

   const [searchValues, setSearchValues] = useState({
      dateFrom: null,
      dateTo: null,
      invoiceType: "I",
   });
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [currentRowToEdit, setCurrentRowToEdit] = useState(null);
   const [isEditModalOpen, setIsEditModalOpen] = useState(false);

   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const { getColumnSearchProps, filteredData } = useSearchColumn(tableData || []);

   const navigate = useNavigate();

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

   const handleOpenModal = (record) => {
      setCurrentRowToEdit(record);
      setIsEditModalOpen(true);
   };

   const handleEditSubmitClick = async (values) => {
      var editedRow = currentRowToEdit;
      editedRow.receiverName = values.ReceiverName;
      editedRow.registrationNumber = values.RegistrationNumber;
      return await onSubmit([editedRow]);
   };

   const handleCancel = () => {
      setIsEditModalOpen(false);
      setCurrentRowToEdit(null);
   };

   const tableColumns = InvoicesTableColumns(getColumnSearchProps, handleOpenModal);
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

            <ActionModal
               title="Edit & Submit Invoice For"
               isModalOpen={isEditModalOpen}
               handleOk={handleEditSubmitClick}
               handleCancel={handleCancel}
               data={currentRowToEdit}
               isMobile={isMobile}
               notificationApi={notificationApi}
               callback={() => handleSearch(searchValues)}
            />
         </Card>
      </>
   );
};

export default InvoicesPage;
