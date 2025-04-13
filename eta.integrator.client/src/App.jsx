import { BrowserRouter as Router } from "react-router-dom";
import { useState, useEffect } from "react";
import { ConfigProvider, theme } from "antd";
import { useMediaQuery } from "react-responsive";

import MainRoutedApp from "./Routes/MainRoutedApp";
// import {useConfig , CustomConfigProvider} from './Componenets/CustomConfigProvider.jsx';
// import './assets/css/App.css';

function App() {
   const [mode, setMode] = useState("Light");
   const [isLoggedIn, setLogIn] = useState(false);
   const isMobile = useMediaQuery({ query: "(max-width: 768px)" });

   useEffect(() => {
      // fetch('/config/landing')
      //     .then(res => res.json())
      //     .then(data => setLanding(data.landing));

      const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
      prefersDark ? setMode("Dark") : setMode("Light");
   }, []);

   // if (!landing) return <div>Loading...</div>;

   return (
      <ConfigProvider
         theme={{
            algorithm: mode == "Dark" ? theme.darkAlgorithm : theme.defaultAlgorithm,
            components: {
               // Responsive typography
               Typography: {
                  fontSize: isMobile ? 14 : 16,
               },
            },
         }}
      >
         <Router>
            <MainRoutedApp
               mode={mode}
               setMode={setMode}
               isLoggedIn={isLoggedIn}
               setLogIn={setLogIn}
               isMobile={isMobile}
            />
         </Router>
      </ConfigProvider>
   );
}

export default App;
