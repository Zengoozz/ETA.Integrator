import React from 'react'

import { Button } from 'antd'

const CustomButton = ({ name = 'Submit', icon, loading, handleClick }) => {
    return (
        <>
            <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: 16 }}>
                <Button
                    type='primary'
                    icon={icon}
                    size='large'
                    onClick={handleClick}
                    loading={loading}
                    disabled={loading}
                >
                    {name}
                </Button>
            </div>
        </>
    )
}

export default CustomButton