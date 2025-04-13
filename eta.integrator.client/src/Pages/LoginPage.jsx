import React from 'react';
import { Layout, theme, Flex } from 'antd';
import { Outlet } from 'react-router-dom';

const { Content } = Layout;

const LoginPage = ({isMobile}) => {

  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  return (
    <Layout
      style={{ minHeight: '100vh' }}
    >
      <Content
        style={{
          padding: isMobile ? '16px' : '48px',
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
        }}
      >
        <Flex
               vertical
               align="center"
               justify="center"
               style={{
                  background: colorBgContainer,
                  borderRadius: borderRadiusLG,
                  padding: isMobile ? '24px' : '36px',
                  width: isMobile ? '100%' : '400px',
                  maxWidth: '100%',
                  boxShadow: '0 4px 12px rgba(0, 0, 0, 0.1)',
               }}
            >
          <Outlet />
        </Flex>
      </Content>
    </Layout>
  );
};
export default LoginPage;