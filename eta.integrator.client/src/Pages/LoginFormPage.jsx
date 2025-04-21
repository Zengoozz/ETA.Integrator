import React from "react";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { Button, Form, Input, Flex } from "antd";

import { LoginFormValidationRules } from "../Constants/Constants";
import { useLogin } from "../Hooks/useLogin";

const LoginFormPage = ({ setLogIn, isMobile }) => {
   const { handleLogin, loading } = useLogin(setLogIn);

   const onLoginFailed = (errorInfo) => {
      console.log("Failed:", errorInfo);
   };

   return (
      <Flex
         align="center"
         justify="center"
         style={{ width: "100%" }}
      >
         <Form
            name="login"
            onFinish={handleLogin}
            onFinishFailed={onLoginFailed}
            style={{ width: "100%" }}
            layout={isMobile ? "vertical" : "horizontal"}
            wrapperCol={{ span: isMobile ? 24 : 100 }}
         >
            <Form.Item
               name="username"
               rules={LoginFormValidationRules.username}
            >
               <Input
                  prefix={<UserOutlined />}
                  size={isMobile ? "large" : "middle"}
                  placeholder="Username"
                  autoComplete="off"
               />
            </Form.Item>

            <Form.Item
               name="password"
               rules={LoginFormValidationRules.password}
            >
               <Input.Password
                  prefix={<LockOutlined />}
                  size={isMobile ? "large" : "middle"}
                  placeholder="Password"
               />
            </Form.Item>

            <Form.Item>
               <Button
                  block
                  type="primary"
                  htmlType="submit"
                  loading={loading}
                  size={isMobile ? "large" : "middle"}
               >
                  Log in
               </Button>
            </Form.Item>
         </Form>
      </Flex>
   );
};

export default LoginFormPage;
