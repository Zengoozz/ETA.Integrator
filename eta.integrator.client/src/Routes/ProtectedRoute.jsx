import React from "react";

import { Navigate } from "react-router-dom";


const ProtectedRoute = ({ isLoggedIn, children }) => {
   return !isLoggedIn ? (
      <Navigate
         to="/login"
         replace
      />
   ) : (
      children
   );
};

export default ProtectedRoute;
