import { Flex, Tag } from "antd";

const TagsWrapper = ({ listOfElements, color = "green" }) => {
   const chunks = [];
   for (let i = 0; i < listOfElements.length; i += 5) {
      chunks.push(listOfElements.slice(i, i + 5));
   }
   return (
      <Flex
         vertical
         gap="small"
      >
         {chunks.map((row, rowIndex) => (
            <Flex key={rowIndex}>
               {row.map((element, index) => (
                  <Tag
                     key={index}
                     
                     color="white"
                     style={{
                        minWidth: "55px",
                        width: "auto",
                        textAlign: "center",
                        backgroundColor: color,
                        fontWeight: "bold",
                        padding:"0 8px"
                     }}
                  >
                     {element}
                  </Tag>
               ))}
            </Flex>
         ))}
      </Flex>
   );
};

export default TagsWrapper;
