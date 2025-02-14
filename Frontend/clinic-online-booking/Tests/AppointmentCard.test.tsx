import React from "react";
import { render, screen } from "@testing-library/react";
import { AppointmentCard } from "@components/AppointmentsCard";

test("AppointmentCard", () => {
  render(<AppointmentCard date={new Date("2024-12-19T09:45:00")} />);
  expect(screen.getByText(/09:45/i)).toBeInTheDocument();
});
