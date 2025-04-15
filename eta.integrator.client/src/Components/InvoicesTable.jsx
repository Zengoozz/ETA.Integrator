import React from "react";
import { Divider, Table, Flex } from "antd";
import { CloudDownloadOutlined } from "@ant-design/icons";

import { InvoicesTableColumns } from "../Constants/Constants";
import CustomButton from "../Components/CustomButton";

const InvoicesTable = ({ isMobile, tableData, onSubmit, loading, messageApi }) => {
   const [selectedRowToSubmit, setSelectedRowToSubmit] = React.useState([]);
   // rowSelection object indicates the need for row selection
   const rowSelection = {
      onChange: (selectedRowKeys, selectedRows) => {
         console.log(
            `selectedRowKeys: ${selectedRowKeys}`,
            "selectedRows: ",
            selectedRows
         );
         setSelectedRowToSubmit(selectedRows); // Update the selected rows state
      },
      getCheckboxProps: (record) => ({
         disabled: record.status === true, // Column configuration not to be checked
         name: record.receiptnumber,
      }),
   };

   const handleSubmitButtonClick = () => {
      if (!selectedRowToSubmit || selectedRowToSubmit.length === 0) {
         console.warn("No rows selected.");
         messageApi.warning("Please select at least one row to submit.");
         return;
      }
      onSubmit(selectedRowToSubmit); // Pass the selected rows to the parent
   };

   return (
      <Flex
         vertical
         style={{ overflowX: "auto", width: "100%" }}
      >
         <CustomButton
            icon={<CloudDownloadOutlined />}
            loading={loading}
            handleClick={handleSubmitButtonClick}
            style={{ alignSelf: "flex-end" }}
         />
         <Divider />
         <Table
            rowSelection={Object.assign({ type: "checkbox" }, rowSelection)}
            dataSource={tableData}
            bordered
            scroll={{ x: isMobile ? "max-content" : "unset" }}
            columns={InvoicesTableColumns.map((col) => ({
               ...col,
               ellipsis: true,
               width: isMobile ? 150 : "auto",
               responsive: ["md"],
            }))}
            rowKey={(record) => record.inoviceNumber}
            pagination={{
               pageSize: isMobile ? 5 : 10,
               responsive: true,
               position: ["bottomCenter"],
            }}
            //TODO:  onChange={handleTableChange}  // For sorting/filtering
         />
      </Flex>
   );
};
export default InvoicesTable;
