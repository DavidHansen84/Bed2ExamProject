"use client";

import Appointments from "@/components/ClinicComponent";
import { useParams } from "next/navigation";
import React, { useState, useEffect } from "react";
import { CLINIC_API_URL } from "@/lib/constants";
import styles from "@/components/PageStyles";

interface IClinic {
  id: number;
  name: string;
}

export default function Clinic() {
  const [clinic, setClinic] = useState<{ id: number; name: string }[]>([]);

  const params = useParams();

  const clinicId = Number(params.id);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const clinicsResult = await fetch(CLINIC_API_URL);
        const clinicsData = await clinicsResult.json();
        const filteredClinic = clinicsData.filter(
          (clinic: IClinic) => clinic.id === clinicId
        );
        setClinic(filteredClinic);
      } catch (error) {
        console.error("Failed to fetch data", error);
      }
    };

    fetchData();
  }, [clinicId]);

  return (
    <>
      {clinic.length > 0 ? (
        <main className="flex flex-col gap-8 row-start-2 items-center ">
          <div style={styles.centerBig}>
            <h1>{clinic[0].name}</h1>
          </div>
          <h1>Book an appointment</h1>
          <p>Each appointment is 15 min</p>
          <Appointments />
        </main>
      ) : (
        <main className="flex flex-col gap-8 row-start-2 items-center ">
          <h1>Clinic does not exist</h1>
        </main>
      )}
    </>
  );
}
