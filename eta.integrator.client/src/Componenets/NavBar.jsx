import React from "react";
import { Layout, Typography, Space } from "antd";
import { SunOutlined, MoonOutlined, SettingOutlined } from "@ant-design/icons";
import { Link } from "react-router-dom";

const { Header } = Layout;
const { Title } = Typography;

const Navbar = ({ colorMode, setColorMode, isMarginedTop }) => {
   const toggleColorMode = (color) => {
      setColorMode(color);
   };

   return (
      <Header
         style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            padding: "0 24px",
            position: "sticky",
            top: 0,
            zIndex: 1,
            width: "100%",
            marginBottom: isMarginedTop ? 20 : 0,
            // background: '#001529', // Default AntD dark header
         }}
      >
         <Link to="/">
            <Title
               level={3}
               style={{ color: "#fff", margin: 0 }}
            >
               Global Soft
            </Title>
         </Link>

         <span style={{ display: "flex", gap: 20 }}>
            <Link to="/settings">
               <SettingOutlined
                  style={{ fontSize: "20px", color: "#fff", cursor: "pointer" }}
               />
            </Link>
            <Space>
               {colorMode == "Dark" ? (
                  <SunOutlined
                     onClick={() => toggleColorMode("Light")}
                     style={{ fontSize: "20px", color: "#fff", cursor: "pointer" }}
                  />
               ) : (
                  <MoonOutlined
                     onClick={() => toggleColorMode("Dark")}
                     style={{ fontSize: "20px", color: "#fff", cursor: "pointer" }}
                  />
               )}
            </Space>
         </span>
      </Header>
   );
};

export default Navbar;
