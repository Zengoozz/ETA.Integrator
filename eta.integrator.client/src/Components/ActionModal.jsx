import { useEffect, useState } from "react";
import { Form, Input, Button, Modal } from "antd";

import { EditInvoiceRules } from "../Constants/Constants";

const ActionModal = ({
   title,
   isModalOpen,
   handleOk,
   handleCancel,
   data,
   isMobile,
   notificationApi,
   callback,
}) => {
   const [finalTitle, setFinalTitle] = useState(title);  
   const [isLoading, setIsLoading] = useState(false);
   const [form] = Form.useForm();

   useEffect(() => {
      if(data){
         form.setFieldsValue({
            ReceiverName: data.receiverName,
            RegistrationNumber: data.registrationNumber,
         });

         setFinalTitle(`${title} For No. ${data.invoiceNumber}`);
      }
   }, [data, form, title]);

   const onOkClick = () => {
      setIsLoading(true); // Start loading

      form.validateFields().then((values) => {
         if (isNaN(values.RegistrationNumber)) {
            notificationApi.error({
               message: "Registeration Number must be numeric.",
               duration: 0,
            });
            setIsLoading(false);
            return;
         }

         return handleOk(values)
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

               onCancelClick();

            })
            .catch((error) => {
               notificationApi.error({
                  message: error.detail,
                  duration: 0,
               });
               console.error(error.message);
            })
            .finally(async () => {
               await callback();
               setIsLoading(false);
            });
      });
   };

   const onCancelClick = () => {
      form.resetFields();
      handleCancel();
   }

   return (
      <>
         {" "}
         <Modal
            title={finalTitle}
            open={isModalOpen}
            onCancel={onCancelClick}
            footer={null}
         >
            <Form
               form={form}
               layout="vertical"
               name="editSubmitForm"
               labelCol={{ span: isMobile ? 24 : 10 }}
               wrapperCol={{ span: isMobile ? 24 : 100 }}
               style={{ width: "100%" }}
               onFinish={onOkClick}
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
                     loading={isLoading}
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
