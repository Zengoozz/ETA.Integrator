const mockFunc = (text, values, obj = true) => {
   console.log(text, values);
   return new Promise((resolve) => {
      setTimeout(() => {
         resolve(obj);
      }, 1000); // Simulate network delay
   });
};

const login = async (credentials) => mockFunc("UserCredentials:", credentials);

const getUserProgress = async () => mockFunc("UserProgress:", "", { step: 2 });

const getConnectionSettings = async () =>
   mockFunc("ConnectionSettings:", "", {
      ConnectionString: "",
      ClientId: "",
      ClientSecret: "",
   });

const updateConnectionSettings = async (values) =>
   mockFunc("updateConnectionSettings:", values);

const getIssuerSettings = async () =>
   mockFunc("IssuerSettings:", "", {
      IssuerName: "",
      TaxId: "",
   });

const updateIssuerSettings = async (values) => mockFunc("updateIssuerSettings:", values);

export default {
   login,
   getUserProgress,
   getConnectionSettings,
   updateConnectionSettings,
   getIssuerSettings,
   updateIssuerSettings,
};
