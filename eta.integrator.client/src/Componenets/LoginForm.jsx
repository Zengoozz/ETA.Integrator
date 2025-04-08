import React from 'react';
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { Button, Form, Input } from 'antd';
import { useNavigate } from 'react-router-dom';




const LoginForm = ({ setLogIn }) => {
    const navigate = useNavigate();

    // eslint-disable-next-line no-unused-vars
    const onLogin = async (values) => {
        setLogIn(true);
        navigate('/home');
        // console.log(values);

        // const response = await fetch('HMS/Login', {
        //     method: 'POST',
        //     headers: {
        //         'Content-Type': 'application/json'
        //     },
        //     body: JSON.stringify({
        //         Username: values.username,
        //         Password: values.password
        //     })
        // });
        // const data = await response.json();
        // console.log(data);


    }

    const onLoginFailed = errorInfo => {
        console.log('Failed:', errorInfo);
    };

    return (
        <Form
            name="login"
            initialValues={{ remember: true }}
            style={{ maxWidth: 360 }}
            onFinish={onLogin}
            onFinishFailed={onLoginFailed}
        >
            <Form.Item
                name="username"
                rules={[{ required: true, message: 'Please input your Username!' }]}
            >
                <Input prefix={<UserOutlined />} placeholder="Username" />
            </Form.Item>

            <Form.Item
                name="password"
                rules={[{ required: true, message: 'Please input your Password!' }]}
            >
                <Input prefix={<LockOutlined />} type="password" placeholder="Password" />
            </Form.Item>

            <Form.Item>
                <Button block type="primary" htmlType="submit">
                    Log in
                </Button>
            </Form.Item>
        </Form>
    )
};

export default LoginForm;