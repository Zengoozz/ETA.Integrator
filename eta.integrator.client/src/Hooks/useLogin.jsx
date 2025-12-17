import { useNavigate } from "react-router-dom";

import AuthService from "../Services/AuthService";
import { handleErrorNotification } from "../Services/GenericService";

export const useLogin = (
   setLogIn,
   notificationApi,
   setLoading,
   setCredentialsIncorrect
) => {
   const navigate = useNavigate();

   const handleLogin = async (values) => {
      setLoading(true);
      try {
         const loginResponse = await AuthService.login(values);
         console.log("Login successful:", loginResponse);

         if (loginResponse) {
            setLogIn(true);
            navigate("/", { replace: true });
         }
      } catch (error) {
         handleErrorNotification(notificationApi, error);
         setCredentialsIncorrect(true);
      } finally {
         setLoading(false);
      }
   };

   return { handleLogin };
};
