import React from 'react';
import { Layout, Typography, Space } from 'antd';
import { SunOutlined, MoonOutlined } from '@ant-design/icons';

const { Header } = Layout;
const { Title } = Typography;

const Navbar = ({ colorMode, setColorMode }) => {

    const toggleColorMode = () => {
        setColorMode(!colorMode);
    };

    return (
        <Header style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '0 24px',
            position: 'sticky',
            top: 0,
            zIndex: 1,
            width: '100%',
            // background: '#001529', // Default AntD dark header
        }}>
            <Title level={3} style={{ color: '#fff', margin: 0 }}>
                Global Soft
            </Title>

            <Space>
                {
                    colorMode ?
                        (
                            <SunOutlined onClick={toggleColorMode} style={{ fontSize: '20px', color: '#fff', cursor: 'pointer' }} />
                        ) :
                        (
                            <MoonOutlined onClick={toggleColorMode} style={{ fontSize: '20px', color: '#fff', cursor: 'pointer' }} />
                        )
                }

            </Space>
        </Header>
    );
};

export default Navbar;
