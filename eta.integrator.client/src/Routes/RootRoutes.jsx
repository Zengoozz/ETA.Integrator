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
   
   return (
      <Navigate
         to={ROUTES.CONFIG}
         replace
      />
   );
};

export default RootRoutes;
