import React, { useMemo } from "react";
import { Divider, Table } from "antd";

import { InvoicesTableColumns } from "../Constants/InvoicesTableColumns";

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
   const data = useMemo(() => biggerData, []);

   // rowSelection object indicates the need for row selection
   const rowSelection = useMemo(
      () => ({
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
      }),
      []
   );

   return (
      <div>
         <Divider />
         <Table
            bordered
            rowSelection={Object.assign({ type: "checkbox" }, rowSelection)}
            columns={InvoicesTableColumns}
            dataSource={data}
            pagination={{ pageSize: 10 }}
            //TODO:  onChange={handleTableChange}  // For sorting/filtering
         />
      </div>
   );
};
export default React.memo(InvoicesTable);
