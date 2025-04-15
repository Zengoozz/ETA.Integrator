import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../Constants/Constants";
import { isUndefined } from "../Constants/Helpers";

const useAuthPresistence = (setLogin) => {
   const navigate = useNavigate();

   useEffect(() => {
      const storedToken = localStorage.getItem("HMS_Token");
      const currentPath = window.location.pathname;

      if (!storedToken || isUndefined(storedToken)) {
         console.error("No valid token found. Redirecting to login...");

         if (currentPath !== ROUTES.LOGIN) {
            navigate(ROUTES.LOGIN, { replace: true });
         }
      } else {
         setLogin(true);
         console.log("Valid token found. User is authenticated.");

         if (currentPath === ROUTES.LOGIN) {
            navigate(ROUTES.DEFAULT, { replace: true });
         }
      }
   }, [navigate, setLogin]);
};

export default useAuthPresistence;
