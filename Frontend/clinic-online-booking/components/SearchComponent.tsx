"use client";
import React, { useState } from "react";
import DoctorCard from "@/components/DoctorCard";
import { DOCTOR_API_URL } from "@lib/constants";
import Link from "next/link";

interface IDoctor {
  id: number;
  firstname: string;
  lastname: string;
  clinicId: number;
  clinic: string;
  specialityId: number;
  speciality: string;
}

const SearchComponent = () => {
  const [message, setMessage] = useState("");
  const [query, setQuery] = useState("");
  const [doctors, setDoctors] = useState([]);

  const handleSearch = async () => {
    setDoctors([]);
    try {
      const response = await fetch(`${DOCTOR_API_URL}/search/${query}`);
      if (!response.ok) {
        const DoctorJson = await response.json();
        if (response.status === 404) {
          setMessage(DoctorJson.message);
          return;
        }
        throw new Error("Failed to fetch data");
      }
      const data = await response.json();
      setDoctors(data);
      setMessage("");
    } catch {
      return;
    }
  };

  const handleInputChange = (e) => {
    setQuery(e.target.value);
  };

  return (
    <div className="w-full max-w-lg">
      <div className="flex mb-4">
        <input
          type="text"
          value={query}
          onChange={handleInputChange}
          placeholder="Search for a doctor..."
          className="flex-grow px-4 py-2 border rounded-md"
        />
        <button
          onClick={handleSearch}
          className="ml-2 px-4 py-2 text-white bg-blue-500 rounded-md hover:bg-blue-600"
        >
          Search
        </button>
      </div>
      <div className="grid gap-4">
        {doctors.map((doctor: IDoctor) => (
          <Link
            key={doctor.id}
            href={{
              pathname: `/clinic/${doctor.clinicId}`,
              query: { doctorId: doctor.id },
            }}
            passHref
          >
            <DoctorCard key={doctor.id} doctor={doctor} />
          </Link>
        ))}
      </div>
      {message && <div className="mt-4 text-red-500">{message}</div>}
    </div>
  );
};

export default SearchComponent;
