import React, { useState } from "react";
import { DatePicker, Button, Form, Flex, message } from "antd";
// eslint-disable-next-line no-unused-vars
import dayjs from "dayjs";
import InvoicesService from "../Services/InvoicesService";

const { RangePicker } = DatePicker;

const DateRangeSearch = ({ isMobile }) => {
   const [form] = Form.useForm();
   const [loading, _setLoading] = useState(false);

   const handleSearch = () => {
      form
         .validateFields()
         .then((values) => {
            const { dateFrom, dateTo } = values;

            const formattedValues = {
               dateFrom: dateFrom ? dateFrom.format("YYYY-MM-DD") : null,
               dateTo: dateTo ? dateTo.format("YYYY-MM-DD") : null,
            };

            message.success(
               `Searching from ${formattedValues.dateFrom} to ${formattedValues.dateTo}`
            );
            // Call your API here
            var test =
               InvoicesService.getInvoicesAccordingToDateAsQueryParams(formattedValues);
            console.log(test);
         })
         .catch(() => {
            message.error("Please select valid dates.");
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
               name="dateFrom"
               rules={[{ required: true, message: "Please select start date" }]}
               style={{ flex: 1 }}
            >
               <DatePicker
                  placeholder="From Date"
                  style={{ width: "100%" }}
               />
            </Form.Item>

            <Form.Item
               name="dateTo"
               rules={[{ required: true, message: "Please select end date" }]}
               style={{ flex: 1 }}
            >
               <DatePicker
                  placeholder="To Date"
                  style={{ width: "100%" }}
               />
            </Form.Item>

            <Form.Item>
               <Button
                  type="primary"
                  onClick={handleSearch}
                  loading={loading}
                  block={isMobile} // Full width on mobile
               >
                  Search
               </Button>
            </Form.Item>
         </Flex>
      </Form>
   );
};

export default DateRangeSearch;
