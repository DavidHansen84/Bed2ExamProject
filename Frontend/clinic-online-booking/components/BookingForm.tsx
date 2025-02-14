"use client";

import { useParams, useSearchParams } from "next/navigation";
import { useActionState } from "react";
import { bookPatient } from "@modules/Add/actions";
import React, { useState, useEffect } from "react";
import Input from "./Input";
import Button from "./Button";
import Select from "./Select";
import {
  CATEGORY_API_URL,
  CLINIC_API_URL,
  DOCTOR_API_URL,
  PATIENT_API_URL,
} from "@/lib/constants";
import moment from "moment";
import styles from "./PageStyles";

interface IBookingFormProps {
  initialData?: {
    firstname: string;
    lastname: string;
    email: string;
    birthdate: Date;
    doctorId: number;
    clinicId: number;
    categoryId: number;
    date: Date;
  };
}

const initialState = {
  successMessage: "",
  failMessage: "",
  appointment: "",
};

const BookingForm: React.FC<IBookingFormProps> = ({ initialData }) => {
  const [doctorId, setDoctor] = useState(initialData?.doctorId || "");
  const [clinicId, setClinic] = useState(initialData?.clinicId || "");
  const [categoryId, setCategory] = useState(initialData?.categoryId || "");
  const [doctors, setDoctors] = useState<
    { id: number; firstname: string; lastname: string; clinicId: string }[]
  >([]);
  const [clinics, setClinics] = useState<{ id: number; name: string }[]>([]);
  const [categories, setCategories] = useState<{ id: number; name: string }[]>(
    []
  );
  const [email, setEmail] = useState("");
  const [patientFirstname, setFirstname] = useState("");
  const [patientLastname, setLastname] = useState("");
  const [patientBirthdate, setBirthdate] = useState("");
  const [PatientNote, setPatientNote] = useState("");
  const [state, formAction] = useActionState(bookPatient, initialState);

  const searchParams = useSearchParams();
  const clinicIdParam = searchParams.get("clinicId");
  const doctorsIdParam = searchParams.get("doctorId");

  useEffect(() => {
    if (clinicIdParam) {
      setClinic(clinicIdParam);
    }
  }, [clinicIdParam]);

  useEffect(() => {
    if (doctorsIdParam) {
      setDoctor(doctorsIdParam);
    }
  }, [doctorsIdParam]);

  // Had to lookup the delay of search - https://stackoverflow.com/questions/42217121/how-to-start-search-only-when-user-stops-typing
  useEffect(() => {
    if (email) {
      const delayTimer = setTimeout(() => {
        const fetchPatient = async () => {
          try {
            const patientResult = await fetch(
              `${PATIENT_API_URL}/email/${email}`
            );
            if (patientResult.ok) {
              const patientData = await patientResult.json();
              patientData.birthdate = moment(patientData.birthdate).format(
                "YYYY-MM-DD"
              );
              setFirstname(patientData.firstname);
              setLastname(patientData.lastname);
              setBirthdate(patientData.birthdate);
            }
          } catch (error) {
            console.error("Failed to fetch data", error);
          }
        };

        fetchPatient();
      }, 1000);
      return () => clearTimeout(delayTimer);
    }
  }, [email]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [doctorsResult, clinicsResult, categoryResult] =
          await Promise.all([
            fetch(DOCTOR_API_URL),
            fetch(CLINIC_API_URL),
            fetch(CATEGORY_API_URL),
          ]);
        const doctorsData = await doctorsResult.json();
        const clinicsData = await clinicsResult.json();
        const categoryData = await categoryResult.json();
        setDoctors(doctorsData);
        setClinics(clinicsData);
        setCategories(categoryData);
      } catch (error) {
        console.error("Failed to fetch data", error);
      }
    };

    fetchData();
  }, []);

  const params: string = useParams();

  const formattedDate: string = params.date
    .replace("T", " ")
    .replaceAll("%3A", ":");
  return (
    <div>
      <form
        name="appointmentForm"
        action={formAction}
        className="max-w-md mx-auto"
      >
        <h1>Date</h1>
        <Input
          id="appointmentDate"
          type="text"
          name="appointmentDate"
          value={formattedDate}
          readOnly
        />
        <h1>Patient Info</h1>
        <Input
          id="email"
          type="email"
          name="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="Your email"
          autoComplete="email"
          required
        />
        <Input
          id="firstname"
          type="text"
          name="firstname"
          value={patientFirstname}
          onChange={(e) => setFirstname(e.target.value)}
          placeholder="Your firstname"
          autoComplete="firstname"
          required
        />
        <Input
          id="lastname"
          type="lastname"
          name="lastname"
          value={patientLastname}
          onChange={(e) => setLastname(e.target.value)}
          placeholder="Your lastname"
          autoComplete="lastname"
          required
        />
        <Input
          id="birthdate"
          type="date"
          name="birthdate"
          value={patientBirthdate}
          onChange={(e) => setBirthdate(e.target.value)}
          placeholder="Your birthdate"
          autoComplete="birthdate"
          required
        />
        <h1>Appointment Details</h1>
        <Select
          id="clinicId"
          name="clinicId"
          value={clinicId}
          onChange={(e) => setClinic(e.target.value)}
          className="input"
          required
        >
          <option value="">Select a clinic</option>
          {clinics.map((clinic) => (
            <option key={clinic.id} value={clinic.id}>
              {clinic.name}
            </option>
          ))}
        </Select>

        <Select
          id="doctorId"
          name="doctorId"
          typeof="number"
          value={doctorId}
          onChange={(e) => setDoctor(e.target.value)}
          className="input"
          required
        >
          <option value="">Select a doctor</option>
          {clinicId === ""
            ? doctors.map((doctor) => (
                <option key={doctor.id} value={doctor.id}>
                  {doctor.firstname + " " + doctor.lastname}
                </option>
              ))
            : doctors
                .filter((doctor) => doctor.clinicId == clinicId)
                .map((doctor) => (
                  <option key={doctor.id} value={doctor.id}>
                    {doctor.firstname + " " + doctor.lastname}
                  </option>
                ))}
        </Select>

        <Select
          id="categoryId"
          name="categoryId"
          typeof="number"
          value={categoryId}
          onChange={(e) => setCategory(e.target.value)}
          className="input"
          required
        >
          <option value="">Select a category</option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </Select>
        <p>Optional</p>
        <textarea
          id="patientNote"
          type="patientNote"
          name="patientNote"
          value={PatientNote}
          onChange={(e) => setPatientNote(e.target.value)}
          placeholder="Note to doctor... max 150 letters"
          autoComplete="off"
          maxLength={150}
          style={styles.inputNote}
        />
        <div>
          <p>Booking an appointment requires us to save your info.</p>
          <p>
            <input type="checkbox" required></input> Check this box to agree
          </p>
        </div>

        <Button type="submit">Book Appointment</Button>
        {state?.successMessage && (
          <div className="mt-4 text-green-500">{state.successMessage}</div>
        )}
        {state?.failMessage && (
          <div className="mt-4 text-red-500">{state.failMessage}</div>
        )}
        {state?.appointment && (
          <div className="mt-4 text-black-500">
            <p>
              Appointment Date:{" "}
              {moment(state.appointment.date).format("YYYY-MM-DD")}
            </p>
            <p>At time: {moment(state.appointment.date).format("HH.mm")}</p>
            <p>At {state.appointment.clinic}</p>
            <p>With Dr. {state.appointment.doctor}</p>
            <p>Category: {state.appointment.category}</p>
            <p>Note: {state.appointment.patientNote}</p>
          </div>
        )}
      </form>
    </div>
  );
};

export default BookingForm;
