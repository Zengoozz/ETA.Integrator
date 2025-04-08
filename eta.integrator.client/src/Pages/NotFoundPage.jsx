import React from 'react';
import { Button, Result, theme  } from 'antd';
import { useNavigate } from 'react-router-dom';

const NotFoundPage = () => {
  const navigate = useNavigate();

  const { token } = theme.useToken();

  return (
    <div
      style={{
        display: 'flex',
        height: '100vh',
        alignItems: 'center',
        justifyContent: 'center',
        background: token.colorBgLayout,
        padding: '24px',
      }}
    >
      <Result
        status="404"
        title="404"
        subTitle="Sorry, the page you visited does not exist."
        extra={
          <Button type="primary" onClick={() => navigate('/')}>
            Back Home
          </Button>
        }
        style={{
          background: token.colorBgContainer,
          padding: '40px',
          borderRadius: token.borderRadiusLG,
          boxShadow: token.boxShadowSecondary,
          color: token.colorText,
        }}
      />
    </div>
  );
};

export default NotFoundPage;