import moment, { Moment } from "moment";

interface IAppointment {
  id: number;
  date: string;
  clinicId: number;
  doctorId: number;
  patientId: number;
}

// ChatGTP helped me make this
export function generateAppointments(
  startTime: string,
  endTime: string,
  interval: number,
  today: Moment
) {
  const excludedTimes = ["11:00", "11:15", "11:30", "11:45"];
  const appointments = [];
  const currentTime = moment(startTime, "HH:mm");

  const endMoment = moment(endTime, "HH:mm");
  const exclusions = excludedTimes.map((time) =>
    moment(time, "HH:mm").format("HH:mm")
  );

  while (currentTime.isBefore(endMoment)) {
    const formattedTime = currentTime.format("HH:mm");
    if (!exclusions.includes(formattedTime)) {
      appointments.push({
        id: appointments.length + 1,
        date: `${today.format("YYYY-MM-DD")}T${formattedTime}:00`,
      });
    }
    currentTime.add(interval, "minutes");
  }

  return appointments;
}

export function getDoctorFreeAppointments(
  doctorId: number,
  occupiedAppointments: IAppointment[],
  today: Moment
) {
  const occupiedDates = occupiedAppointments
    .filter((appointment: IAppointment) => appointment.doctorId === doctorId)
    .map((appointment: IAppointment) => appointment.date);

  const baseAppointments = generateAppointments("09:00", "14:00", 15, today);

  return baseAppointments.filter((base) => !occupiedDates.includes(base.date));
}
