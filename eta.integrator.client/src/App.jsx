import { BrowserRouter as Router } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { ConfigProvider, theme } from 'antd';

import MainRoutedApp from './Routes/MainRoutedApp';
// import {useConfig , CustomConfigProvider} from './Componenets/CustomConfigProvider.jsx';
// import './assets/css/App.css';

function App() {
    const [mode, setMode] = useState('Light');
    const [isLoggedIn, setLogIn] = useState(false);

    useEffect(() => {
        // fetch('/config/landing')
        //     .then(res => res.json())
        //     .then(data => setLanding(data.landing));

        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        prefersDark ? setMode('Dark') : setMode('Light');

    }, []);

    // if (!landing) return <div>Loading...</div>;

    return (
        <ConfigProvider
            theme={{
                algorithm: mode == "Dark" ? theme.darkAlgorithm : theme.defaultAlgorithm,
            }}
        >
            <Router>
                <MainRoutedApp mode={mode} setMode={setMode} isLoggedIn={isLoggedIn} setLogIn={setLogIn} />
            </Router>
        </ConfigProvider>
    );

}

export default App;

