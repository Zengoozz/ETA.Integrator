import axios from "axios";

const makeRequestFactory = async (method, url, data = null, headers = {}) => {
   try {
      const fullUrl =
         url.startsWith("http") || url.startsWith("https")
            ? url
            : `${import.meta.env.VITE_API_BASE_URL}${url}`;

      const response = await axios({
         method,
         url: fullUrl,
         data, // Request body (used for POST, PUT, etc.)
         headers: {
            "Content-Type": "application/json", // Default header
            ...headers, // Allow overriding or adding custom headers
         },
      });

      return response.data; // Return the parsed response data
   } catch (error) {
      if (error.response) {
         // Server responded with an error status (e.g., 400, 500)
         throw new Error(`Error: ${error.message}, Status: ${error.response.status}`);
      } else if (error.request) {
         // No response received from the server
         throw new Error("No response from server, please try again later.");
      } else {
         // Something went wrong during request setup
         throw new Error(`Error: ${error.message}`);
      }
   }
};

const updateStepFactory = async (values, step) => {
   try {
      const updateStepDTO = {
         Order: step,
         Data: JSON.stringify(values)
      };
      console.log("updateStep", updateStepDTO);

      const response = await makeRequestFactory("POST", "/Config/UpdateStep", updateStepDTO);

      return response;
   } catch (error) {
      console.error(error.message);
      throw error;
   }
}
export default { makeRequestFactory, updateStepFactory };
