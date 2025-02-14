"use client";

import { useParams, useSearchParams } from "next/navigation";
import React, { useState, useEffect } from "react";
import { APPOINTMENTS_API_URL, DOCTOR_API_URL } from "@/lib/constants";
import { AppointmentCard, OccupiedAppointmentCard } from "./AppointmentsCard";
import Link from "next/link";
import styles from "./PageStyles";
import moment from "moment";
import { getDoctorFreeAppointments } from "@components/GenerateAppointmentTimes";

interface IAppointment {
  id: number;
  date: string;
  clinicId: number;
  doctorId: number;
  patientId: number;
}

interface IDoctor {
  id: number;
  firstname: string;
  lastname: string;
  clinicId: number;
  clinic: string;
  specialityId: number;
  speciality: string;
}

const Appointments = () => {
  const [occupiedAppointments, setOccupiedAppointments] = useState<
    IAppointment[]
  >([]);
  const [doctors, setDoctors] = useState<IDoctor[]>([]);
  const [today, setToday] = useState(moment());

  const day = today.format("dddd");

  useEffect(() => {
    async function fetchAppointments() {
      try {
        const response = await fetch(APPOINTMENTS_API_URL);
        if (!response.ok) {
          throw new Error("Failed to fetch appointments data");
        }
        const occupiedAppointmentsData = await response.json();

        const todaysOccupiedAppointments = occupiedAppointmentsData.filter(
          (appointment: IAppointment) =>
            moment(appointment.date).format("YYYY-MM-DD") ===
            today.format("YYYY-MM-DD")
        );

        setOccupiedAppointments(todaysOccupiedAppointments);
      } catch (err) {
        console.error("Error fetching appointments:", err);
      }
    }

    fetchAppointments();
  }, [today]);

  const params = useParams();
  const clinicId = Number(params.id);
  const searchParams = useSearchParams();
  const doctorsIdParam = Number(searchParams.get("doctorId"));

  useEffect(() => {
    async function fetchDoctors() {
      try {
        const response = await fetch(DOCTOR_API_URL);
        if (!response.ok) {
          throw new Error("Failed to fetch doctors data");
        }
        const doctorsData = await response.json();

        const filteredDoctors = doctorsIdParam
          ? doctorsData.filter(
              (doctor: IDoctor) => doctor.id === doctorsIdParam
            )
          : doctorsData.filter(
              (doctor: IDoctor) => doctor.clinicId === clinicId
            );

        setDoctors(filteredDoctors);
      } catch (err) {
        console.error("Error fetching doctors:", err);
      }
    }

    fetchDoctors();
  }, [clinicId, doctorsIdParam]);

  const handleAdd = () => {
    const nextday = moment(today).add(1, "days");
    setToday(nextday);
  };

  return (
    <>
      <div>
        <h1> {day}</h1>
        <button
          className="ml-2 px-4 py-2 text-white bg-blue-500 rounded-md hover:bg-blue-600"
          onClick={handleAdd}
        >
          Next day
        </button>
      </div>

      {day !== "Sunday" && day !== "Saturday" ? (
        <div>
          {doctors.length > 0 ? (
            doctors.map((doctor) => {
              const doctorFreeAppointments = getDoctorFreeAppointments(
                doctor.id,
                occupiedAppointments,
                today
              );
              const doctorOccupiedAppointments = occupiedAppointments.filter(
                (appointment) => appointment.doctorId === doctor.id
              );

              return (
                <div key={doctor.id} style={styles.doctor}>
                  <div style={styles.centerBig}>
                    <h1>
                      Dr. {doctor.firstname} {doctor.lastname}
                    </h1>
                  </div>

                  <h4>Free Appointments</h4>
                  <div style={styles.Container}>
                    {doctorFreeAppointments.length > 0 ? (
                      doctorFreeAppointments.map((appointment) => (
                        <Link
                          key={appointment.id}
                          href={{
                            pathname: `/book/${appointment.date}`,
                            query: { clinicId: clinicId, doctorId: doctor.id },
                          }}
                          passHref
                        >
                          <AppointmentCard date={appointment.date} />
                        </Link>
                      ))
                    ) : (
                      <p>No free appointments available</p>
                    )}
                  </div>

                  <h4>Occupied Appointments</h4>
                  <div style={styles.Container}>
                    {doctorOccupiedAppointments.length > 0 ? (
                      doctorOccupiedAppointments.map((appointment) => (
                        <OccupiedAppointmentCard
                          key={appointment.id}
                          date={appointment.date}
                        />
                      ))
                    ) : (
                      <p>No occupied appointments</p>
                    )}
                  </div>
                </div>
              );
            })
          ) : (
            <p>No doctors available for this clinic.</p>
          )}
        </div>
      ) : (
        <div>No Appointments on Saturday or Sunday</div>
      )}
    </>
  );
};

export default Appointments;
