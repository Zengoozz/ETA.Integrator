import AuthServiceMock from "./AuthServiceMock";
import GenericService from "./GenericService";

const login = async (credentials) => {
   try {
      const response = await GenericService.makeRequest("POST", "/Login", {
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

const getUserProgress = async () => {
   try {
      const response = await GenericService.makeRequest("GET", "/Login/UserProgress");

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const getConnectionSettings = async () => {
   try {
      const response = await GenericService.makeRequest("GET", "/Login/Settings");

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const updateConnectionSettings = async (values) => {
   try {
      const updateSettings = {
         ConnectionString: values.connectionString,
         ClientId: values.clientId,
         ClientSecret: values.clientSecret,
      };
      console.log("updateSettings", updateSettings);

      const response = await GenericService.makeRequest(
         "PUT",
         "/Login/Settings",
         updateSettings
      );

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

const updateIssuerSettings = async (values) => {
   try {
      const updateSettings = {
         IssuerName: values.issuerName,
         TaxId: values.taxId,
      };
      console.log("updateIssuerSettings", updateSettings);

      const response = await GenericService.makeRequest(
         "PUT",
         "/Login/IssuerSettings",
         updateSettings
      );

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
};

// const AuthService = {
//    login,
//    getUserProgress,
//    getConnectionSettings,
//    updateConnectionSettings,
//    updateIssuerSettings,
// };

const AuthService = AuthServiceMock;

export default AuthService;
