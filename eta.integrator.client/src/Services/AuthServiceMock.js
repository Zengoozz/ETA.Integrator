const mockFunc = (text, values, obj = true) => {
   console.log(text, values);
   return new Promise((resolve) => {
      setTimeout(() => {
         resolve(obj);
      }, 1000); // Simulate network delay
   });
};

const login = async (credentials) => {
   localStorage.setItem("HMS_Token", "token");
   return mockFunc("UserCredentials:", credentials);
};

const logout = async () => {
   localStorage.removeItem("HMS_Token");
   return mockFunc("Logout:", "");
};

const getUserProgress = async () => mockFunc("UserProgress:", "", { step: "completed" });

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
   return mockFunc("UpdateStep:", values, true);
};

export default {
   login,
   getUserProgress,
   getConnectionSettings,
   getIssuerSettings,
   updateStep,
   logout
};
