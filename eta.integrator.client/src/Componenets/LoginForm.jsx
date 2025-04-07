
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { Button, Form, Input } from 'antd';

const onFinish = values => {
  console.log('Success:', values);
};
const onFinishFailed = errorInfo => {
  console.log('Failed:', errorInfo);
};

const LoginForm = () => (
    <Form
    name="login"
    initialValues={{ remember: true }}
    style={{ maxWidth: 360 }}
    onFinish={onFinish}
    onFinishFailed={onFinishFailed}
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
);
export default LoginForm;