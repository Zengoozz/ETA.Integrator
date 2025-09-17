import { CheckCircleTwoTone, CloseCircleTwoTone } from "@ant-design/icons";
import dayjs from "dayjs";

const InvoicesTableColumns = (getColumnSearchProps) => ([
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
   {
      title: "Vat Net",
      dataIndex: "vatNet",
      render: (value) => value.toFixed(2),
   },
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
]);

const SubmittedInvoiceColumns = [
   {
      title: "Id / InternalId",
      render: (_, record) => (
         <>
            <p style={{ margin: 0, display: "flex", flexDirection:"column", alignItems: "center" }}>
               <a target="_blank" href={record.publicUrl}>{record.uuid} /</a>
               <p style={{ margin: 0 }}>
                  <strong>{record.internalId}</strong>
               </p>
            </p>
         </>
      ),
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
   },
   {
      title: "Status",
      dataIndex: "status",
      render: (text) => <>{text}</>,
   },
];

export { InvoicesTableColumns, SubmittedInvoiceColumns };
