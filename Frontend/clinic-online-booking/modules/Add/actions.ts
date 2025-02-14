"use server";

import { PATIENT_API_URL, APPOINTMENTS_API_URL } from "@/lib/constants";

export async function bookPatient(
  prevState: {
    successMessage: string;
    failMessage: string;
    appointment: string;
  },
  formData: FormData
) {
  try {
    const firstname = formData.get("firstname");
    const lastname = formData.get("lastname");
    const email = formData.get("email");
    const birthdate = formData.get("birthdate");
    const clinicId = Number(formData.get("clinicId"));
    const doctorId = Number(formData.get("doctorId"));
    const categoryId = Number(formData.get("categoryId"));
    const dateData = formData.get("appointmentDate");
    const date = dateData.replace(" ", "T");
    const patientNote = formData.get("patientNote");

    const data = await fetch(`${PATIENT_API_URL}/email/${email}`);
    if (!data.ok) {
      const result = await registerPatient(
        firstname,
        lastname,
        email,
        birthdate,
        date,
        categoryId,
        clinicId,
        doctorId,
        patientNote
      );
      if (result.successMessage == "") {
        return {
          successMessage: "",
          failMessage: result.failMessage,
          appointment: "",
        };
      }
      return {
        successMessage: result.successMessage,
        failMessage: "",
        appointment: result.appointment,
      };
    } else {
      const patient = await data.json();
      const appointmentResponse = await registerAppointment(
        date,
        categoryId,
        clinicId,
        patient.id,
        doctorId,
        patientNote
      );
      if (appointmentResponse.successMessage == "") {
        return {
          successMessage: "",
          failMessage: appointmentResponse.failMessage,
          appointment: "",
        };
      }
      return {
        successMessage: appointmentResponse.successMessage,
        failMessage: "",
        appointment: appointmentResponse.appointment,
      };
    }
  } catch (error) {
    return {
      successMessage: "",
      failMessage: "Failed to create user: " + error,
      appointment: "",
    };
  }
}

export async function registerPatient(
  firstname: string,
  lastname: string,
  email: string,
  birthdate: Date,
  date: Date,
  categoryId: number,
  clinicId: number,
  doctorId: number,
  patientNote: string
) {
  try {
    const response = await fetch(PATIENT_API_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        firstname,
        lastname,
        email,
        birthdate,
      }),
    });
    const json = await response.json();
    if (!response.ok) {
      if (response.status === 400) {
        return {
          failMessage: json.message,
          successMessage: "",
          appointment: "",
        };
      } else {
        throw new Error(`API responded with status: ${response.status}`);
      }
    }

    const appointmentResponse = await registerAppointment(
      date,
      categoryId,
      clinicId,
      json.patient.id,
      doctorId,
      patientNote
    );

    return {
      patient: json,
      appointment: appointmentResponse.appointment,
      successMessage: `${json.message} And ${appointmentResponse.successMessage}`,
      failMessage: "",
    };
  } catch {
    return {
      failMessage: "Failed to create patient and appointment: ",
      successMessage: "",
      appointment: "",
    };
  }
}

export async function registerAppointment(
  date: Date,
  categoryId: number,
  clinicId: number,
  patientId: number,
  doctorId: number,
  patientNote: string
) {
  try {
    const response = await fetch(APPOINTMENTS_API_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        date,
        categoryId,
        clinicId,
        patientId,
        doctorId,
        patientNote,
      }),
    });
    if (!response.ok) {
      if (response.status === 400) {
        const { message } = await response.json();
        return { failMessage: message, successMessage: "", appointment: "" };
      } else {
        throw new Error(`API responded with status: ${response.status}`);
      }
    }
    const json = await response.json();
    return {
      successMessage: json.message,
      failMessage: "",
      appointment: json.appointment,
    };
  } catch (error) {
    return {
      failMessage: "Failed to create user" + "ERROR" + error,
      successMessage: "",
      appointment: "",
    };
  }
}
