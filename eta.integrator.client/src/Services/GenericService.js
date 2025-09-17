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

      if (response.status == 0) throw response;

      return response.data; // Return the parsed response data
   } catch (error) {
      handleErrorGeneric(error);
   }
};

const updateStepFactory = async (values, step) => {
   try {
      const updateStepDTO = {
         Order: step,
         Data: JSON.stringify(values),
      };

      const response = await makeRequestFactory(
         "POST",
         "/Config/UpdateStep",
         updateStepDTO
      );

      return response;
   } catch (error) {
      handleErrorGeneric(error);
   }
};

const handleErrorGeneric = (error) => {
   if (error.response) {
      throw {
         status: error.response.status,
         message: `${error.response.data.title || "An unexpected error occurred."}`,
         detail: `${error.response.data.detail || "No additional details available."}`,
      };
   } else if (error.request) {
      throw {
         status: 400,
         message: "No response from server, please try again later.",
         detail: "No additional details available.",
      };
   } else {
      throw {
         status: 500,
         message: `${error.message || "An unexpected error occurred."}`,
         detail: "No additional details available.",
      };
   }
};

export default { makeRequestFactory, updateStepFactory, handleErrorGeneric };
