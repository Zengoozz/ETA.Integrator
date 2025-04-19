import React from "react";
import { Layout, theme, Flex } from "antd";
import { Outlet } from "react-router-dom";

import Navbar from "./NavBar";

const { Content } = Layout;

const DefaultLayout = ({
   mode,
   setMode,
   maxWidthValue,
   contentStyle,
   isMarginedTop,
   isMobile,
   isLoggedIn
}) => {
   const {
      token: { colorBgContainer, borderRadiusLG },
   } = theme.useToken();

   return (
      <Layout style={{ minHeight: "100vh" }}>
         <Navbar
            colorMode={mode}
            setColorMode={setMode}
            isMarginedTop={isMarginedTop}
            isMobile={isMobile}
            isLoggedIn={isLoggedIn}
         />
         <Content
            style={{
               ...contentStyle,
               padding: isMobile ? "0 16px" : "0 48px",
               maxWidth: "100%",
            }}
         >
            <Flex
               vertical
               gap="middle"
               style={{
                  background: colorBgContainer,
                  borderRadius: borderRadiusLG,
                  padding: isMobile ? 16 : 24, // Responsive padding
                  width: "100%",
                  maxWidth: maxWidthValue, // Original prop preserved
                  boxShadow: "0 4px 12px rgba(0, 0, 0, 0.1)",
                  minHeight: "100%", // Changed from height to minHeight
                  margin: "0 auto", // Center content
               }}
            >
               <Outlet />
            </Flex>
         </Content>
      </Layout>
   );
};
export default DefaultLayout;
