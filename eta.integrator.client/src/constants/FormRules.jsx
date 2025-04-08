export const LoginFormValidationRules = {
    username: [
        { required: true, message: 'Required' }
    ],
    password: [
        { required: true, message: 'Required' },
        { min: 6, message: 'Password too short' }
    ],
};