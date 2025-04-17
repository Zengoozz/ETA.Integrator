import React from "react";
import { Button, Form, Input, Flex } from "antd";
import AuthService from "../Services/AuthService";

const IssuerSettingsPage = ({ isMobile, onFinish }) => {
   const [form] = Form.useForm();

   const onSave = (values) => {
      AuthService.updateIssuerSettings(values, 2).then(() => {
         console.log("Issuer settings saved successfully");
         // Mark steps as completed in the backend
         //  AuthService.markStepsCompleted().then(() => {
         //     console.log("All steps completed");
         //     onFinish(); // Trigger navigation to the main app
         //  });
         onFinish(); // Trigger navigation to the main app
      });
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
            name="issuer-settings"
            form={form}
            layout={"vertical"}
            labelCol={{ span: isMobile ? 24 : 10 }}
            wrapperCol={{ span: isMobile ? 24 : 100 }}
            style={{ width: "100%" }}
            onFinish={onSave}
            onFinishFailed={onSaveFailed}
            requiredMark="optional"
         >
            <Form.Item
               label="Issuer Name"
               name="issuerName"
               rules={[{ required: true, message: "Please input the issuer name!" }]}
            >
               <Input size={isMobile ? "large" : "middle"} />
            </Form.Item>

            <Form.Item
               label="Tax ID"
               name="taxId"
               rules={[{ required: true, message: "Please input the tax ID!" }]}
            >
               <Input size={isMobile ? "large" : "middle"} />
            </Form.Item>

            <Form.Item>
               <Button
                  block
                  type="primary"
                  htmlType="submit"
                  size={isMobile ? "large" : "middle"}
               >
                  Finish Setup
               </Button>
            </Form.Item>
         </Form>
      </Flex>
   );
};

export default IssuerSettingsPage;
