import React, { useState } from 'react';
import { DatePicker, Button, Form, Row, Col, message } from 'antd';
// eslint-disable-next-line no-unused-vars
import dayjs from 'dayjs';
import InvoicesService from '../Services/InvoicesService';

const { RangePicker } = DatePicker;

const DateRangeSearch = () => {
  const [form] = Form.useForm();
  const [loading, _setLoading] = useState(false);

  const handleSearch = () => {
    form
      .validateFields()
      .then(values => {
        const { dateFrom, dateTo } = values;

        const formattedValues = {
          dateFrom: dateFrom ? dateFrom.format('YYYY-MM-DD') : null,
          dateTo: dateTo ? dateTo.format('YYYY-MM-DD') : null,
        };

        message.success(
          `Searching from ${formattedValues.dateFrom} to ${formattedValues.dateTo}`
        );
        // Call your API here
        var test = InvoicesService.getInvoicesAccordingToDateAsQueryParams(formattedValues);
        console.log(test);
      })
      .catch(() => {
        message.error('Please select valid dates.');
      });
  };

  return (
    <Form form={form} layout="inline">
      <Form.Item
        name="dateFrom"
        rules={[{ required: true, message: 'Please select start date' }]}
      >
        <DatePicker placeholder="From Date" />
      </Form.Item>
      <Form.Item
        name="dateTo"
        rules={[{ required: true, message: 'Please select end date' }]}
      >
        <DatePicker placeholder="To Date" />
      </Form.Item>
      <Form.Item>
        <Button type="primary" onClick={handleSearch} loading={loading}>
          Search
        </Button>
      </Form.Item>
    </Form>
  );
};

export default DateRangeSearch;
