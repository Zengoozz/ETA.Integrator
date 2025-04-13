import React, { useEffect } from "react";
import { Button, Form, Input, Flex } from "antd";
import AuthService from "../Services/AuthService";

const SettingsPage = ({ isMobile }) => {
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
      var response = AuthService.saveSettings(values);
      console.log("Response:", response);
      // console.log("Saved:", values);
   };

   const onSaveFailed = (errorInfo) => {
      console.log("Failed:", errorInfo);
   };

   return (
      <Flex
         align="center"
         justify="center"
         style={{
            minHeight: "100%",
            // padding: isMobile ? "16px" : "24px",
         }}
      >
            <Form
               name="settings"
               form={form}
               layout={"vertical"}
               labelCol={{ span: isMobile ? 24 : 10 }}
               wrapperCol={{ span: isMobile ? 24 : 100 }}
               style={{ width: "100%" }}
               initialValues={{ remember: true }}
               onFinish={onSave}
               onFinishFailed={onSaveFailed}
               requiredMark="optional"
            >
               <Form.Item
                  label="Connection String"
                  name="connectionString"
                  rules={[
                     { required: true, message: "Please input your connection string!" },
                  ]}
               >
                  <Input size={isMobile ? "large" : "middle"} />
               </Form.Item>

               <Form.Item
                  label="Client Id"
                  name="clientId"
                  rules={[{ required: true, message: "Please input your client id!" }]}
               >
                  <Input.Password size={isMobile ? "large" : "middle"} />
               </Form.Item>

               <Form.Item
                  label="Client Secret"
                  name="clientSecret"
                  rules={[
                     { required: true, message: "Please input your client secret!" },
                  ]}
               >
                  <Input.Password size={isMobile ? "large" : "middle"} />
               </Form.Item>

               <Form.Item 
                  // wrapperCol={{ offset: isMobile ? 0 : 8, span: 24 }}
               >
                  <Button
                     block
                     type="primary"
                     htmlType="submit"
                     size={isMobile ? "large" : "middle"}
                  >
                     Save
                  </Button>
               </Form.Item>
            </Form>
         {/* </Card> */}
      </Flex>
   );
};

export default SettingsPage;
