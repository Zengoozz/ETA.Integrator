import React, { useEffect } from "react";
import { Button, Form, Input, Flex } from "antd";
import AuthService from "../Services/AuthService";

const IssuerSettingsPage = ({ isMobile, onFinish }) => {
   const [form] = Form.useForm();

   useEffect(() => {
      const fetchSettings = async () => {
         try {
            const response = await AuthService.getIssuerSettings();
            // Update form fields dynamically
            form.setFieldsValue({
               IssuerName: response.IssuerName,
               TaxId: response.TaxId,
            });
         } catch (err) {
            console.log("Failed to fetch settings", err);
         }
      };
      fetchSettings();
   }, [form]);

   const onSave = (values) => {
      AuthService.updateStep(values, 2).then(() => {
         console.log("Issuer settings saved successfully");
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
               name="IssuerName"
               rules={[{ required: true, message: "Please input the issuer name!" }]}
            >
               <Input size={isMobile ? "large" : "middle"} />
            </Form.Item>

            <Form.Item
               label="Tax ID"
               name="TaxId"
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
