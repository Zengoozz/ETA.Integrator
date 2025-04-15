import { useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthService from "../Services/AuthService";

export const useLogin = (setLogIn) => {
   const navigate = useNavigate();
   const [loading, setLoading] = useState(false);

   const handleLogin = async (values) => {
      setLoading(true);
      try {
         const loginResponse = await AuthService.login(values);
         console.log("Login successful:", loginResponse);

         if (loginResponse) {
            setLogIn(true);
            navigate("/", { replace: true });
         }

      } catch (err) {
         console.error("Login failed:", err);
      } finally {
         setLoading(false);
      }
   };

   return { handleLogin, loading };
};
