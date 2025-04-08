import NotFoundPage from './Pages/NotFoundPage.jsx'
import DefaultPage from './Pages/DefaultPage.jsx';
import LoginForm from './Componenets/LoginForm.jsx'
import Settings from './Componenets/Settings.jsx'
// import './assets/css/App.css';

import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { ConfigProvider, theme } from 'antd';
import HomePage from './Pages/HomePage.jsx';
// import {useConfig , CustomConfigProvider} from './Componenets/CustomConfigProvider.jsx';

function App() {
    const [mode, setMode] = useState('Light');

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
                <MainRoutedApp mode={mode} setMode={setMode} />
            </Router>
        </ConfigProvider>
    );

}

export default App;


function MainRoutedApp({ mode, setMode }) {
    var loginContentStyle = {
        padding: '0 48px',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
    };

    var homeContentStyle = {
        padding: '0 48px',
        display: 'flex',
        justifyContent: 'start',
        alignItems: 'start',
        height:'500px'
    };

    return (
        <>

            <Routes>

                <Route path="/" element={<DefaultPage mode={mode} setMode={setMode} maxWidthValue={400} contentStyle={loginContentStyle} />}>

                    <Route path="/" element={
                        <LoginForm />
                    } />

                    <Route path="/login" element={
                        <LoginForm />
                    } />

                    <Route path='/settings' element={
                        <Settings />
                    } />

                    <Route path="*" element={<NotFoundPage />} />
                </Route>


                <Route path="/home" element={<DefaultPage mode={mode} setMode={setMode} maxWidthValue={'100%'} contentStyle={homeContentStyle}/>}>

                    <Route path="/home" element={
                        <HomePage />
                    } />

                    {/* <Route path="/login" element={
                        <HomePage />
                    } />

                    <Route path='/settings' element={
                        <Settings />
                    } /> */}

                    <Route path="*" element={<NotFoundPage />} />
                </Route>


            </Routes>
        </>

    );
}