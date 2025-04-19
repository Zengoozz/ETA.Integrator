import React from "react";
import { Layout, Typography, Space, Flex } from "antd";
import { SunOutlined, MoonOutlined, LogoutOutlined } from "@ant-design/icons";
import { Link } from "react-router-dom";

import "../assets/css/Navbar.css";

import AuthService from "../Services/AuthService";

const { Header } = Layout;
const { Title } = Typography;

const Navbar = ({ colorMode, setColorMode, isMarginedTop, isMobile, isLoggedIn }) => {
   const toggleColorMode = (color) => {
      setColorMode(color);
   };

   const onLogoutClick = () => {
      AuthService.logout();
      window.location.reload();
   };

   return (
      <Header
         style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            position: "sticky",
            top: 0,
            zIndex: 1,
            width: "100%",
            padding: isMobile ? "0 16px" : "0 24px",
            marginBottom: isMobile ? 10 : isMarginedTop ? 20 : 0,
            // background: '#001529', // Default AntD dark header
         }}
      >
         <Link to="/">
            <Title
               className="title-hover"
               level={3}
            >
               Global Soft
            </Title>
         </Link>

         <Flex
            gap="10px"
            align="center"
            justify="center"
         >
            {colorMode == "Dark" ? (
               <SunOutlined
                  className="regular-btn"
                  onClick={() => toggleColorMode("Light")}
               />
            ) : (
               <MoonOutlined
                  className="regular-btn"
                  onClick={() => toggleColorMode("Dark")}
               />
            )}
            {isLoggedIn && (
               <LogoutOutlined
                  className="small-btn"
                  onClick={() => onLogoutClick()}
               />
            )}
         </Flex>
      </Header>
   );
};

export default Navbar;
