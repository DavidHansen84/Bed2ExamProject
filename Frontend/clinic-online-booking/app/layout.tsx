import type { Metadata } from "next";
import "./globals.css";
import PageHeader from "@/components/PageHeader";

import { Inter, Montserrat } from "next/font/google";
import PageFooter from "@/components/PageFooter";

export const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });
export const montserrat = Montserrat({
  subsets: ["latin"],
  variable: "--font-montserrat",
});

export const metadata: Metadata = {
  title: "Online Clinic Bookings",
  description: "Books Clinic appointments Online",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={`${inter.variable} ${montserrat.variable}`}>
      <body className="bg-background h-screen m-0">
        <div className="flex flex-col h-full">
          <div className="h-20 bg-background-light">
            <PageHeader />
          </div>
          <div className="flex-1">
            <div className="w-full h-full mx-auto p-4">{children}</div>
          </div>
          <footer className="h-25 bg-background-dark">
            <PageFooter />
          </footer>
        </div>
      </body>
    </html>
  );
}
