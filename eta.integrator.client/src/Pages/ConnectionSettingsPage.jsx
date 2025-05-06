import React, { useEffect } from "react";
import { Button, Form, Input, Flex } from "antd";

import AuthService from "../Services/AuthService";
import { SettingsValidationRules } from "../Constants/Constants.js";
//TODO: Add Second Client Secret
//TODO: Add The Token USB Pin Input
const ConnectionSettingsPage = ({ isMobile, setSuccessfulSave }) => {
   const [form] = Form.useForm();

   useEffect(() => {
      const fetchSettings = async () => {
         try {
            const response = await AuthService.getConnectionSettings();
            // Update form fields dynamically
            form.setFieldsValue({
               ClientId: response.ClientId,
               ClientSecret: response.ClientSecret,
               TokenPin: response.TokenPin
            });
         } catch (err) {
            console.log("Failed to fetch settings", err);
         }
      };

      fetchSettings();
   }, [form]);

   const onSave = async (values) => {
      var response = await AuthService.updateStep(values, 1);
      if (response == "UPDATED") {
         setSuccessfulSave(true);
      } else {
         setSuccessfulSave(false);
      }
      console.log("Response:", response);
   };

   const onSaveFailed = (errorInfo) => {
      console.log("Failed:", errorInfo);
   };

   return (
      <Flex
         align="center"
         justify="center"
         style={{
            // minHeight: "100%",
            width: "100%",
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
               label="Client Id"
               name="ClientId"
               rules={SettingsValidationRules.clientId}
            >
               <Input.Password
                  size={isMobile ? "large" : "middle"}
                  autoComplete="off"
               />
            </Form.Item>

            <Form.Item
               label="Client Secret"
               name="ClientSecret"
               rules={SettingsValidationRules.clientSecret}
            >
               <Input.Password
                  size={isMobile ? "large" : "middle"}
                  autoComplete="off"
               />
            </Form.Item>

            <Form.Item
               label="Token Pin"
               name="TokenPin"
               rules={SettingsValidationRules.tokenPin}
            >
               <Input.Password
                  size={isMobile ? "large" : "middle"}
                  autoComplete="off"
               />
            </Form.Item>

            <Form.Item>
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
      </Flex>
   );
};

export default ConnectionSettingsPage;
