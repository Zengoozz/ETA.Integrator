import { useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthService from "../Services/AuthService";

export const useLoginForm = (setLogIn) => {
   const navigate = useNavigate();
   const [loading, setLoading] = useState(false);

   const handleLogin = async (values) => {
      setLoading(true);
      try {
         //TODO: Add your real login logic here
         const response = await AuthService.login(values);
         console.log(response);

         if (response) {
            navigate("/home");
            setLogIn(true);
         }
      } catch (err) {
         console.error("Login failed:", err);
      } finally {
         setLoading(false);
      }
   };

   return { handleLogin, loading };
};
