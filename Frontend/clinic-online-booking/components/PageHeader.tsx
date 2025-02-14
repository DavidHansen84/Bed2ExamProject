import React from "react";
import Link from "next/link";
import MainNav from "./MainNav";

const PageHeader = async () => {
  return (
    <header className="container h-full mx-auto flex justify-between items-center p-4">
      <div>
        <Link href="/">Clinic Online Booking</Link>
      </div>
      <MainNav />
    </header>
  );
};

export default PageHeader;
