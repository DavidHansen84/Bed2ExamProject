import React from "react";
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import styles from "./PageStyles";

interface IDate {
  date: Date;
}
const AppointmentCard: React.FC<IDate> = ({ date }) => {
  const time = new Date(date).toLocaleTimeString([], {
    hour: "2-digit",
    minute: "2-digit",
  });
  return (
    <Card style={styles.style}>
      <CardContent>
        <Typography variant="h5" component="div">
          {time}
        </Typography>
      </CardContent>
      <CardActions></CardActions>
    </Card>
  );
};

const OccupiedAppointmentCard: React.FC<IDate> = ({ date }) => {
  const time = new Date(date).toLocaleTimeString([], {
    hour: "2-digit",
    minute: "2-digit",
  });

  return (
    <Card style={styles.style2}>
      <CardContent>
        <Typography variant="h5" component="div">
          {time}
        </Typography>
      </CardContent>
      <CardActions></CardActions>
    </Card>
  );
};

export { AppointmentCard, OccupiedAppointmentCard };
