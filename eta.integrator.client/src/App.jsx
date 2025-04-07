import Landing from './Pages/Landing.jsx'
// import './assets/css/App.css';

import { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Button, ConfigProvider, theme } from 'antd';
import Settings from './Pages/Settings.jsx';

function App() {
    const [isDarkMode, setIsDarkMode] = useState(false);
    const [landing, setLanding] = useState('settings');

    useEffect(() => {
        fetch('/config/landing')
            .then(res => res.json())
            .then(data => setLanding(data.landing));

        console.log(landing);

        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        setIsDarkMode(prefersDark);
    }, [landing]);

    if (!landing) return <div>Loading...</div>;

    return (
        <ConfigProvider
            theme={{
                algorithm: isDarkMode ? theme.darkAlgorithm : theme.defaultAlgorithm,
            }}
        >
            <Router>
                <Routes>
                    <Route path="/" element={
                        <Navigate to={`/${landing}`} replace />
                    } />

                    <Route path='/login' element={
                        <Landing colorMode={isDarkMode}
                            setColorMode={setIsDarkMode} />
                    } />

                    <Route path='/settings' element={
                        <Settings />
                    } />
                </Routes>
            </Router>
        </ConfigProvider>
    );

}

export default App;