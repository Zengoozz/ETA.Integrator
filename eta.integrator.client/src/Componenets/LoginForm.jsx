import React, { useCallback } from 'react';
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { Button, Form, Input } from 'antd';
import { useNavigate } from 'react-router-dom';


const LoginForm = ({ setLogIn }) => {
    const navigate = useNavigate();

    // eslint-disable-next-line no-unused-vars
    const onLogin = useCallback(async (values) => {
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


    }, [setLogIn, navigate])

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
            <LoginFields />
            <Form.Item>
                <Button block type="primary" htmlType="submit">
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
            rules={[{ required: true, message: 'Required' }]}
        >
            <Input prefix={<UserOutlined />} placeholder="Username" />
        </Form.Item>
        <Form.Item
            name="password"
            rules={[
                { required: true, message: 'Required' },
                { min: 6, message: 'Password too short' }
            ]}
        >
            <Input prefix={<LockOutlined />} type="password" placeholder="Password" />
        </Form.Item>
    </>
));