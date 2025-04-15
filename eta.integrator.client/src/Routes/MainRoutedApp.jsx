import React, { useEffect, useState } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import { Flex } from "antd";

import InvoicesPage from "../Pages/InvoicesPage.jsx";
import NotFoundPage from "../Pages/NotFoundPage.jsx";
import LoginFormPage from "../Pages/LoginFormPage.jsx";

import DefaultLayout from "../Components/DefaultLayout.jsx";
import StepperWrapper from "../Components/StepperWrapper.jsx";

import Styles from "../assets/Styles.js";
import useAuthPresistence from "../Hooks/useAuthPresistence.jsx";
import AuthService from "../Services/AuthService.js";
import EnforceStepperFlow from "../Components/EnforceStepperFlow.jsx";
import { ROUTES } from "../Constants/Constants.jsx";
import { isUndefined } from "../Constants/Helpers.js";

const ProtectedRoute = ({ isLoggedIn, userProgress, children }) => {
   if (!isLoggedIn)
      return (
         <Navigate
            to="/login"
            replace
         />
      );
   else if (userProgress !== "completed") {
      return (
         <Navigate
            to="/"
            replace
         />
      );
   } else {
      return children;
   }
};

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
                  userProgressResponse.step &&
                  !isUndefined(userProgressResponse.step)
               ) {
                  setUserProgress(userProgressResponse.step);
                  console.log("User progress:", userProgressResponse.step);
               }
               setUserProgress(1);
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
   //TODO: Add a loading spinner or some indication of loading state
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
                  isLoggedIn ? (
                     userProgress === "completed" ? (
                        <Navigate
                           to={ROUTES.COMPLETED}
                           replace
                        />
                     ) : userProgress === 2 ? (
                        <Navigate
                           to={ROUTES.SECOND_STEP}
                           replace
                        />
                     ) : (
                        <Navigate
                           to={ROUTES.FIRST_STEP}
                           replace
                        />
                     )
                  ) : (
                     <Navigate
                        to={ROUTES.LOGIN}
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
                  <ProtectedRoute isLoggedIn={isLoggedIn} userProgress={userProgress}>
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
               path={ROUTES.HOME}
               element={
                  <ProtectedRoute
                     isLoggedIn={isLoggedIn}
                     userProgress={userProgress}
                  >
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
                  path={ROUTES.COMPLETED}
                  element={<InvoicesPage isMobile={isMobile} />}
               />
               {/* <Route
                  path="*"
                  element={<NotFoundPage isMobile={isMobile} />}
               /> */}
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
