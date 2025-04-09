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

export default { login };
