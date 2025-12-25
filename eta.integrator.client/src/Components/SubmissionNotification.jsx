import { Flex, Tag } from "antd";

const SubmissionNotification = ({ response }) => {
   return (
      <Flex vertical>
         <p style={{ fontWeight: "bold", fontSize: "17px" }}>Submitted:</p>
         <TagsWrapper
            listOfElements={response.acceptedInvoices}
            color="green"
         />
         <p style={{ fontWeight: "bold", fontSize: "17px" }}>Rejected:</p>
         <TagsWrapper
            listOfElements={response.rejectedInvoices}
            color="red"
         />
      </Flex>
   );
};

export default SubmissionNotification;
