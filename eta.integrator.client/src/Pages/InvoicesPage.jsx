import React from 'react'
import { SaveOutlined } from '@ant-design/icons';
import { Button } from 'antd'

import InvoicesTable from '../Componenets/InvoicesTable'



const InvoicesPage = () => {

  const onSave = () => {

  }

  return (
    <>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: 16 }}>
        <Button
          type='primary'
          icon={<SaveOutlined />}
          size='large'
          onClick={onSave}
        >
          Save
        </Button>
      </div>
      <InvoicesTable />
    </>
  )
}

export default InvoicesPage