import React, { useEffect } from "react";
import { Form, Input, Select, Space } from "antd";
import { CountriesCodes, SettingsValidationRules } from "../Constants/Constants";

const { Option } = Select;

const AddressForm = ({ isBusinessType, form }) => {
   useEffect(() => {
      if (!isBusinessType) {
         form.setFieldsValue({
            address: {
               branchId: "",
            },
         });
      }
   }, [isBusinessType, form]);
   return (
      <Form.Item
         label="Address"
         required
      >
         {/* Country */}
         <Space.Compact style={{ width: "100%", marginBottom: "10px" }}>
            <Form.Item
               name={["Address", "Country"]}
               rules={SettingsValidationRules.country}
               noStyle
            >
               <Select
                  disabled={true}
                  // defaultValue={"EG"}
                  placeholder="Country"
                  showSearch
                  optionFilterProp="children"
                  filterOption={(input, opt) =>
                     opt.children.toLowerCase().includes(input.toLowerCase())
                  }
               >
                  {CountriesCodes.map((country) => (
                     <Option
                        key={country.code}
                        value={country.code}
                     >
                        {country.Desc_en}
                     </Option>
                  ))}
               </Select>
            </Form.Item>

            {/* Governorate */}
            <Form.Item
               name={["Address", "Governate"]}
               rules={SettingsValidationRules.governorate}
               noStyle
            >
               {/* <Select
                  disabled={true}
                //   defaultValue={"cairo"}
                  placeholder="Governate/Province"
                  showSearch
                  optionFilterProp="children"
                  filterOption={(input, opt) =>
                     opt.children.toLowerCase().includes(input.toLowerCase())
                  }
               >
                  <Option value="cairo">Cairo</Option>
               </Select> */}
               <Input
                  disabled={false}
                  placeholder="Governate/Province"
               />
            </Form.Item>

            {/* Region/City */}
            <Form.Item
               name={["Address", "RegionCity"]}
               rules={SettingsValidationRules.region}
               noStyle
            >
               {/* <Select
                  disabled={true}
                  //   defaultValue={"cairo"}
                  placeholder="Region/City"
                  showSearch
                  optionFilterProp="children"
                  filterOption={(input, opt) =>
                     opt.children.toLowerCase().includes(input.toLowerCase())
                  }
               >
                  <Option value="cairo">Cairo</Option>
               </Select> */}
               <Input
                  disabled={false}
                  placeholder="Region/City"
               />
            </Form.Item>
         </Space.Compact>

         {/* Branch ID */}
         <Space.Compact style={{ width: "100%" }}>
            <Form.Item
               name={["Address", "BranchId"]}
               rules={SettingsValidationRules.branchId}
            >
               <Input
                  disabled={!isBusinessType}
                  placeholder="Branch ID"
               />
            </Form.Item>

            {/* Building Number */}
            <Form.Item
               name={["Address", "BuildingNumber"]}
               rules={SettingsValidationRules.buildingNumber}
            >
               <Input placeholder="Building Number" />
            </Form.Item>

            {/* Street Information */}
            <Form.Item
               name={["Address", "Street"]}
               rules={SettingsValidationRules.streetInfo}
               style={{ width: "100%" }}
            >
               <Input placeholder="Street Information" />
            </Form.Item>
         </Space.Compact>
      </Form.Item>
   );
};

export default AddressForm;
