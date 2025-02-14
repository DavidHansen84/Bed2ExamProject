import React from "react";

const PageFooter = () => {
  const currentYear = new Date().getFullYear();
  return (
    <footer className="h-32 bg-background-dark w-full mx-auto flex justify-between items-center p-4">
      <p>© {currentYear} Made by David Hansen. All rights reserved</p>
    </footer>
  );
};

export default PageFooter;
