import InvoicesPage from '../Pages/InvoicesPage.jsx';
import NotFoundPage from '../Pages/NotFoundPage.jsx'
import DefaultPage from '../Pages/DefaultPage.jsx';

import LoginForm from '../Componenets/LoginForm.jsx'
import Settings from '../Componenets/Settings.jsx'

import { Routes, Route, Navigate } from 'react-router-dom';


const MainRoutedApp = ({ mode, setMode, isLoggedIn, setLogIn }) => {
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
        height: '500px'
    };

    return (
        <>

            <Routes>

                <Route path="/" element={<DefaultPage mode={mode} setMode={setMode} maxWidthValue={400} contentStyle={loginContentStyle} />}>

                    <Route path="/"
                        element={
                            isLoggedIn ? (
                                <Navigate to="/home" replace />
                            ) : (
                                <Navigate to="/login" replace />
                            )
                        } />

                    <Route path="/login" element={
                        <LoginForm setLogIn={setLogIn}/>
                    } />

                    <Route path='/settings' element={
                        <Settings />
                    } />

                    <Route path="*" element={<NotFoundPage />} />
                </Route>


                <Route path="/home" element={<DefaultPage mode={mode} setMode={setMode} maxWidthValue={'100%'} contentStyle={homeContentStyle} />}>

                    <Route path="/home" element={
                        isLoggedIn ? (
                            <Navigate to="/home/invoices" replace />
                        ) : (
                            <Navigate to="/login" replace />
                        )
                    } />

                    <Route path="/home/invoices" element={
                        <InvoicesPage />
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

export default MainRoutedApp;