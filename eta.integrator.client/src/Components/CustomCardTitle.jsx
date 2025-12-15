import React from "react";
import { Space, Typography } from "antd";

const { Title } = Typography;

const CustomCardTitle = ({ title, icon }) => {
   const iconStyle = {
      fontSize: "24px",
      color: "#1890ff",
   };

   return (
      <Space
         size="middle"
         align="center"
         style={{ width: "100%", padding: "8px 0" }}
      >
         {icon && React.cloneElement(icon, { style: iconStyle })}

         <Title
            level={3}
            style={{ margin: 0, marginBottom: "2px" }}
         >
            {title}
         </Title>
      </Space>
   );
};

export default CustomCardTitle;
