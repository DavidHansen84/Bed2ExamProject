"use client";
import { CLINIC_API_URL } from "@/lib/constants";
import React, { useState, useEffect } from "react";
import styles from "./PageStyles";
import ClinicsCard from "./ClinicsCard";
import Link from "next/link";

interface IClinic {
  id: number;
  name: string;
  doctors: IDoctor[];
}

interface IDoctor {
  id: number;
  fullname: string;
  specialityId: number;
  speciality: string;
}

const Clinics = () => {
  const [clinics, setClinics] = useState<IClinic[]>([]);

  useEffect(() => {
    async function fetchClinics() {
      try {
        const response = await fetch(CLINIC_API_URL);
        if (!response.ok) {
          throw new Error("Failed to fetch clinics data");
        }
        const clinicsData = await response.json();
        setClinics(clinicsData);
      } catch (err) {
        console.error("Error fetching clinics:", err);
      }
    }

    fetchClinics();
  }, []);

  return (
    <div style={styles.Container}>
      {clinics.map((clinic: IClinic) => (
        <Link key={clinic.id} href={`/clinic/${clinic.id}`} passHref>
          <ClinicsCard title={clinic.name} doctors={clinic.doctors} />
        </Link>
      ))}
    </div>
  );
};

export default Clinics;
