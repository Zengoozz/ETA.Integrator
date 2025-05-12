const mockFunc = (text, values, obj = true) => {
   console.log(text, values);
   return new Promise((resolve) => {
      setTimeout(() => {
         resolve(obj);
      }, 1000); // Simulate network delay
   });
};

//e945b032-513d-4bd4-8678-6860bb490649

const login = async (credentials) => {
   localStorage.setItem("HMS_Token", "token");
   return mockFunc("UserCredentials:", credentials);
};

const logout = async () => {
   localStorage.removeItem("HMS_Token");
   return mockFunc("Logout:", "");
};

const getUserProgress = async () => mockFunc("UserProgress:", "", "completed");

const getConnectionSettings = async () =>
   mockFunc("ConnectionSettings:", "", {
      ConnectionString: "",
      ClientId: "",
      ClientSecret: "",
   });

const getIssuerSettings = async () =>
   mockFunc("IssuerSettings:", "", {
      IssuerName: "",
      TaxId: "",
   });

const updateStep = async (values, step) => {
   console.log("UpdateStep:", values, step);
   return mockFunc("UpdateStep:", values, "UPDATED");
};

const connectToConsumer = async (values) => {
   console.log("ConnectToETA:", values);
   return mockFunc("ConnectToETA:", values, true);
};

export default {
   login,
   logout,
   getUserProgress,
   getConnectionSettings,
   getIssuerSettings,
   updateStep,
   connectToConsumer
};
