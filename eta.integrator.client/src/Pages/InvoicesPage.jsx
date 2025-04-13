import React, { useState } from "react";
import { Flex, Card } from "antd";

import InvoicesTable from "../Components/InvoicesTable";
import SaveButton from "../Components/SaveButton";
import DateRangeSearch from "../Components/DateRangeSearch";

const InvoicesPage = ({ isMobile }) => {
   const [loading, setLoading] = useState(false);

   const onSave = async () => {
      setLoading(true); // Start loading
      //TODO: Simulate save operation (e.g., API call)
      await new Promise((resolve) => setTimeout(resolve, 2000)); // 2 seconds delay
      setLoading(false); // End loading
   };

   return (
      <Card style={{ width: "100%" }}>
         <Flex
            vertical
            gap="middle"
         >
            <DateRangeSearch isMobile={isMobile} />
            <SaveButton
               loading={loading}
               onSave={onSave}
               style={{ alignSelf: "flex-end" }}
            />
            <InvoicesTable isMobile={isMobile} />
         </Flex>
      </Card>
   );
};

export default InvoicesPage;
