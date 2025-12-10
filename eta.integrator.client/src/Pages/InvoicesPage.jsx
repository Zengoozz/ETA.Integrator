import { useEffect, useState } from "react";
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
import CustomModal from "../Components/CustomModal";
import CustomForm from "../Components/CustomForm";

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
   const [editModalInitialValues, setEditModalInitialValues] = useState({
      ReceiverName: "",
      RegistrationNumber: "",
   });

   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const { getColumnSearchProps, filteredData } = useSearchColumn(tableData || []);
   const navigate = useNavigate();

   useEffect(() => {
      setEditModalInitialValues({
         ReceiverName: currentRowToEdit?.receiverName || "",
         RegistrationNumber: currentRowToEdit?.registrationNumber || "",
      });
   }, [currentRowToEdit]);

   const handleInvoiceSubmission = async (selectedRows) => {
      try {
         var response = await InvoicesService.submitInvoices(
            selectedRows,
            searchValues.invoiceType
         );

         notificationApi.open({
            type: "success",
            message: (
               <span
                  dangerouslySetInnerHTML={{
                     __html: response.responseMessage.replace(/\n/g, "<br/>"),
                  }}
               />
            ),
            duration: 0,
         });
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

   const handleOpenEditModal = (record) => {
      setCurrentRowToEdit(record);
      setIsEditModalOpen(true);
   };

   const handleEditSubmitClick = async (values) => {
      var editedRow = currentRowToEdit;
      editedRow.receiverName = values.ReceiverName;
      editedRow.registrationNumber = values.RegistrationNumber;
      return await handleInvoiceSubmission([editedRow]);
   };

   const handleEditFormValidation = (values) => {
      if (isNaN(values.RegistrationNumber)) {
         notificationApi.error({
            message: "Registeration Number must be numeric.",
            duration: 0,
         });

         return false;
      }

      return true;
   };

   const handleEditFormCallback = () => {
      handleSearch(searchValues);
      handleEditModalCancel();
   };

   const handleEditModalCancel = () => {
      setIsEditModalOpen(false);
      setCurrentRowToEdit(null);
   };

   const tableColumns = InvoicesTableColumns(getColumnSearchProps, handleOpenEditModal);
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
                  onSubmit={handleInvoiceSubmission}
                  submissionCallBack={() => handleSearch(searchValues)}
               />
            </Flex>

            <CustomModal
               title={`Edit & Submit Invoice For ${
                  currentRowToEdit?.invoiceNumber ?? ""
               }`}
               isModalOpen={isEditModalOpen}
               handleCancel={handleEditModalCancel}
            >
               <CustomForm
                  name="editSubmitForm"
                  isMobile={isMobile}
                  notificationApi={notificationApi}
                  initialValues={editModalInitialValues}
                  handleSubmit={handleEditSubmitClick}
                  handleFormValidation={handleEditFormValidation}
                  handleSubmitCallback={handleEditFormCallback}
               />
            </CustomModal>
         </Card>
      </>
   );
};

export default InvoicesPage;
