import React from "react";
import { Navigate } from "react-router-dom";
import { isUndefined } from "../Constants/Helpers";
import { ROUTES } from "../Constants/Constants";

const EnforceStepperFlow = ({ userProgress, requiredStep, redirectTo, children }) => {
   // Redirect if the user is not on the correct step
   console.log("ESF: User progress:", userProgress);
   console.log("ESF: requiredStep:", requiredStep);
   if (Number(userProgress) !== requiredStep) {
      return isUndefined(userProgress) ? (
         <Navigate
            to={ROUTES.FIRST_STEP}
            replace
         />
      ) : (
         <Navigate
            to={redirectTo}
            replace
         />
      );
   }

   // Render the child component if the user is allowed to access this route
   return children;
};

export default EnforceStepperFlow;
