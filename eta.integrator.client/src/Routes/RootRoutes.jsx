import React from "react";
import { Navigate } from "react-router-dom";

import { ROUTES } from "../Constants/Constants";

const RootRoutes = ({ isLoggedIn, userProgress }) => {
   if (!isLoggedIn) {
      return (
         <Navigate
            to={ROUTES.LOGIN}
            replace
         />
      );
   }

   if (userProgress === "completed") {
      return (
         <Navigate
            to={ROUTES.COMPLETED}
            replace
         />
      );
   }

   if (Number(userProgress) === 2) {
      return (
         <Navigate
            to={ROUTES.SECOND_STEP}
            replace
         />
      );
   }

   return (
      <Navigate
         to={ROUTES.FIRST_STEP}
         replace
      />
   );
};

export default RootRoutes;
