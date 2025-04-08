import { CheckCircleTwoTone, CloseCircleTwoTone } from '@ant-design/icons';


export const InvoicesTableColumns = [
    {
        title: 'Receipt Number',
        dataIndex: 'receiptnumber',
        render: text => <a>{text}</a>,
    },
    {
        title: 'Visit Type',
        dataIndex: 'visittype',
    },
    {
        title: 'Company',
        dataIndex: 'company',
    },
    {
        title: 'Tax Registeration Number',
        dataIndex: 'taxregisterationnumber',
    },
    {
        title: 'Net Price',
        dataIndex: 'netprice',
        render: value => value.toFixed(2)
    },
    {
        title: 'Patient Share',
        dataIndex: 'patientshare',
        render: value => value.toFixed(2)
    },
    {
        title: 'Financial Share',
        dataIndex: 'financialshare',
        render: value => value.toFixed(2)
    },
    {
        title: 'Vat Net',
        dataIndex: 'vatnet',
        render: value => value.toFixed(2)
    },
    {
        title: 'Date',
        dataIndex: 'date',
        render: value => <>{value.toLocaleDateString('en-GB')}</>
    },
    {
        title: 'Status',
        dataIndex: 'status',
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