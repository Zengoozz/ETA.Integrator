import { CheckCircleTwoTone, CloseCircleTwoTone } from "@ant-design/icons";
import { Button, Input } from "antd";
import { EditOutlined } from "@ant-design/icons";
import dayjs from "dayjs";

import { EditInvoiceRules } from "../Constants/Constants";

const InvoicesTableColumns = (getColumnSearchProps, handleOpenModal) => [
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
      render: (value) => {
         return (
            <span style={{ display: "flex", width: "100%", justifyContent: "center" }}>
               {value ? (
                  <CheckCircleTwoTone
                     style={{ fontSize: 30 }}
                     twoToneColor={["green", "transparent"]}
                  />
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
            <Button
               type="primary"
               icon={<EditOutlined />}
               size="large"
               onClick={() => handleOpenModal(record)}
               disabled={isDisabled}
            >
               Edit & Submit
            </Button>
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

const EditFormItems = (isMobile) => [
   {
      label: "Receiver Name",
      name: "ReceiverName",
      rules: EditInvoiceRules.receiverName,
      element: (
         <Input
            size={isMobile ? "large" : "middle"}
            autoComplete="off"
            allowClear={true}
         />
      ),
   },
   {
      label: "Registration Number",
      name: "RegistrationNumber",
      rules: EditInvoiceRules.registrationNumber,
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
];

export { InvoicesTableColumns, SubmittedInvoiceColumns, EditFormItems };
