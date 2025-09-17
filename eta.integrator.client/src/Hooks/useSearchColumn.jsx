import { useState, useRef, useEffect } from "react";
import { Input, Button, Space } from "antd";
import { SearchOutlined } from "@ant-design/icons";

const useSearchColumn = (initialData) => {
   const [filteredData, setFilteredData] = useState(initialData);
   const [activeFilters, setActiveFilters] = useState({});
   const searchInput = useRef(null);

   useEffect(() => {
      setFilteredData(initialData || []);
   }, [initialData]);

   const getColumnSearchProps = (dataIndex, placeholder, caseSenestivity = false) => ({
      filterDropdown: ({ setSelectedKeys, selectedKeys, confirm, clearFilters }) => (
         <div style={{ padding: 8 }}>
            <Input
               ref={searchInput}
               placeholder={placeholder}
               value={selectedKeys[0]}
               onChange={(e) => setSelectedKeys(e.target.value ? [e.target.value] : [])}
               onPressEnter={() => handleSearch(selectedKeys, confirm, dataIndex)}
               style={{ marginBottom: 8, display: "block" }}
            />
            <Space>
               <Button
                  type="primary"
                  onClick={() => handleSearch(selectedKeys, confirm, dataIndex, caseSenestivity)}
                  icon={<SearchOutlined />}
                  size="small"
                  style={{ width: 90 }}
               >
                  Search
               </Button>
               <Button
                  onClick={() => handleReset(clearFilters, confirm, dataIndex, caseSenestivity)}
                  size="small"
                  style={{ width: 90 }}
               >
                  Reset
               </Button>
            </Space>
         </div>
      ),
      filterIcon: (filtered) => (
         <SearchOutlined style={{ color: filtered ? "#1890ff" : undefined }} />
      ),
      onFilter: (value, record) => {
         const recordValue = getValueByPath(record, dataIndex);
         return recordValue
            ? recordValue.toString().toLowerCase().includes(value.toLowerCase())
            : false;
      },
      render: (text) => text,
   });

   const getValueByPath = (obj, path) => {
      return path.split(".").reduce((acc, part) => acc && acc[part], obj);
   };

   const handleSearch = (selectedKeys, confirm, dataIndex, caseSenestivity) => {
      confirm();

      const newFilters = { ...activeFilters, [dataIndex]: selectedKeys[0] };
      setActiveFilters(newFilters);

      applyFilters(newFilters, caseSenestivity);
   };

   const handleReset = (clearFilters, confirm, dataIndex, caseSenestivity) => {
      clearFilters();

      const newFilters = { ...activeFilters };
      delete newFilters[dataIndex];
      setActiveFilters(newFilters);

      if (Object.keys(newFilters).length === 0) {
         setFilteredData(initialData);
      } else {
         applyFilters(newFilters, caseSenestivity);
      }
      confirm();
   };

   const applyFilters = (filters, caseSenestivity) => {
      let filtered = initialData;

      Object.keys(filters).forEach((key) => {
         const filterValue = caseSenestivity ? filters[key] : filters[key].toLowerCase();
         filtered = filtered.filter((item) =>
            caseSenestivity ? item[key]?.toString().includes(filterValue) : item[key]?.toString().toLowerCase().includes(filterValue)
         );
      });

      setFilteredData(filtered);
   };

   return { getColumnSearchProps, filteredData };
};

export default useSearchColumn;
