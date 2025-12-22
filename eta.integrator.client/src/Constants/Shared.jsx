import {
   CheckCircleTwoTone,
   CloseCircleTwoTone,
   IssuesCloseOutlined,
} from "@ant-design/icons";
import { Button, Input, DatePicker, Select, Flex } from "antd";
import { EditOutlined, RedoOutlined } from "@ant-design/icons";
import dayjs from "dayjs";

import {
   EditInvoiceRules,
   InvoiceSearchValidationRules,
   DocumentTypes,
   InvoiceTypes,
   InvoiceStatus,
} from "./Constants";

const { RangePicker } = DatePicker;
const { Option } = Select;

const InvoicesTableColumns = (
   getColumnSearchProps,
   handleOpenModal,
   handleRevalidateSubmission
) => [
   {
      title: "Receipt Number",
      dataIndex: "invoiceNumber",
      render: (text) => <a>{text}</a>,
      ...getColumnSearchProps("invoiceNumber", "search"),
   },
   {
      title: "Visit Type",
      dataIndex: "invoiceType",
      render: (text) => text,
      ...getColumnSearchProps("invoiceType", "search"),
   },
   {
      title: "Company",
      dataIndex: "receiverName",
      render: (text) => text,
      ...getColumnSearchProps("receiverName", "search"),
   },
   {
      title: "Tax Registeration Number",
      dataIndex: "registrationNumber",
      render: (text) => text,
      ...getColumnSearchProps("registrationNumber", "search"),
   },
   {
      title: "Net Price",
      dataIndex: "netPrice",
      render: (value) => value.toFixed(2),
   },
   // {
   //    title: "Vat Net",
   //    dataIndex: "vatNet",
   //    render: (value) => value.toFixed(2),
   // },
   {
      title: "Date",
      dataIndex: "createdDate",
      render: (value) => <>{dayjs(value).format("DD/MM/YYYY")}</>,
   },
   {
      title: "Status",
      dataIndex: "isReviewed",
      render: (value, record) => {
         return (
            <span style={{ display: "flex", width: "100%", justifyContent: "center" }}>
               {value ? (
                  record.reviewStatus == "Valid" ? (
                     <CheckCircleTwoTone
                        style={{ fontSize: 30 }}
                        twoToneColor={["green", "transparent"]}
                     />
                  ) : (
                     <IssuesCloseOutlined
                        style={{ fontSize: 30, color: "greenyellow" }}
                     />
                  )
               ) : (
                  <CloseCircleTwoTone
                     style={{ fontSize: 30 }}
                     twoToneColor={["red", "transparent"]}
                  />
               )}
            </span>
         );
      },
   },
   {
      title: "Actions",
      dataIndex: "invoiceNumber",
      render: (_, record) => {
         var isDisabled = record.isReviewed || record.invoiceId.startsWith("C-");

         return (
            <Flex gap="small">
               <Button
                  type="primary"
                  icon={<EditOutlined />}
                  size="large"
                  onClick={() => handleOpenModal(record)}
                  disabled={isDisabled}
               >
                  Edit & Submit
               </Button>

               <Button
                  type="primary"
                  icon={<RedoOutlined />}
                  size="large"
                  onClick={() => handleRevalidateSubmission(record)}
                  disabled={record.reviewStatus != "Submitted"}
               >
                  Re-validate
               </Button>
            </Flex>
         );
      },
   },
];

const SubmittedInvoiceColumns = (getColumnSearchProps) => [
   {
      title: "UUID",
      dataIndex: "uuid",
      render: (_, record) => (
         <a
            target="_blank"
            href={record.publicUrl}
         >
            {record.uuid}
         </a>
      ),
      // ...getColumnSearchProps("uuid", "search"),
   },
   {
      title: "Internal Id",
      dataIndex: "internalId",
      render: (text) => <>{text}</>,
      ...getColumnSearchProps("internalId", "search"),
   },
   {
      title: "Document type",
      dataIndex: "typeName",
      ...getColumnSearchProps("typeName", "search"),
      render: (typeName, record) => {
         var documentType = DocumentTypes(record.receiverType).find(
            (d) => d.value.toLowerCase() == typeName
         );

         return <>{documentType.label}</>;
      },
   },
   {
      title: "Date Time Received",
      dataIndex: "dateTimeReceived",
      render: (value) => <>{dayjs(value).format("DD/MM/YYYY HH:mm:ss")}</>,
   },
   {
      title: "Total Value",
      dataIndex: "total",
      render: (text) => <>{text}</>,
   },
   {
      title: "Issuer",
      dataIndex: "issuerName",
      render: (text) => <>{text.trim()}</>,
   },
   {
      title: "Receiver",
      dataIndex: "receiverName",
      render: (text) => <>{text}</>,
      ...getColumnSearchProps("receiverName", "search"),
   },
   {
      title: "Status",
      dataIndex: "status",
      render: (text) => <>{text}</>,
      ...getColumnSearchProps("status", "search", true),
   },
];

const EditFormItems = (isMobile, isLoading) => {
   return {
      WrapperElement: null,
      Elements: [
         {
            showItem: true,
            label: "Receiver Name",
            name: "ReceiverName",
            rules: EditInvoiceRules.receiverName,
            style: null,
            element: (
               <Input
                  size={isMobile ? "large" : "middle"}
                  autoComplete="off"
                  allowClear={true}
               />
            ),
         },
         {
            showItem: true,
            label: "Registration Number",
            name: "RegistrationNumber",
            rules: EditInvoiceRules.registrationNumber,
            style: null,
            element: (
               <Input
                  className="ant-input-number-no-arrows"
                  type="number"
                  maxLength={14}
                  size={isMobile ? "large" : "middle"}
                  autoComplete="off"
                  allowClear
               />
            ),
         },
         {
            showItem: true,
            label: null,
            name: "SubmissionButton",
            rules: null,
            style: null,
            element: (
               <Button
                  type="primary"
                  htmlType="submit"
                  loading={isLoading}
                  block={isMobile}
                  disabled={isLoading}
                  size={isMobile ? "large" : "middle"}
               >
                  Submit
               </Button>
            ),
         },
      ],
   };
};

const InvoicesSearchFormItems = (
   isMobile,
   isLoading,
   disabledDate,
   isStatusIncluded = false
) => {
   return {
      WrapperElement: (
         <Flex
            vertical={true}
            gap="small"
            wrap
         />
      ),
      Elements: [
         {
            showItem: true,
            label: null,
            name: "DateRange",
            rules: InvoiceSearchValidationRules.dateRange,
            style: null,
            element: (
               <RangePicker
                  placeholder={["Start Date", "End Date"]}
                  style={{ width: "100%" }}
                  disabledDate={disabledDate}
                  autoComplete="off"
               />
            ),
         },
         {
            showItem: true,
            label: null,
            name: "InvoiceType",
            rules: InvoiceSearchValidationRules.invoiceType,
            style: null,
            element: (
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
            ),
         },
         {
            showItem: isStatusIncluded,
            label: null,
            name: "InvoiceStatus",
            rules: InvoiceSearchValidationRules.invoiceStatus,
            style: null,
            element: (
               <Select placeholder="Please select invoice status">
                  {InvoiceStatus.map((type) => (
                     <Option
                        key={type.value}
                        value={type.value}
                     >
                        {type.label}
                     </Option>
                  ))}
               </Select>
            ),
         },
         {
            showItem: true,
            label: null,
            name: "SubmissionButton",
            rules: null,
            style: null,
            element: (
               <Button
                  type="primary"
                  htmlType="submit"
                  loading={isLoading}
                  block={isMobile} // Full width on mobile
                  disabled={isLoading}
               >
                  Search
               </Button>
            ),
         },
      ],
   };
};

export {
   InvoicesTableColumns,
   SubmittedInvoiceColumns,
   EditFormItems,
   InvoicesSearchFormItems,
};
