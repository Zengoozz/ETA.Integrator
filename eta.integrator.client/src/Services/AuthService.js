const login = async (credentials) => {
   const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/Login/Login`, {
      method: "POST",
      headers: {
         "Content-Type": "application/json",
      },
      body: JSON.stringify({
         Username: credentials.username,
         Password: credentials.password,
      }),
   });

   if (response.status != 200) {
      throw new Error("Invalid username or password, Status: " + response.status);
   } else {
      const data = await response.json();

      const token = data.token;

      localStorage.setItem("HMS_Token", token);

      return true;
   }
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
   const response = await fetch(`HMS/Login/SaveSettings`, {
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
