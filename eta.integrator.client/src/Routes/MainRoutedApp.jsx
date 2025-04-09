import InvoicesPage from '../Pages/InvoicesPage.jsx';
import NotFoundPage from '../Pages/NotFoundPage.jsx'

import DefaultLayout from '../Componenets/DefaultLayout.jsx';
import LoginForm from '../Componenets/LoginForm.jsx'
import Settings from '../Componenets/Settings.jsx'

import Styles from '../assets/Styles.js';

import { Routes, Route, Navigate } from 'react-router-dom';


const ProtectedRoute = ({ isLoggedIn, children }) => {
    if (!isLoggedIn) return <Navigate to="/login" replace />;
    return children;
};

const MainRoutedApp = ({ mode, setMode, isLoggedIn, setLogIn }) => {
    
    return (
        <>

            <Routes>
                {/* 🔁 Root route redirects to login or home */}
                <Route
                    path="/"
                    element={
                        isLoggedIn
                            ? <Navigate to="/home" replace />
                            : <Navigate to="/login" replace />
                    }
                />

                {/* 🌐 Public layout: login + settings */}
                <Route
                    element={
                        <DefaultLayout
                            mode={mode}
                            setMode={setMode}
                            maxWidthValue={400}
                            contentStyle={Styles.loginContentStyle}
                        />
                    }
                >
                    <Route path="/login" element={<LoginForm setLogIn={setLogIn} />} />
                    <Route path="/settings" element={<Settings />} />
                </Route>

                {/* 🔒 Protected layout (needs login) */}
                <Route
                    path="/home"
                    element={
                        <ProtectedRoute isLoggedIn={isLoggedIn}>
                            <DefaultLayout
                                mode={mode}
                                setMode={setMode}
                                maxWidthValue="100%"
                                contentStyle={Styles.homeContentStyle}
                                isMarginedTop={true}
                            />
                        </ProtectedRoute>
                    }
                >
                    <Route index element={<Navigate to="/home/invoices" replace />} />
                    <Route path="invoices" element={<InvoicesPage />} />
                    <Route path="*" element={<NotFoundPage />} />
                </Route>

                {/* 🔚 Fallback */}
                <Route path="*" element={<NotFoundPage />} />
            </Routes>

        </>

    );
}

export default MainRoutedApp;