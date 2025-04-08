import React from "react";
import { Layout, theme } from "antd";
import { Outlet } from "react-router-dom";

import Navbar from "../Componenets/NavBar";

const { Content } = Layout;

const DefaultPage = ({ mode, setMode, maxWidthValue, contentStyle, isMarginedTop }) => {
   const {
      token: { colorBgContainer, borderRadiusLG },
   } = theme.useToken();

   return (
      <Layout style={{ minHeight: "100vh" }}>
         <Navbar
            colorMode={mode}
            setColorMode={setMode}
            isMarginedTop={isMarginedTop}
         />
         <Content style={contentStyle}>
            <div
               style={{
                  background: colorBgContainer,
                  borderRadius: borderRadiusLG,
                  padding: 24,
                  width: "100%",
                  maxWidth: maxWidthValue, // Optional: limits form width
                  boxShadow: "0 4px 12px rgba(0, 0, 0, 0.1)",
                  height: "100%",
               }}
            >
               <Outlet />
            </div>
         </Content>
      </Layout>
   );
};
export default DefaultPage;
