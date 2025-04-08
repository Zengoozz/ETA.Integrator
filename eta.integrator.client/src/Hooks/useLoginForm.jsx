import React from 'react';
import { useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';


export const useLoginForm = (setLogIn) => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);


    const handleLogin = useCallback(async (values) => {
        setLoading(true);
        try {
            //TODO: Add your real login logic here

            // const response = await fetch('HMS/Login', {
            //     method: 'POST',
            //     headers: {
            //         'Content-Type': 'application/json'
            //     },
            //     body: JSON.stringify({
            //         Username: values.username,
            //         Password: values.password
            //     })
            // });
            // const data = await response.json();
            // console.log(data);

            console.log('Logging in...', values);
            setTimeout(() => {
                navigate('/home');
                setLogIn(true);
            }, 0);
        } catch (err) {
            console.error('Login failed:', err);
        } finally {
            setLoading(false);
        }
    }, [navigate, setLogIn]);

    return { handleLogin, loading };
};