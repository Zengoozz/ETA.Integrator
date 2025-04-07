// src/contexts/ConfigContext.jsx
// import { createContext, useContext, useEffect, useState } from 'react';

// const ConfigContext  = createContext(null);

// export const useConfig = () => useContext(ConfigContext);

// export const CustomConfigProvider = ({ children }) => {
//   const [config, setConfig] = useState(null);
//   const [loading, setLoading] = useState(true);

//   useEffect(() => {
//     fetch('/config/landing')
//       .then(res => res.json())
//       .then(data => {
//         setConfig(data);
//         setLoading(false);
//       });
//   }, []);

//   if (loading) return <div>Loading...</div>;

//   return (
//     <ConfigContext.Provider value={config}>
//       {children}
//     </ConfigContext.Provider>
//   );
// };
