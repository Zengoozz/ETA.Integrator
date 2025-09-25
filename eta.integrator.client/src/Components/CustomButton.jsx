import { Button } from "antd";

const CustomButton = ({
   name = "Submit",
   icon,
   loading,
   handleClick,
   type = "primary",
   style = {}
}) => {
   return (
      <>
         <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: 16 }}>
            <Button
               type={type}
               icon={icon}
               size="large"
               onClick={handleClick}
               loading={loading}
               disabled={loading}
               style={style}
            >
               {name}
            </Button>
         </div>
      </>
   );
};

export default CustomButton;
