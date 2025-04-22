import React, { useEffect, useState } from "react";
import { Routes, Route, Outlet, Navigate } from "react-router-dom";
import { Flex } from "antd";

import LoginFormPage from "../Pages/LoginFormPage.jsx";
import InvoicesPage from "../Pages/InvoicesPage";
import NotFoundPage from "../Pages/NotFoundPage.jsx";

import DefaultLayout from "../Components/DefaultLayout.jsx";
import EnforceStepperFlow from "../Components/EnforceStepperFlow.jsx";
import StepperWrapper from "../Components/StepperWrapper.jsx";

import Styles from "../assets/Styles.js";
import { ROUTES } from "../Constants/Constants.js";
import { isUndefined } from "../Constants/Helpers.js";
import AuthService from "../Services/AuthService.js";
import useAuthPresistence from "../Hooks/useAuthPresistence.jsx";

import RootRoutes from "./RootRoutes.jsx";
import ProtectedRoute from "./ProtectedRoute.jsx";

const MainRoutedApp = ({ mode, setMode, isMobile }) => {
   const [isLoggedIn, setLogIn] = useState(false);
   const [userProgress, setUserProgress] = useState(null);
   const [isLoading, setLoading] = useState(true);

   useAuthPresistence(setLogIn);

   useEffect(() => {
      const fetchUserProgress = async () => {
         if (isLoggedIn) {
            try {
               const userProgressResponse = await AuthService.getUserProgress();

               if (
                  userProgressResponse &&
                  userProgressResponse &&
                  !isUndefined(userProgressResponse)
               ) {
                  setUserProgress((prev) =>
                     prev !== userProgressResponse ? userProgressResponse : prev
                  );

                  console.log("User progress:", userProgressResponse);
               } else {
                  setUserProgress(1);
               }
            } catch (error) {
               console.error("Error fetching user progress:", error);
               setUserProgress(1);
            } finally {
               setLoading(false);
            }
         } else {
            setLoading(false);
         }
      };
      fetchUserProgress();
   }, [isLoggedIn]);

   // Ensure redirection happens while loading
   // TODO: Add a loading spinner or some indication of loading state
   if (isLoading) {
      return <div>Loading...</div>;
   }

   return (
      <Flex
         vertical
         style={{ minHeight: "100vh" }}
      >
         <Routes>
            {/* 🔁 Root route ROUTES to login or home/stepper */}
            <Route
               path="/"
               element={
                  <RootRoutes
                     isLoggedIn={isLoggedIn}
                     userProgress={userProgress}
                  />
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
                     isLoggedIn={isLoggedIn}
                  />
               }
            >
               <Route
                  path={ROUTES.LOGIN}
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
                        isLoggedIn={isLoggedIn}
                     />
                  </ProtectedRoute>
               }
            >
               <Route
                  path={ROUTES.FIRST_STEP}
                  element={
                     <EnforceStepperFlow
                        userProgress={userProgress}
                        requiredStep={1}
                        redirectTo={ROUTES.SECOND_STEP}
                     >
                        <StepperWrapper
                           currentStep={1}
                           isMobile={isMobile}
                        />
                     </EnforceStepperFlow>
                  }
               />
               <Route
                  path={ROUTES.SECOND_STEP}
                  element={
                     <EnforceStepperFlow
                        userProgress={userProgress}
                        requiredStep={2}
                        redirectTo={ROUTES.COMPLETED}
                     >
                        <StepperWrapper
                           currentStep={2}
                           isMobile={isMobile}
                        />
                     </EnforceStepperFlow>
                  }
               />
            </Route>

            {/* 🔒 Protected layout (needs login) */}
            {/* 🏠 Home Routes */}
            <Route
               element={
                  <ProtectedRoute isLoggedIn={isLoggedIn}>
                     <DefaultLayout
                        mode={mode}
                        setMode={setMode}
                        maxWidthValue="100%"
                        contentStyle={Styles.homeContentStyle}
                        isMarginedTop={true}
                        isMobile={isMobile}
                        isLoggedIn={isLoggedIn}
                     />
                  </ProtectedRoute>
               }
            >
               <Route
                  path={ROUTES.COMPLETED}
                  element={
                     userProgress === "completed" ? (
                        <InvoicesPage isMobile={isMobile} />
                     ) : (
                        <Navigate
                           to="/"
                           replace
                        />
                     )
                  }
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
