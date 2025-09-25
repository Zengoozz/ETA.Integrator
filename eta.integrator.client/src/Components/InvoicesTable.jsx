import { useEffect, useState } from "react";
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
   isSubmittedInvoicesTable = false,
   buttonName = "Submit",
   onSubmit = null,
   submissionCallBack = null,
}) => {
   const [loading, setLoading] = useState(false);
   const [selectedRowsToAction, setSelectedRowsToActionOn] = useState([]);
   // rowSelection object indicates the need for row selection
   useEffect(() => {}, []);

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
         disabled: isSubmittedInvoicesTable ? record.status === "Valid" : record.isReviewed === true, // Column configuration not to be checked
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
                  <span
                     dangerouslySetInnerHTML={{
                        __html: response.responseMessage.replace(/\n/g, "<br/>"),
                     }}
                  />
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
         .finally(async () => {
            if (tableType == "W") await submissionCallBack();
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
               name={buttonName}
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
            rowKey={(record) => isSubmittedInvoicesTable ? record.longId : record.invoiceNumber}
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
