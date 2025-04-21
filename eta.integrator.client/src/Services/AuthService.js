import AuthServiceMock from "./AuthServiceMock";
import GenericService from "./GenericService";

const login = async (credentials) => {
   try {
      const response = await GenericService.makeRequestFactory("POST", "/Config/Login", {
         Email: credentials.username,
         Password: credentials.password,
      });

      const token = response.token;
      localStorage.setItem("HMS_Token", token);

      return true;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

// const login = AuthServiceMock.login; // Use the mock login function for testing

const logout = async () => {
   localStorage.removeItem("HMS_Token");
};

const getUserProgress = async () => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         "/Config/UserProgress"
      );

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const getConnectionSettings = async () => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         "/Config/ConnectionSettings"
      );

      const myResponse = {
         ClientId: response.clientId,
         ClientSecret: response.clientSecret,
      };

      return myResponse;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const getIssuerSettings = async () => {
   try {
      const response = await GenericService.makeRequestFactory(
         "GET",
         "/Config/IssuerSettings"
      );

      const myResponse = {
         IssuerName: response.issuerName,
         TaxId: response.taxId,
      };

      return myResponse;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const updateStep = async (values, step) => {
   try {
      const response = await GenericService.updateStepFactory(values, step);
      console.log(values, step);
      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

let AuthService = {
   login,
   logout,
   getUserProgress,
   getConnectionSettings,
   getIssuerSettings,
   updateStep,
};

AuthService = AuthServiceMock;

export default AuthService;
