import React, { useState } from "react";
import { DatePicker, Button, Form, Flex, Select } from "antd";
import { InvoiceSearchValidationRules, InvoiceTypes } from "../Constants/Constants";

const { RangePicker } = DatePicker;
const { Option } = Select;

const InvoiceSearchForm = ({ isMobile, handleSearch, messageApi }) => {
   const [form] = Form.useForm();
   const [loading, setLoading] = useState(false);

   const disabledDate = (current) => {
      // Disable dates before today
      return current && current > new Date().setHours(0, 0, 0, 0);
   };

   const onSearchClick = () => {
      setLoading(true); // Start loading

      const loadingMessage = messageApi.open({
         type: "loading",
         content: "Action in progress..",
         duration: 0,
      });

      form
         .validateFields()
         .then((values) => {
            const [dateFrom, dateTo] = values.dateRange || [];
            const invoiceTypeValue = values.InvoiceType;
            const invoiceTypeLabel = InvoiceTypes.find(i => i.value === invoiceTypeValue)?.label ?? "";
            
            if (dateFrom && dateTo && dateFrom.isAfter(dateTo)) {
               throw {
                  type: "validation",
                  message: "Start date must be earlier than or equal to the end date.",
               };
            }

            const formattedValues = {
               dateFrom: dateFrom ? dateFrom.format("YYYY-MM-DD") : null,
               dateTo: dateTo ? dateTo.format("YYYY-MM-DD") : null,
               invoiceType: invoiceTypeValue
            };

            return handleSearch(formattedValues).then(() => {
               messageApi.open({
                  type: "success",
                  content: `Showing ${invoiceTypeLabel} from ${formattedValues.dateFrom} to ${formattedValues.dateTo}`,
                  duration: 2,
               });
            });
         })
         .catch((error) => {
            if (error.type === "validation") {
               messageApi.error(error.message);
            } else {
               messageApi.error("Failed to fetch data. Please try again.");
               console.error("Error:", error);
            }
         })
         .finally(() => {
            loadingMessage(); // Close the loading message
            setLoading(false); // End loading
         });
   };

   return (
      <Form
         form={form}
         layout={isMobile ? "vertical" : "inline"}
      >
         <Flex
            gap="small"
            wrap
         >
            <Form.Item
               name="dateRange"
               rules={[
                  {
                     required: true,
                     message: "Please select a valid date range",
                  },
               ]}
               style={{ flex: 1 }}
            >
               <RangePicker
                  placeholder={["Start Date", "End Date"]}
                  style={{ width: "100%" }}
                  disabledDate={disabledDate}
                  autoComplete="off"
               />
            </Form.Item>
            
            <Form.Item //TODO: Styling the dropdown
               // label="Invoice Type"
               name="InvoiceType"
               rules={InvoiceSearchValidationRules.invoiceType}
            >
               <Select placeholder="Please select invoice type">
                  {InvoiceTypes.map((type) => (
                     <Option
                        key={type.value}
                        value={type.value}
                     >
                        {type.label}
                     </Option>
                  ))}
               </Select>
            </Form.Item>

            <Form.Item>
               <Button
                  type="primary"
                  onClick={onSearchClick}
                  loading={loading}
                  block={isMobile} // Full width on mobile
                  disabled={loading}
               >
                  Search
               </Button>
            </Form.Item>
         </Flex>
      </Form>
   );
};

export default InvoiceSearchForm;
