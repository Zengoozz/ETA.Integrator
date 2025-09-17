import { useState } from "react";
import { Divider, Table, Flex } from "antd";
import { CloudDownloadOutlined } from "@ant-design/icons";

import CustomButton from "../Components/CustomButton";

const InvoicesTable = ({
   isMobile,
   tableData,
   messageApi,
   notificationApi,
   tableType,
   tableColumns,
   onSubmit = null,
   submissionCallBack = null,
   selectedRowsToAction = [],
   setSelectedRowsToActionOn = null,
}) => {
   const [loading, setLoading] = useState(false);
   // rowSelection object indicates the need for row selection
   const rowSelection = {
      onChange: (selectedRowKeys, selectedRows) => {
         console.info(
            `selectedRowKeys: ${selectedRowKeys}`,
            "selectedRows: ",
            selectedRows
         );
         
         setSelectedRowsToActionOn(selectedRows); // Update the selected rows state
      },
      getCheckboxProps: (record) => ({
         disabled: record.isReviewed === true, // Column configuration not to be checked
         name: record.invoiceNumber,
      }),
   };

   const handleSubmitButtonClick = () => {
      if (!selectedRowsToAction || selectedRowsToAction.length === 0) {
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
      onSubmit(selectedRowsToAction)
         .then((response) => {
            notificationApi.open({
               type: "success",
               message: (
                  <span dangerouslySetInnerHTML={{ __html: response.responseMessage.replace(/\n/g, "<br/>") }} />
               ),
               duration: 0,
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
            if(tableType == "W") 
               submissionCallBack(); // Clear selection after submission
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
