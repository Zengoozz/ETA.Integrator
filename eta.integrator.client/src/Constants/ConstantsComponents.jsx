import { CheckCircleTwoTone, CloseCircleTwoTone } from "@ant-design/icons";
import dayjs from "dayjs";

const InvoicesTableColumns = [
   {
      title: "Receipt Number",
      dataIndex: "invoiceNumber",
      render: (text) => <a>{text}</a>,
   },
   {
      title: "Visit Type",
      dataIndex: "invoiceType",
      render: (text) => text,
   },
   {
      title: "Company",
      dataIndex: "receiverName",
      render: (text) => text,
   },
   {
      title: "Tax Registeration Number",
      dataIndex: "registrationNumber",
      render: (text) => text,
   },
   {
      title: "Net Price",
      dataIndex: "netPrice",
      render: (value) => value.toFixed(2),
   },
   {
      title: "Patient Share",
      dataIndex: "patShare",
      render: (value) => value.toFixed(2),
   },
   {
      title: "Financial Share",
      dataIndex: "finShare",
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
];

const SubmittedInvoiceColumns = [
   {
      title: "Id / InternalId",
      // dataIndex: "internalId",
      render: (_, record) => (
         <>
            <a>
               <span>{record.uuid}</span> / <span>{record.internalId}</span>
            </a>
         </>
      ),
   },
   {
      title: "Date Time Received",
      dataIndex: "dateTimeReceived",
      render: (text) => <>{text}</>,
   },
   {
      title: "Total Value",
      dataIndex: "total",
      render: (text) => <>{text}</>,
   },
   {
      title: "Issuer",
      dataIndex: "issuerName",
      render: (text) => <>{text}</>,
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
