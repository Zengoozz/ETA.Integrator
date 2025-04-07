import { useState, useEffect } from 'react';
// import './assets/css/App.css';
import Landing from './Pages/Landing.jsx'
import { Button, ConfigProvider, theme } from 'antd';

function App() {
    const [isDarkMode, setIsDarkMode] = useState(false);

    useEffect(() => {
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        setIsDarkMode(prefersDark);
    }, []);

    return (
        <ConfigProvider
            theme={{
                algorithm: isDarkMode ? theme.darkAlgorithm : theme.defaultAlgorithm,
            }}
        >
            <Landing colorMode={isDarkMode} setColorMode={setIsDarkMode} />
        </ConfigProvider>
    );

}

export default App;