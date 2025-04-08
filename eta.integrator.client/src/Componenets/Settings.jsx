import React from 'react'
import { Button, Form, Input } from 'antd';

const Settings = () => {

  const onSave = values => {
    console.log('Saved:', values);
  };

  const onSaveFailed = errorInfo => {
    console.log('Failed:', errorInfo);
  };

  return (
    <Form
      layout="vertical"
      labelCol={{ span: 10 }}
      // wrapperCol={{ span: 20 }}
      name="settings"
      initialValues={{ remember: true }}
      style={{ minWidth: 360 }}
      onFinish={onSave}
      onFinishFailed={onSaveFailed}
    >
      <Form.Item
        label="Connection String"
        name="connectionString"
        rules={[{ required: true, message: 'Please input your connection string!' }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        label="Client Id"
        name="clientId"
        rules={[{ required: true, message: 'Please input your client id!' }]}
      >
        <Input type="password" />
      </Form.Item>

      <Form.Item
        label="Client Secret"
        name="clientSecret"
        rules={[{ required: true, message: 'Please input your client secret!' }]}
      >
        <Input type="password" />
      </Form.Item>

      <Form.Item>
        <Button block type="primary" htmlType="submit">
          Save
        </Button>
      </Form.Item>
    </Form>
  );
}

export default Settings