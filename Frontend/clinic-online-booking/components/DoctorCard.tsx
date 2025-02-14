import React from "react";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";

interface Doctor {
  id: number;
  firstname: string;
  lastname: string;
  clinicId: number;
  clinic: string;
  specialityId: number;
  speciality: string;
}

interface IDoctor {
  doctor: Doctor;
}
const DoctorCard: React.FC<IDoctor> = ({ doctor }) => (
  <Card>
    <CardContent>
      <Typography variant="h5" component="div">
        {doctor.firstname} {doctor.lastname}
      </Typography>
      <Typography variant="body2" color="textSecondary">
        Speciality: {doctor.speciality}
      </Typography>
      <Typography variant="body2" color="textSecondary">
        Clinic: {doctor.clinic}
      </Typography>
    </CardContent>
  </Card>
);

export default DoctorCard;
