const isUndefined = (value) => {
   return value === undefined || value === "undefined";
};

const isNullOrEmpty = (value) => {
   return value === null || value.trim().length === 0 ;
};

const isNullOrEmptyOrUndefined = (value) => {
   return isNullOrEmpty(value) || isUndefined(value);
};

export { isUndefined, isNullOrEmpty, isNullOrEmptyOrUndefined };
