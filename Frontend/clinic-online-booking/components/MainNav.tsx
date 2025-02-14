"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";

const MainNav = () => {
  const pathname = usePathname();

  return (
    <nav>
      <Link
        href="/"
        passHref
        className={`hover:text-background px-2 transition duration-300 ${
          pathname === "/" ? "underline text-background-dark" : ""
        }`}
      >
        Home
      </Link>
      <Link
        href="/search"
        passHref
        className={`hover:text-background px-2 transition duration-300 ${
          pathname === "/" ? "underline text-background-dark" : ""
        }`}
      >
        Search Doctor
      </Link>

      {/* {pathname.startsWith("/admin") && <AdminNav />} */}
    </nav>
  );
};

export default MainNav;
