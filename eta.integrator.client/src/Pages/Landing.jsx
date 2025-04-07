import React from 'react';
import { Layout, theme } from 'antd';

import LoginForm from "../Componenets/LoginForm"
import Navbar from '../Componenets/NavBar';

const { Content } = Layout;

const Landing = ({ colorMode, setColorMode }) => {

  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  return (
    <Layout
      style={{ minHeight: '100vh' }}
    >
      <Navbar colorMode={colorMode} setColorMode={setColorMode} />
      <Content
        style={{
          padding: '0 48px',
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
        }}
      >
        <div
          style={{
            background: colorBgContainer,
            borderRadius: borderRadiusLG,
            padding: 24,
            width: '100%',
            maxWidth: '400px', // Optional: limits form width
            boxShadow: '0 4px 12px rgba(0, 0, 0, 0.1)',
          }}
        >
          <LoginForm />
        </div>
      </Content>
    </Layout>
  );
};
export default Landing;