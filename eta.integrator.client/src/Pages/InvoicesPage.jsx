import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Flex, Card, message, notification } from "antd";
import { RightCircleOutlined } from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";
import CustomButton from "../Components/CustomButton";

import {
   EditFormItems,
   InvoicesSearchFormItems,
   InvoicesTableColumns,
} from "../Constants/ConstantsComponents";
import InvoicesService from "../Services/InvoicesService";
import { ROUTES, InvoiceTypes, InvoiceStatus } from "../Constants/Constants";
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
   const [isLoading, setIsLoading] = useState(false);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [currentRowToEdit, setCurrentRowToEdit] = useState(null);
   const [isEditModalOpen, setIsEditModalOpen] = useState(false);
   const [editFormInitialValues, setEditFormInitialValues] = useState({
      ReceiverName: "",
      RegistrationNumber: "",
   });
   const [searchInvoiceFormInitialValues, setSearchInvoiceFormInitialValues] = useState({
      InvoiceType: "I",
   });

   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const { getColumnSearchProps, filteredData } = useSearchColumn(tableData || []);
   const navigate = useNavigate();

   useEffect(() => {
      setEditFormInitialValues({
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

   //#region Edit Modal Handlers
   const handleOpenEditModal = (record) => {
      setCurrentRowToEdit(record);
      setIsEditModalOpen(true);
   };

   const handleEditModalCancel = () => {
      setIsEditModalOpen(false);
      setCurrentRowToEdit(null);
   };

   const handleEditFormValidation = (values) => {
      setIsLoading(true);

      if (isNaN(values.RegistrationNumber)) {
         notificationApi.error({
            message: "Registeration Number must be numeric.",
            duration: 0,
         });

         return false;
      }

      return true;
   };

   const handleEditSubmitClick = async (values) => {
      var isValid = handleEditFormValidation(values);

      if (!isValid) return setIsLoading(false);

      var editedRow = currentRowToEdit;
      editedRow.receiverName = values.ReceiverName;
      editedRow.registrationNumber = values.RegistrationNumber;
      try {
         await handleInvoiceSubmission([editedRow]);
         await handleSearch(searchValues);
         handleEditModalCancel();
      } catch (error) {
         notificationApi.error({
            message: error.detail,
            duration: 0,
         });
      } finally {
         setIsLoading(false);
      }
   };

   //#endregion

   //#region Invoice Search Form Handlers
   const disabledDate = (current) => {
      // Disable dates after today
      return current && current > new Date().setHours(0, 0, 0, 0);
   };

   const handleInvoiceSearchFormValidation = (values) => {
      const [dateFrom, dateTo] = values.DateRange || [];
      const invoiceTypeValue = values.InvoiceType;
      const invoiceTypeLabel =
         InvoiceTypes.find((i) => i.value === invoiceTypeValue)?.label ?? "";

      if (dateFrom && dateTo && dateFrom.isAfter(dateTo)) {
         throw {
            type: "validation",
            message: "Start date must be earlier than or equal to the end date.",
         };
      }

      var formattedValues = {
         dateFrom: dateFrom ? dateFrom.format("YYYY-MM-DD") : null,
         dateTo: dateTo ? dateTo.format("YYYY-MM-DD") : null,
         invoiceType: invoiceTypeValue,
      };

      var notificationMessage = `Showing ${invoiceTypeLabel}`;

      var notificationObject = {
         type: "success",
         message: notificationMessage,
         description: `from ${formattedValues.dateFrom} to ${formattedValues.dateTo}`,
         duration: 3,
      };
      return { formattedValues, notificationObject };
   };

   const handleInvoiceSearchClick = async (values) => {
      setIsLoading(true);
      const loadingMessage = messageApi.open({
         type: "loading",
         content: "Action in progress..",
         duration: 0,
      });

      try {
         const { formattedValues, notificationObject } =
            handleInvoiceSearchFormValidation(values);

         try {
            await handleSearch(formattedValues);
            notificationApi.open(notificationObject);
            setSearchInvoiceFormInitialValues({ InvoiceType: values.InvoiceType });
         } catch (error) {
            notificationApi.error({
               message: error.detail,
               duration: 0,
            });
            console.error(error.message);
         }
      } catch (error) {
         if (error.type === "validation") {
            messageApi.error(error.message);
            console.error(error.detail);
         } else {
            messageApi.error("Failed to fetch data. Please try again.");
            console.error(error.detail);
         }
      } finally {
         loadingMessage(); // Close the loading message
         setIsLoading(false); // End loading
      }
   };
   //#endregion

   const tableColumns = InvoicesTableColumns(getColumnSearchProps, handleOpenEditModal);
   const editFormItems = EditFormItems(isMobile, isLoading);
   const invoicesSearchFormItems = InvoicesSearchFormItems(
      isMobile,
      isLoading,
      disabledDate
   );
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
                  <CustomForm
                     key={"InvoiceSearchForm"}
                     name="invoiceSearchForm"
                     layout={isMobile ? "vertical" : "inline"}
                     initialValues={searchInvoiceFormInitialValues}
                     formItems={invoicesSearchFormItems}
                     handleSubmit={handleInvoiceSearchClick}
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
               key={"EditModal"}
               title={`Edit & Submit Invoice For ${
                  currentRowToEdit?.invoiceNumber ?? ""
               }`}
               isModalOpen={isEditModalOpen}
               handleCancel={handleEditModalCancel}
            >
               <CustomForm
                  key={"EditForm"}
                  name="editSubmitForm"
                  isMobile={isMobile}
                  initialValues={editFormInitialValues}
                  formItems={editFormItems}
                  handleSubmit={handleEditSubmitClick}
               />
            </CustomModal>
         </Card>
      </>
   );
};

export default InvoicesPage;
