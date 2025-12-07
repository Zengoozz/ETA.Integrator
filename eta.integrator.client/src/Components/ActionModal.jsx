import { Form, Input, Button, Modal } from "antd";

import { EditInvoiceRules } from "../Constants/Constants";

const ActionModal = ({ title, isModalOpen, handleOk, handleCancel, data, isMobile }) => {
   const [form] = Form.useForm();

   var finalTitle = data != null ? `${title} For No. ${data.invoiceNumber}` : title;
   return (
      <>
         {" "}
         <Modal
            title={finalTitle}
            open={isModalOpen}
            onCancel={handleCancel}
            footer={null}
         >
            <Form
               form={form}
               layout="vertical"
               name="editSubmitForm"
               labelCol={{ span: isMobile ? 24 : 10 }}
               wrapperCol={{ span: isMobile ? 24 : 100 }}
               style={{ width: "100%" }}
               onFinish={handleOk}
               requiredMark="optional"
            >
               <Form.Item
                  label="Receiver Name"
                  name="ReceiverName"
                  rules={EditInvoiceRules.receiverName}
               >
                  <Input
                     size={isMobile ? "large" : "middle"}
                     autoComplete="off"
                     allowClear
                  />
               </Form.Item>

               <Form.Item
                  label="Registration Number"
                  name="RegistrationNumber"
                  rules={EditInvoiceRules.registrationNumber}
               >
                  <Input
                     length={12}
                     size={isMobile ? "large" : "middle"}
                     autoComplete="off"
                     allowClear
                  />
               </Form.Item>
               <Form.Item>
                  <Button
                     block
                     type="primary"
                     htmlType="submit"
                     size={isMobile ? "large" : "middle"}
                  >
                     Submit
                  </Button>
               </Form.Item>
            </Form>
         </Modal>
      </>
   );
};
export default ActionModal;
