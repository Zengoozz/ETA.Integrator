export const loginContentStyle = {
   padding: "0 48px",
   display: "flex",
   justifyContent: "center",
   alignItems: "center",
};

export const homeContentStyle = {
   padding: "0 48px",
   display: "flex",
   justifyContent: "start",
   alignItems: "start",
   height: "auto", // Changed from fixed height
   minHeight: 300,
};

export const darkNavbarTitleStyle = {
   margin: 0,
   color: "#000",
};

export const lightNavbarTitleStyle = {
   margin: 0,
   color: "#fff",
};

export const darkNavbarIconStyle = {
   fontSize: "20px",
   color: "#000",
   cursor: "pointer",
};

export const lightNavbarIconStyle = {
   fontSize: "20px",
   color: "#fff",
   cursor: "pointer",
};

export default {
   loginContentStyle,
   homeContentStyle,
   darkNavbarIconStyle,
   lightNavbarIconStyle,
   darkNavbarTitleStyle,
   lightNavbarTitleStyle,
};

export const responsiveStyle = {
   xs: { span: 24 },
   sm: { span: 12 },
   md: { span: 8 },
   lg: { span: 6 },
   xl: { span: 4 },
};
