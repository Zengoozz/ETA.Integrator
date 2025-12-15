import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { Card, message, notification, Flex } from "antd";
import { LeftCircleOutlined } from "@ant-design/icons";

import InvoicesTable from "../Components/InvoicesTable";
import CustomButton from "../Components/CustomButton";
import CustomForm from "../Components/CustomForm";
import { ROUTES, InvoiceTypes, InvoiceStatus } from "../Constants/Constants";
import { SubmittedInvoiceColumns, InvoicesSearchFormItems } from "../Constants/Shared";
import useSearchColumn from "../Hooks/useSearchColumn";
import InvoicesService from "../Services/InvoicesService";

const SubmittedInvoicesPage = ({ isMobile }) => {
   const [searchKey, setSearchKey] = useState(1);
   const [tableData, setTableData] = useState([]); // State to hold table data
   const [isLoading, setIsLoading] = useState(false);
   const [searchInvoiceFormInitialValues, setSearchInvoiceFormInitialValues] = useState({
      dateFrom: null,
      dateTo: null,
      InvoiceType: "I",
      InvoiceStatus: "V",
   });

   const [messageApi, contextHolder] = message.useMessage();
   const [notificationApi, contextHolderNotification] = notification.useNotification();
   const { getColumnSearchProps, filteredData } = useSearchColumn(tableData || []);

   const navigate = useNavigate();

   const handleResubmit = async (selectedRows) => {
      try {
         return await InvoicesService.submitInvoices(
            [],
            searchInvoiceFormInitialValues.invoiceType,
            true,
            selectedRows.map((r) => r.internalId)
         );
      } catch (error) {
         console.error(error.detail);
         throw error;
      }
   };

   const handleSearch = async (values) => {
      try {
         const response = await InvoicesService.searchDocumentsWithFilters(values);
         // setSearchValues(values);
         setSearchInvoiceFormInitialValues(values);
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

      const invoiceStatusLabel =
         InvoiceStatus.find((i) => i.value === values.InvoiceStatus)?.label ?? "all";

      var formattedValues = {
         DateFrom: dateFrom ? dateFrom.format("YYYY-MM-DD") : null,
         DateTo: dateTo ? dateTo.format("YYYY-MM-DD") : null,
         InvoiceType: invoiceTypeValue,
         InvoiceStatus: values.InvoiceStatus,
      };

      var notificationObject = {
         type: "success",
         message: `Showing ${invoiceTypeLabel} of ${invoiceStatusLabel} status`,
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
            await handleSearch(formattedValues);

            notificationApi.open(notificationObject);
            setSearchInvoiceFormInitialValues({
               InvoiceType: values.InvoiceType,
               InvoiceStatus: values.InvoiceStatus,
            });
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

   const tableColumns = SubmittedInvoiceColumns(getColumnSearchProps);
   const invoicesSearchFormItems = InvoicesSearchFormItems(
      isMobile,
      isLoading,
      disabledDate, 
      true
   );

   return (
      <>
         {contextHolder}
         {contextHolderNotification}
         <Card style={{ width: "100%" }}>
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
               buttonName="Re-submit"
               onSubmit={handleResubmit}
               submissionCallBack={() => handleSearch(searchInvoiceFormInitialValues)}
            />
         </Card>
      </>
   );
};

export default SubmittedInvoicesPage;
