import React, { useEffect } from "react";
import { Button, Form, Input, Flex, Select } from "antd";

import AddressForm from "../Components/AddressForm";

import AuthService from "../Services/AuthService";
import { IssuerTypes, SettingsValidationRules } from "../Constants/Constants";

const { Option } = Select;

const IssuerSettingsPage = ({ isMobile, onFinish }) => {
   const [form] = Form.useForm();
   const [isBusinessType, setIsBusinessType] = React.useState(false);

   useEffect(() => {
      const fetchSettings = async () => {
         try {
            const response = await AuthService.getIssuerSettings();
            // Update form fields dynamically
            form.setFieldsValue({
               IssuerName: response.IssuerName,
               RegistrationNumber: response.RegistrationNumber,
               IssuerType: response.IssuerType,
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

   const onIssuerTypeChange = (value) => {
      if (value === "B") {
         setIsBusinessType(true);
      } else {
         setIsBusinessType(false);
      }
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
            initialValues={{
               Address: {
                  Country: "EG", // Default country
                  Governorate: "cairo", // Default governorate
                  Region: "cairo", // Default region
               },
            }}
         >
            <Form.Item
               label="Issuer Type"
               name="IssuerType"
               rules={SettingsValidationRules.issuerType}
            >
               <Select
                  placeholder="Please select a type"
                  onChange={onIssuerTypeChange}
               >
                  {IssuerTypes.map((type) => (
                     <Select.Option
                        key={type.value}
                        value={type.value}
                     >
                        {type.label}
                     </Select.Option>
                  ))}
               </Select>
            </Form.Item>

            <Form.Item
               label="Issuer Name"
               name="IssuerName"
               rules={SettingsValidationRules.issuerName}
            >
               <Input
                  size={isMobile ? "large" : "middle"}
                  autoComplete="off"
               />
            </Form.Item>

            <Form.Item
               label="Registration Number"
               name="RegistrationNumber"
               rules={[
                  { required: true, message: "Please input the registration number!" },
                  { whitespace: true, message: "Username cannot be empty spaces" },
               ]}
            >
               <Input.OTP
                  formatter={(value) => value.replace(/\D/g, "")}
                  length={12}
                  size={isMobile ? "large" : "middle"}
                  autoComplete="off"
               />
            </Form.Item>

            <AddressForm
               isBusinessType={isBusinessType}
               form={form}
            />

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

export default IssuerSettingsPage;
