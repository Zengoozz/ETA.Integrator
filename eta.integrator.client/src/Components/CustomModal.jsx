import { Modal } from "antd";

const CustomModal = ({
   hasTitle = true,
   title = "Default Title",
   hasFooter = null,
   isModalOpen = false,
   handleOk,
   handleCancel,
   children,
}) => {
  
   return (
      <Modal
         title={hasTitle ? title : ""}
         open={isModalOpen}
         footer={hasFooter}
         onOk={hasFooter != null ? handleOk : null}
         onCancel={handleCancel}
      >
         {children}
      </Modal>
   );
};
export default CustomModal;
