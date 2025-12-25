import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Flex, Card, message, notification } from "antd";
import {
   FileTextOutlined,
   LeftCircleOutlined,
   RightCircleOutlined,
} from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";
import CustomButton from "../Components/CustomButton";
import CustomModal from "../Components/CustomModal";
import CustomForm from "../Components/CustomForm";
import CustomCardTitle from "../Components/CustomCardTitle";
import {
   EditFormItems,
   InvoicesSearchFormItems,
   InvoicesTableColumns,
} from "../Constants/Shared";
import { ROUTES, InvoiceTypes } from "../Constants/Constants";
import InvoicesService from "../Services/InvoicesService";
import GenericService from "../Services/GenericService";
import useSearchColumn from "../Hooks/useSearchColumn";
import TagsWrapper from "../Components/TagsWrapper";

const InvoicesPage = ({ isMobile, forNotes = false }) => {
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

   const { handleErrorNotification } = GenericService;

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
            currentRowToEdit ? "C" : searchValues.InvoiceType, // Set invoice type to claim when editing before submission
            forNotes
         );

         // Handle new version of response for better view
         if (!response.isError) {
            notificationApi.open({
               type: "success",
               message: <SubmissionNotification response={response} />,
               duration: 0,
            });
         }
      } catch (error) {
         console.error(error.detail);
         throw error;
      }
   };

   const handleSearch = async (values) => {
      try {
         var response = null;
         if (forNotes) {
            response = await InvoicesService.getNotesAccodingToDateAsQueryParams(values);
         } else {
            response = await InvoicesService.getInvoicesAccordingToDateAsQueryParams(
               values
            );
         }

         setSearchValues(values);
         setTableData(response); // Update table data with the response

         setSearchKey(searchKey + 1); // Force re-render of InvoicesTable by changing key

         return true;
      } catch (error) {
         console.error("Failed to fetch documents", error);
         throw error;
      }
   };

   const handleRevalidateSubmission = async (record) => {
      try {
         var response = await InvoicesService.revalidateSubmission(record.invoiceId);

         if (response) {
            await handleSearch(searchValues);
            notificationApi.open({
               type: "success",
               message: `Status updated succussefully for document #${record.invoiceNumber}`,
               duration: 3,
            });
         }
      } catch (error) {
         handleErrorNotification(notificationApi, error);
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
         notificationApi.warning({
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
         handleErrorNotification(notificationApi, error);
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
         DateFrom: dateFrom ? dateFrom.format("YYYY-MM-DD") : null,
         DateTo: dateTo ? dateTo.format("YYYY-MM-DD") : null,
         InvoiceType: invoiceTypeValue,
      };

      var notificationMessage = `Showing ${invoiceTypeLabel}`;

      var notificationObject = {
         type: "success",
         message: notificationMessage,
         description: `from ${formattedValues.DateFrom} to ${formattedValues.DateTo}`,
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
            var searchResponse = await handleSearch(formattedValues);
            if (searchResponse) notificationApi.open(notificationObject);

            setSearchInvoiceFormInitialValues({ InvoiceType: values.InvoiceType });
         } catch (error) {
            handleErrorNotification(notificationApi, error);
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

   const navigateToRoute = (route) => {
      setTableData([]);
      navigate(route);
   };

   const cardTitle = forNotes ? "Notes" : "Invoices";
   const tableColumns = InvoicesTableColumns(
      getColumnSearchProps,
      handleOpenEditModal,
      handleRevalidateSubmission
   );
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
         <Card
            title={
               <CustomCardTitle
                  title={cardTitle}
                  icon={<FileTextOutlined />}
               />
            }
            style={{ width: "100%" }}
         >
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

                  <Flex vertical>
                     <CustomButton
                        name={forNotes ? "Back to Invoices" : "Go to Notes"}
                        icon={forNotes ? <LeftCircleOutlined /> : <RightCircleOutlined />}
                        handleClick={() =>
                           forNotes
                              ? navigateToRoute(ROUTES.COMPLETED)
                              : navigateToRoute(ROUTES.NOTES)
                        }
                        type="link"
                        style={{ padding: 0 }}
                     />

                     <CustomButton
                        name="Go to Submitted Invoices"
                        icon={<RightCircleOutlined />}
                        handleClick={() => navigate(ROUTES.SUBMITTED)}
                        type="link"
                        style={{ padding: 0 }}
                     />
                  </Flex>
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

const SubmissionNotification = ({ response }) => {
   return (
      <Flex vertical>
         <p style={{ fontWeight: "bold", fontSize: "17px" }}>Submitted:</p>
         <TagsWrapper
            listOfElements={response.acceptedInvoices}
            color="green"
         />
         <p style={{ fontWeight: "bold", fontSize: "17px" }}>Rejected:</p>
         <TagsWrapper
            listOfElements={response.rejectedInvoices}
            color="red"
         />
      </Flex>
   );
};
