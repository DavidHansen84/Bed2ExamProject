import React from "react";

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  variant?: "default" | "error" | "success";
  message?: string;
  id: string;
}

const Select: React.FC<SelectProps> = ({
  label,
  variant = "default",
  message,
  id,
  className,
  children,
  ...rest
}) => {
  const baseStyles =
    "block w-full px-4 py-2 mt-2 border rounded-md focus:border-blue-500 focus:outline-none focus:ring";
  let variantStyles = "";

  switch (variant) {
    case "default":
      variantStyles = "border-gray-300";
      break;
    case "error":
      variantStyles = "border-red-500 text-red-600";
      break;
    case "success":
      variantStyles = "border-green-500 text-green-600";
      break;
  }

  return (
    <div className="mb-4">
      {label && (
        <label htmlFor={id} className="block text-sm font-medium text-gray-700">
          {label}
        </label>
      )}
      <select
        id={id}
        className={`${baseStyles} ${variantStyles} ${className}`}
        aria-invalid={variant === "error" ? true : undefined}
        aria-describedby={message ? `${id}-message` : undefined}
        {...rest}
      >
        {children}
      </select>
      {message && (
        <p
          id={`${id}-message`}
          className={`mt-2 text-sm ${
            variant === "error" ? "text-red-600" : "text-green-600"
          }`}
        >
          {message}
        </p>
      )}
    </div>
  );
};

export default Select;
