import { CheckCircleTwoTone, CloseCircleTwoTone } from "@ant-design/icons";
import dayjs from "dayjs";

const InvoicesTableColumns = (getColumnSearchProps) => [
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
];

const SubmittedInvoiceColumns = (getColumnSearchProps) => [
   {
      title: "UUID",
      dataIndex: "uuid",
      render: (text, record) => (
         <a
            target="_blank"
            href={record.publicUrl}
         >
            {text}
         </a>
      ),
      ...getColumnSearchProps("uuid", "search"),
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

export { InvoicesTableColumns, SubmittedInvoiceColumns };
