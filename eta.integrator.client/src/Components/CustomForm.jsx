import { useEffect, cloneElement } from "react";
import { Form } from "antd";

const CustomForm = ({
   name = null,
   layout = "vertical",
   isMobile,
   initialValues = null,
   formItems,
   handleSubmit,
   handleSubmitFailure = null,
}) => {
   const [form] = Form.useForm();

   useEffect(() => {
      if (initialValues) {
         form.setFieldsValue(initialValues);
      }
   }, [form, initialValues]);

   const onButtonClick = async () => {
      var values = form.getFieldsValue();
      await handleSubmit(values);
   }
   
   const items = formItems.Elements.map((item, index) => {
      const labelProps = item.label ? { label: item.label } : {};
      const rules = item.rules ? { rules: item.rules } : {};
      return item.showItem && (
         <Form.Item
            key={index}
            name={item.name}
            {...labelProps}
            {...rules}
         >
            {item.element}
         </Form.Item>
      );
   });

   const wrapper = formItems.WrapperElement;

   const wrappedContent = wrapper ? cloneElement(wrapper, {}, items) : items;

   return (
      <Form
         form={form}
         layout={layout}
         name={name}
         labelCol={{ span: isMobile ? 24 : 10 }}
         wrapperCol={{ span: isMobile ? 24 : 100 }}
         style={{ width: "100%" }}
         onFinish={onButtonClick}
         onFinishFailed={handleSubmitFailure}
         requiredMark="optional"
      >
         {wrappedContent}
      </Form>
   );
};

export default CustomForm;
