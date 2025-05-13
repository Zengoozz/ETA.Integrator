import { Spin } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import { theme } from "antd";

import "../assets/css/Loading.css";

const antIcon = (
   <LoadingOutlined
      style={{ fontSize: 48 }}
      spin
   />
);

// TODO: Observe behavior
// TODO: Inherit theme for the main loading screen
const Loading = ({ message = "Loading...", size = "large" }) => {
   const {
      token: { colorBgContainer },
   } = theme.useToken();

   return (
      <div
         className="loading-container"
         style={{ background: colorBgContainer }}
      >
         <Spin
            indicator={antIcon}
            size={size}
         />
         <p className="loading-message">{message}</p>
      </div>
   );
};

export default Loading;
