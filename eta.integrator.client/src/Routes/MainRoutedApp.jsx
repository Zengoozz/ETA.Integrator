import InvoicesPage from "../Pages/InvoicesPage.jsx";
import NotFoundPage from "../Pages/NotFoundPage.jsx";

import DefaultLayout from "../Components/DefaultLayout.jsx";
import LoginForm from "../Components/LoginForm.jsx";
import Settings from "../Components/Settings.jsx";

import Styles from "../assets/Styles.js";

import { Routes, Route, Navigate } from "react-router-dom";
import { Flex } from "antd";

const ProtectedRoute = ({ isLoggedIn, children }) => {
   if (!isLoggedIn)
      return (
         <Navigate
            to="/login"
            replace
         />
      );
   return children;
};

const MainRoutedApp = ({ mode, setMode, isLoggedIn, setLogIn, isMobile }) => {
   return (
      <Flex
         vertical
         style={{ minHeight: "100vh" }}
      >
         <Routes>
            {/* 🔁 Root route redirects to login or home */}
            <Route
               path="/"
               element={
                  isLoggedIn ? (
                     <Navigate
                        to="/home"
                        replace
                     />
                  ) : (
                     <Navigate
                        to="/login"
                        replace
                     />
                  )
               }
            />

            {/* 🌐 Public layout: login + settings */}
            <Route
               element={
                  <DefaultLayout
                     mode={mode}
                     setMode={setMode}
                    //  maxWidthValue={400}
                    //  contentStyle={Styles.loginContentStyle}
                    maxWidthValue={isMobile ? "100%" : 500}
                    contentStyle={{
                        ...Styles.loginContentStyle,
                        padding: isMobile ? "16px" : "48px", // Responsive padding
                     }}
                     isMarginedTop={false}
                     isMobile={isMobile}
                  />
               }
            >
               <Route
                  path="/login"
                  element={<LoginForm setLogIn={setLogIn} isMobile={isMobile} />}
               />
               <Route
                  path="/settings"
                  element={<Settings isMobile={isMobile} />}
               />
            </Route>

            {/* 🔒 Protected layout (needs login) */}
            <Route
               path="/home"
               element={
                  <ProtectedRoute isLoggedIn={isLoggedIn}>
                     <DefaultLayout
                        mode={mode}
                        setMode={setMode}
                        maxWidthValue="100%"
                        contentStyle={Styles.homeContentStyle}
                        isMarginedTop={true}
                        isMobile={isMobile}
                     />
                  </ProtectedRoute>
               }
            >
               <Route
                  index
                  element={
                     <Navigate
                        to="/home/invoices"
                        replace
                     />
                  }
               />
               <Route
                  path="invoices"
                  element={<InvoicesPage isMobile={isMobile} />}
               />
               <Route
                  path="*"
                  element={<NotFoundPage isMobile={isMobile} />}
               />
            </Route>

            {/* 🔚 Fallback */}
            <Route
               path="*"
               element={<NotFoundPage isMobile={isMobile} />}
            />
         </Routes>
      </Flex>
   );
};

export default MainRoutedApp;
