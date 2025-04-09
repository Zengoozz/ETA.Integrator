const login = async (credentials) => {
   const response = await fetch("HMS/Login", {
      method: "POST",
      headers: {
         "Content-Type": "application/json",
      },
      body: JSON.stringify({
         Username: credentials.username,
         Password: credentials.password,
      }),
   });

   const data = await response.json();

   return data;
};

const getSettings = async () => {
   const response = await fetch("HMS/Login/Settings", {
      method: "GET",
   });

   if(!response.ok)
      throw new Error(`Fetching Settings: HTTP error! Status: ${response.status}`);

   const data = await response.json();

   return data;
};

const saveSettings = async () => {};

export default { login, getSettings };
