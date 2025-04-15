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
         const loginResponse = await AuthService.login(values);
         console.log("Login successful:", loginResponse);
         if(loginResponse){
            const progressResponse = await AuthService.getUserProgress();
            console.log("User progress:", progressResponse);

            if (progressResponse.step === "completed") {
               navigate("/home");
            } else if (progressResponse.step === 2) {
               navigate("/issuer-settings");
            } else {
               navigate("/connection-settings");
            }
         }
        

         setLogIn(true);
      } catch (err) {
         console.error("Login failed:", err);
      } finally {
         setLoading(false);
      }
   };

   return { handleLogin, loading };
};
