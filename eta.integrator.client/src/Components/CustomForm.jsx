import { useState, useEffect } from "react";
import { Form, Button } from "antd";

const CustomForm = ({
   name = "Custom Form",
   isMobile,
   notificationApi,
   initialValues = null,
   formItems,
   buttonName = "Submit",
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
         {formItems.map((item, index) => (
            <Form.Item
               key={index}
               label={item.label}
               name={item.name}
               rules={item.rules}
            >
               {item.element}
            </Form.Item>
         ))}

         <Form.Item>
            <Button
               block
               type="primary"
               htmlType="submit"
               size={isMobile ? "large" : "middle"}
               loading={isLoading}
            >
               {buttonName}
            </Button>
         </Form.Item>
      </Form>
   );
};

export default CustomForm;
