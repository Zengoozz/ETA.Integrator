import React, { useState } from "react";
import { Divider, Table, Flex } from "antd";
import { CloudDownloadOutlined } from "@ant-design/icons";

import CustomButton from "../Components/CustomButton";

const InvoicesTable = ({
   isMobile,
   tableData,
   onSubmit = null,
   messageApi,
   notificationApi,
   tableType,
   tableColumns,
}) => {
   const [selectedRowToSubmit, setSelectedRowToSubmit] = useState([]);
   const [loading, setLoading] = useState(false);
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

      const loadingMessage = messageApi.open({
         type: "loading",
         content: "Action in progress..",
         duration: 0,
      });

      // Start loading
      setLoading(true);
      onSubmit(selectedRowToSubmit)
         .then(() => {
            messageApi.open({
               type: "success",
               content: "Selected rows saved successfully!",
               duration: 2,
            });
         })
         .catch((error) => {
            notificationApi.error({
               message: error.detail,
               duration: 0,
            });
            console.error(error.message);
         })
         .finally(() => {
            loadingMessage(); // Close the loading message
            setLoading(false); // End loading
         });
   };

   return (
      <Flex
         vertical
         style={{ overflowX: "auto", width: "100%" }}
      >
         {tableType == "W" && (
            <CustomButton
               icon={<CloudDownloadOutlined />}
               loading={loading}
               handleClick={handleSubmitButtonClick}
               style={{ alignSelf: "flex-end" }}
            />
         )}

         <Divider />
         <Table
            rowSelection={
               tableType == "W"
                  ? Object.assign({ type: "checkbox" }, rowSelection)
                  : undefined
            }
            dataSource={tableData}
            bordered
            scroll={{ x: isMobile ? "max-content" : "unset" }}
            columns={tableColumns.map((col) => ({
               ...col,
               ellipsis: true,
               width: isMobile ? 150 : "auto",
               responsive: ["md"],
            }))}
            rowKey={(record) => record.invoiceNumber}
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
