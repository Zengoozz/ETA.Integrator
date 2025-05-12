import AuthServiceMock from "./AuthServiceMock";
import GenericService from "./GenericService";

const login = async (credentials) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "POST",
         "/Auth/ProviderConnect",
         {
            Email: credentials.username,
            Password: credentials.password,
         }
      );

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
      const stepResponse = await GenericService.makeRequestFactory(
         "GET",
         "/Config/UserProgress"
      );

      console.log("UserProgress:", stepResponse);

      return stepResponse;
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
         TokenPin: response.TokenPin
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
         RegistrationNumber: response.registrationNumber,
         IssuerType: response.issuerType,
         Address: {
            Country: response.address.country,
            Governate: response.address.governate,
            RegionCity: response.address.regionCity,
            BranchId: response.address.branchId,
            BuildingNumber: response.address.buildingNumber,
            Street: response.address.street,
         },
      };

      console.log("IssuerSettings:", myResponse);

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

const connectToConsumer = async (values) => {
   try {
      const response = await GenericService.makeRequestFactory(
         "POST",
         "/Auth/ConsumerConnect",
         values
      );

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
   connectToConsumer,
};

AuthService = AuthServiceMock;

export default AuthService;
