import { useState, useEffect } from "react";
import { Form, Input, Button } from "antd";

import { EditInvoiceRules } from "../Constants/Constants";

const CustomForm = ({
   name = "Custom Form",
   isMobile,
   notificationApi,
   initialValues = null,
   handleSubmit,
   handleSubmitCallback = null,
   handleSubmitFailure = null,
   handleFormValidation = null,
}) => {
   const [isLoading, setIsLoading] = useState(false);
   const [form] = Form.useForm();

   useEffect(() => {
      if (initialValues) {
         form.setFieldsValue(initialValues);
      }
   }, [form, initialValues]);

   const onSubmitClick = () => {
      setIsLoading(true); // Start loading

      form.validateFields().then(async (values) => {
         var isValid = true;
         if (handleFormValidation) isValid = handleFormValidation(values);

         if (!isValid) return setIsLoading(false);

         try {
            await handleSubmit(values);
            if (handleSubmitCallback) await handleSubmitCallback();
         } catch (error) {
            notificationApi.error({
               message: error.detail,
               duration: 0,
            });
         } finally {
            setIsLoading(false);
         }

         // .then(
         //    handleSubmitCallback()
         //     (response) => {
         //    notificationApi.open({
         //       type: "success",
         //       message: (
         //          <span
         //             dangerouslySetInnerHTML={{
         //                __html: response.responseMessage.replace(/\n/g, "<br/>"),
         //             }}
         //          />
         //       ),
         //       duration: 0,
         //    });

         //    onCancelClick();

         // }
         // )
         // .catch((error) => {
         //    notificationApi.error({
         //       message: error.detail,
         //       duration: 0,
         //    });
         //    console.error(error.message);
         // })
         // .finally(async () => {
         //    //    await callback();
         //    setIsLoading(false);
         // });
      });
   };

   return (
      <Form
         form={form}
         layout="vertical"
         name={name}
         labelCol={{ span: isMobile ? 24 : 10 }}
         wrapperCol={{ span: isMobile ? 24 : 100 }}
         style={{ width: "100%" }}
         onFinish={onSubmitClick}
         onFinishFailed={handleSubmitFailure}
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
   );
};

export default CustomForm;
