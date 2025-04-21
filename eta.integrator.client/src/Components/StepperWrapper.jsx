import React from "react";
import { Steps, Button, Flex } from "antd";
import { useNavigate } from "react-router-dom";

import ConnectionSettingsPage from "../Pages/ConnectionSettingsPage.jsx";
import IssuerSettingsPage from "../Pages/IssuerSettingsPage.jsx";

import { ROUTES } from "../Constants/Constants.js";

const StepperWrapper = ({ currentStep, isMobile }) => {
   const navigate = useNavigate();

   // Define the steps
   const steps = [
      {
         title: "Connection Settings",
         content: <ConnectionSettingsPage isMobile={isMobile} />,
      },
      {
         title: "Issuer Settings",
         content: (
            <IssuerSettingsPage
               isMobile={isMobile} // Assuming you have a home route
            />
         ),
      },
   ];

   // Handle navigation to the next step
   const goToNextStep = () => {
      if (currentStep === 1) {
         navigate(ROUTES.SECOND_STEP);
      } else if (currentStep === 2) {
         navigate(ROUTES.COMPLETED);
      }
   };

   return (
      <Flex
         vertical
         align="center"
         justify="center"
         style={{ width: "100%" }}
      >
         {/* Stepper */}
         <Steps
            current={currentStep - 1} // Steps are zero-indexed
            size={isMobile ? "small" : "default"}
            style={{ width: "100%", maxWidth: "600px", margin: "0 auto 24px" }}
         >
            {steps.map((step, index) => (
               <Steps.Step
                  key={index}
                  title={step.title}
               />
            ))}
         </Steps>

         {/* Content for the current step */}
         <div style={{ width: "100%", maxWidth: "600px", margin: "0 auto" }}>
            {steps[currentStep - 1].content}
         </div>

         {/* Navigation Buttons */}
         <Flex
            gap="small"
            justify="center"
            style={{ marginTop: "24px" }}
         >
            {currentStep > 1 && (
               <Button
                  onClick={() => navigate(-1)} // Go back to the previous step
                  size={isMobile ? "large" : "middle"}
               >
                  Previous
               </Button>
            )}
            <Button
               type="primary"
               onClick={goToNextStep}
               size={isMobile ? "large" : "middle"}
            >
               {currentStep === steps.length ? "Finish Setup" : "Next Step"}
            </Button>
         </Flex>
      </Flex>
   );
};

export default StepperWrapper;
