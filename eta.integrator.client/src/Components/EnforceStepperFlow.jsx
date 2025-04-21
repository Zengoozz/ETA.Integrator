import React from "react";
import { Navigate } from "react-router-dom";
import { isUndefined } from "../Constants/Helpers";
import { ROUTES } from "../Constants/Constants";

const EnforceStepperFlow = ({ userProgress, requiredStep, redirectTo, children }) => {
   // Redirect if the user is not on the correct step
   console.log("ESF: User progress:", userProgress);
   console.log("ESF: requiredStep:", requiredStep);
   
   if (userProgress === "completed")
      return (
         <Navigate
            to={ROUTES.COMPLETED}
            replace
         />
      );

   if (userProgress !== requiredStep) {
      if (isUndefined(userProgress)) {
         return (
            <Navigate
               to={ROUTES.FIRST_STEP}
               replace
            />
         );
      } else if (userProgress < requiredStep) {
         return (
            <Navigate
               to={ROUTES.FIRST_STEP}
               replace
            />
         );
      } else {
         return (
            <Navigate
               to={redirectTo}
               replace
            />
         );
      }
   }

   // Render the child component if the user is allowed to access this route
   return children;
};

export default EnforceStepperFlow;
