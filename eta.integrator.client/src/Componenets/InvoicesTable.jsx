import React from 'react';
import { Divider, Radio, Table } from 'antd';
import { CheckCircleTwoTone, CloseCircleTwoTone } from '@ant-design/icons';

const columns = [
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

// const data = [
//     {
//         key: '5198',
//         receiptnumber: 5198,
//         visittype: 'OP',
//         company: 'Oscar Company-Ocsar Company-Class C',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/12/2025'),
//         status: false,
//     },
//     {
//         key: '5199',
//         receiptnumber: '5199',
//         visittype: 'OP',
//         company: 'Private Cash-cash.-نقدي',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/12/2025'),
//         status: true,
//     },
//     {
//         key: '5200',
//         receiptnumber: '5200',
//         visittype: 'INP',
//         company: 'Oscar Company-Ocsar Company-Class C',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/12/2025'),
//         status: false,
//     },
//     {
//         key: '5201',
//         receiptnumber: '5201',
//         visittype: 'INP',
//         company: 'Private Cash-cash.-نقدي',
//         taxregisterationnumber: 1232312312,
//         netprice: 87.50,
//         patientshare: 17.50,
//         financialshare: 70.00,
//         vatnet: 0.00,
//         date: new Date('03/18/2025'),
//         status: true,
//     },
// ];

const biggerData = Array.from({ length: 46 }).map((_, i) => {
    return ({
        key: i,
        receiptnumber: i,
        visittype: 'OP',
        company: 'Oscar Company-Ocsar Company-Class C',
        taxregisterationnumber: 1232312312,
        netprice: 87.50,
        patientshare: 17.50,
        financialshare: 70.00,
        vatnet: 0.00,
        date: new Date('03/12/2025'),
        status: i % 4 ? false : true,
    })
});


// rowSelection object indicates the need for row selection
const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
        console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
        //TODO: State
    },
    getCheckboxProps: record => ({
        disabled: record.status === true, // Column configuration not to be checked
        name: record.receiptnumber,
    }),
};

const InvoicesTable = () => {
    return (
        <div>
            <Divider />
            <Table
                bordered
                rowSelection={Object.assign({ type: 'checkbox' }, rowSelection)}
                columns={columns}
                dataSource={biggerData}
                pagination={{ pageSize: 10 }}
                //TODO:  onChange={handleTableChange}  // For sorting/filtering
            />
        </div>
    );
};
export default React.memo(InvoicesTable);