import React from "react";
import { Button, Result, theme, Flex } from "antd";
import { useNavigate } from "react-router-dom";

const NotFoundPage = ({ isMobile }) => {
   const navigate = useNavigate();

   const { token } = theme.useToken();

   return (
      <Flex
         align="center"
         justify="center"
         style={{
            minHeight: "100vh",
            background: token.colorBgLayout,
            padding: isMobile ? "24px" : "48px",
         }}
      >
         <Result
            status="404"
            title={<span style={{ fontSize: isMobile ? 24 : 36 }}>404</span>}
            subTitle="Sorry, the page you visited does not exist."
            extra={
               <Button
                  type="primary"
                  onClick={() => navigate("/")}
                  size={isMobile ? 'large' : 'middle'}
                  block={isMobile} 
               >
                  Back Home
               </Button>
            }
            style={{
               background: token.colorBgContainer,
               padding: "40px",
               borderRadius: token.borderRadiusLG,
               boxShadow: token.boxShadowSecondary,
               color: token.colorText,
            }}
         />
      </Flex>
   );
};

export default NotFoundPage;
