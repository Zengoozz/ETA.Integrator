import { CheckCircleTwoTone, CloseCircleTwoTone } from '@ant-design/icons';
import dayjs from 'dayjs';

const LoginFormValidationRules = {
    username: [
        { required: true, message: 'Required' }
    ],
    password: [
        { required: true, message: 'Required' },
        { min: 6, message: 'Password too short' }
    ],
};

const InvoicesTableColumns = [
    {
        title: 'Receipt Number',
        dataIndex: 'inoviceNumber',
        render: text => <a>{text}</a>,
    },
    {
        title: 'Visit Type',
        dataIndex: 'inoviceType',
        render: text => text,
    },
    {
        title: 'Company',
        dataIndex: 'financialClassName',
        render: text => text,
    },
    {
        title: 'Tax Registeration Number',
        dataIndex: 'inovicingNumber',
        render: text => text,
    },
    {
        title: 'Net Price',
        dataIndex: 'netPrice',
        render: value => value.toFixed(2)
    },
    {
        title: 'Patient Share',
        dataIndex: 'patShare',
        render: value => value.toFixed(2)
    },
    {
        title: 'Financial Share',
        dataIndex: 'finShare',
        render: value => value.toFixed(2)
    },
    {
        title: 'Vat Net',
        dataIndex: 'vatNet',
        render: value => value.toFixed(2)
    },
    {
        title: 'Date',
        dataIndex: 'createdDate',
        render: value => <>{dayjs(value).format("DD/MM/YYYY")}</>
    },
    {
        title: 'Status',
        dataIndex: 'isReviewed',
        render: value => {
            return (
                <span style={{ display: 'flex', width: '100%', justifyContent: 'center' }}>
                    {
                        value ?
                            <CheckCircleTwoTone style={{ fontSize: 30 }} twoToneColor={['green', 'transparent']} /> :
                            <CloseCircleTwoTone style={{ fontSize: 30 }} twoToneColor={['red', 'transparent']} />}
                </span>
            )
        }
    },
];

const ROUTES = {
    HOME:"/home",
    LOGIN: "/login",
    FIRST_STEP: "/connection-settings",
    SECOND_STEP: "/issuer-settings",
    COMPLETED: "/home/invoices",
 };

export { LoginFormValidationRules, InvoicesTableColumns, ROUTES  };