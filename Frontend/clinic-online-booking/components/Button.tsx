import React from "react";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "add" | "edit" | "delete";

  ariaLabel?: string;
}

const Button: React.FC<ButtonProps> = ({
  variant = "add",
  children,
  className,
  ariaLabel,
  ...rest
}) => {
  const baseStyles =
    "px-4 py-2 font-medium text-white transition ease-in-out duration-150 focus:outline-none focus:ring-2 focus:ring-offset-2 rounded-lg";
  let variantStyles = "";
  let focusStyles = "";

  switch (variant) {
    case "add":
      variantStyles = "bg-green-500 hover:bg-green-600";
      focusStyles = "focus:ring-green-600";
      break;
    case "edit":
      variantStyles = "bg-yellow-500 hover:bg-yellow-600";
      focusStyles = "focus:ring-yellow-600";
      break;
    case "delete":
      variantStyles = "bg-red-500 hover:bg-red-600";
      break;
  }
  return (
    <button
      className={`${baseStyles} ${variantStyles} ${focusStyles} ${className}`}
      aria-label={ariaLabel}
      {...rest}
    >
      {children}
    </button>
  );
};

export default Button;
