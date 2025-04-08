import React from 'react';
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { Button, Form, Input } from 'antd';
// import { useNavigate } from 'react-router-dom';

import { LoginFormValidationRules } from '../Constants/FormRules';
import { useLoginForm } from '../Hooks/useLoginForm';

const LoginForm = ({ setLogIn }) => {
    const { handleLogin, loading } = useLoginForm(setLogIn);
    // const navigate = useNavigate();


    const onLoginFailed = errorInfo => {
        console.log('Failed:', errorInfo);
    };

    return (
        <Form
            name="login"
            initialValues={{ remember: true }}
            style={{ maxWidth: 360 }}
            onFinish={handleLogin}
            onFinishFailed={onLoginFailed}
        >
            <LoginFields />
            <Form.Item>
                <Button block type="primary" htmlType="submit" loading={loading}>
                    Log in
                </Button>
            </Form.Item>
        </Form>
    )
};

export default React.memo(LoginForm);


const LoginFields = React.memo(() => (
    <>
        <Form.Item
            name="username"
            rules={LoginFormValidationRules.username}
        >
            <Input prefix={<UserOutlined />} placeholder="Username" />
        </Form.Item>
        <Form.Item
            name="password"
            rules={LoginFormValidationRules.password}
        >
            <Input prefix={<LockOutlined />} type="password" placeholder="Password" />
        </Form.Item>
    </>
));