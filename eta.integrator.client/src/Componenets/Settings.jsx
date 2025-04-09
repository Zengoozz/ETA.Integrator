import React, { useEffect } from "react";
import { Button, Form, Input } from "antd";
import AuthService from "../Services/AuthService";

const Settings = () => {
   const [form] = Form.useForm();

   useEffect(() => {
      const fetchSettings = async () => {
         try {
            const response = await AuthService.getSettings();
            console.log("Fetched settings:", response);

            // Update form fields dynamically
            form.setFieldsValue({
               connectionString: response.connectionString,
               clientId: response.clientId,
               clientSecret: response.clientSecret,
            });
         } catch (err) {
            console.log("Failed to fetch settings", err);
         }
      };

      fetchSettings();
   }, [form]);

   const onSave = (values) => {
      console.log("Saved:", values);
   };

   const onSaveFailed = (errorInfo) => {
      console.log("Failed:", errorInfo);
   };

   return (
      <Form
         form={form}
         layout="vertical"
         labelCol={{ span: 10 }}
         // wrapperCol={{ span: 20 }}
         name="settings"
         initialValues={{ remember: true }}
         style={{ minWidth: 360 }}
         onFinish={onSave}
         onFinishFailed={onSaveFailed}
      >
         <Form.Item
            label="Connection String"
            name="connectionString"
            rules={[{ required: true, message: "Please input your connection string!" }]}
         >
            <Input />
         </Form.Item>

         <Form.Item
            label="Client Id"
            name="clientId"
            rules={[{ required: true, message: "Please input your client id!" }]}
         >
            <Input.Password />
         </Form.Item>

         <Form.Item
            label="Client Secret"
            name="clientSecret"
            rules={[{ required: true, message: "Please input your client secret!" }]}
         >
            <Input.Password />
         </Form.Item>

         <Form.Item>
            <Button
               block
               type="primary"
               htmlType="submit"
            >
               Save
            </Button>
         </Form.Item>
      </Form>
   );
};

export default Settings;
