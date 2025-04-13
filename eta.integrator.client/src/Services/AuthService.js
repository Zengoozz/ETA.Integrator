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
   // const response = await fetch(`HMS/Login/Settings`, {
   //    method: "GET",
   // });

   const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Login/Settings`, {
      method: "GET",
   });


   if (!response.ok)
      throw new Error(`Fetching Settings: HTTP error! Status: ${response.status}`);

   console.log(response);

   const data = await response.json();

   return data;
};

const saveSettings = async (values) => {
   const response = await fetch("HMS/Login/SaveSettings", {
      method: "POST",
      headers: {
         "Content-Type": "application/json",
      },
      body: JSON.stringify({
         ConnectionString: values.connectionString,
         ClientId: values.clientId,
         ClientSecret: values.clientSecret,
      }),
   });

   if (!response.ok)
      throw new Error(
         `Fetching Settings: HTTP error! Status: ${response.status} ${response.body}`
      );

   const data = await Response.json();
   console.log(data);

   return data;
};

export default { login, getSettings, saveSettings };
