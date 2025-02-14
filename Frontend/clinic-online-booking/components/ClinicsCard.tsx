"use client";
import React from "react";
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import styles from "./PageStyles";

interface IDoctor {
  id: number;
  fullname: string;
}

const ClinicsCard = ({
  title,
  doctors,
}: {
  title: string;
  doctors: IDoctor[];
}) => {
  return (
    <Card style={styles.clinic}>
      <CardContent>
        <Typography variant="h1" component="div">
          {title}
        </Typography>
        Doctors:
        {doctors.map((doctor: IDoctor) => (
          <Typography key={doctor.id} variant="body1" component="div">
            {doctor.fullname}
          </Typography>
        ))}
      </CardContent>
      <CardActions></CardActions>
    </Card>
  );
};

export default ClinicsCard;
