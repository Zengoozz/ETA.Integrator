import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../Constants/Constants";

const useLogout = ({ setLogin }) => {
   const navigate = useNavigate();

   useEffect(() => {
      localStorage.removeItem("HMS_Token");
      setLogin(false);
      navigate(ROUTES.LOGIN, { replace: true });
   }, [navigate, setLogin]);
};

export default useLogout;
