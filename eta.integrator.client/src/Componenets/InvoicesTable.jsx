import React, { useEffect, useState } from "react";
import { Divider, Table } from "antd";

import { InvoicesTableColumns } from "../Constants/InvoicesTableColumns";
import InvoicesService from "../Services/InvoicesService";

// const data = [
//     {
//         key: '5198',
//         receiptnumber: 5198,
//         visittype: 'OP',
//         company: 'Oscar Company-Ocsar Company-Class C',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/12/2025'),
//         status: false,
//     },
//     {
//         key: '5199',
//         receiptnumber: '5199',
//         visittype: 'OP',
//         company: 'Private Cash-cash.-نقدي',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/12/2025'),
//         status: true,
//     },
//     {
//         key: '5200',
//         receiptnumber: '5200',
//         visittype: 'INP',
//         company: 'Oscar Company-Ocsar Company-Class C',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/12/2025'),
//         status: false,
//     },
//     {
//         key: '5201',
//         receiptnumber: '5201',
//         visittype: 'INP',
//         company: 'Private Cash-cash.-نقدي',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/18/2025'),
//         status: true,
//     },
// ];

const InvoicesType = {
    InoviceNumber : String,
    InoviceType: String,
    VatNet: Number,
    NetPrice: Number,
    PatShare: Number,
    FinShare: Number,
    VatFinShare: Number,
    VatPatShare: Number,
    InvoiceId: Number,
    createdDate: Date,
    IsReviewed: Boolean,
    InovicingNumber: String,
    FinancialClassName: String
}

const biggerData = Array.from({ length: 46 }).map((_, i) => {
   return {
      key: i,
      receiptnumber: i,
      visittype: "OP",
      company: "Oscar Company-Ocsar Company-Class C",
      taxregisterationnumber: 1232312312,
      netprice: 87.5,
      patientshare: 17.5,
      financialshare: 70.0,
      vatnet: 0.0,
      date: new Date("03/12/2025"),
      status: i % 4 ? false : true,
   };
});

const InvoicesTable = () => {
   const [tableData, setTableData] = useState(biggerData);

   useEffect(() => {
      const fetchData = async () => {
         try {
            const response = await InvoicesService.getInvoices();
            console.log("Fetched settings:", response);

            const data = response.map((item) => {
               return {
                  key: item.InvoiceId,
                  receiptnumber: item.InoviceNumber,
                  visittype: item.InoviceType,
                  company: item.FinancialClassName,
                  taxregisterationnumber: item.InovicingNumber,
                  netprice: item.NetPrice,
                  patientshare: item.PatShare,
                  financialshare: item.FinShare,
                  vatnet: item.VatNet,
                  date: item.createdDate,
                  status: item.IsReviewed,
               };
            });

            // Update form table dynamically
            setTableData(data);
         } catch (err) {
            console.log("Failed to fetch invoices", err);
         }
      };

      fetchData();
   }, []);

   // rowSelection object indicates the need for row selection
   const rowSelection = {
      onChange: (selectedRowKeys, selectedRows) => {
         console.log(
            `selectedRowKeys: ${selectedRowKeys}`,
            "selectedRows: ",
            selectedRows
         );
         //TODO: State
      },
      getCheckboxProps: (record) => ({
         disabled: record.status === true, // Column configuration not to be checked
         name: record.receiptnumber,
      }),
   };

   return (
      <div style={{ padding: "0 1rem", overflowX: "auto" }}>
         <Divider />
         <Table
            bordered
            scroll={{ x: "max-content" }}
            rowSelection={Object.assign({ type: "checkbox" }, rowSelection)}
            columns={InvoicesTableColumns}
            dataSource={tableData}
            pagination={{ pageSize: 10, responsive: true }}
            //TODO:  onChange={handleTableChange}  // For sorting/filtering
         />
      </div>
   );
};
export default InvoicesTable;
