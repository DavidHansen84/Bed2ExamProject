import React from "react";
import { render, screen } from "@testing-library/react";
import PageFooter from "@components/PageFooter";

test("PageFooter", () => {
  render(<PageFooter />);
  expect(screen.getByText(/Made by David Hansen/i)).toBeInTheDocument();
});
