import React, { useState } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import { Flex } from "antd";

import InvoicesPage from "../Pages/InvoicesPage.jsx";
import NotFoundPage from "../Pages/NotFoundPage.jsx";
import LoginFormPage from "../Pages/LoginFormPage.jsx";

import DefaultLayout from "../Components/DefaultLayout.jsx";
import StepperWrapper from "../Components/StepperWrapper.jsx";

import Styles from "../assets/Styles.js";

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

const MainRoutedApp = ({ mode, setMode, isMobile }) => {
   const [isLoggedIn, setLogIn] = useState(false);

   return (
      <Flex
         vertical
         style={{ minHeight: "100vh" }}
      >
         <Routes>
            {/* 🔁 Root route redirects to login or home/stepper */}
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

            {/* 🌐 Public layout: login */}
            <Route
               element={
                  <DefaultLayout
                     mode={mode}
                     setMode={setMode}
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
                  element={
                     <LoginFormPage
                        setLogIn={setLogIn}
                        isMobile={isMobile}
                     />
                  }
               />
            </Route>

            {/* 🔒 Protected layout (needs login) */}
            {/* 🚶 Stepper Flow */}
            <Route
               path="/"
               element={
                  <ProtectedRoute isLoggedIn={isLoggedIn}>
                     <DefaultLayout
                        mode={mode}
                        setMode={setMode}
                        maxWidthValue={isMobile ? "100%" : 500}
                        contentStyle={{
                           ...Styles.loginContentStyle,
                           padding: isMobile ? "16px" : "48px", // Responsive padding
                        }}
                        isMarginedTop={false}
                        isMobile={isMobile}
                     />
                  </ProtectedRoute>
               }
            >
               <Route
                  path="/connection-settings"
                  element={
                     <StepperWrapper
                        currentStep={1}
                        isMobile={isMobile}
                     />
                  }
               />
               <Route
                  path="/issuer-settings"
                  element={
                     <StepperWrapper
                        currentStep={2}
                        isMobile={isMobile}
                     />
                  }
               />
            </Route>

            {/* 🔒 Protected layout (needs login) */}
            {/* 🏠 Home Routes */}
            <Route
               path="/"
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
                  path="/home"
                  element={
                     <Navigate
                        to="/home/invoices"
                        replace
                     />
                  }
               />
               <Route
                  path="/home/invoices"
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
