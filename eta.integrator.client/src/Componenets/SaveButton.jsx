import React from 'react'
import { SaveOutlined } from '@ant-design/icons';
import { Button } from 'antd'

const SaveButton = ({ loading, onSave }) => {
    return (
        <>
            <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: 16 }}>
                <Button
                    type='primary'
                    icon={<SaveOutlined />}
                    size='large'
                    onClick={onSave}
                    loading={loading}
                >
                    Save
                </Button>
            </div>
        </>
    )
}

export default SaveButton