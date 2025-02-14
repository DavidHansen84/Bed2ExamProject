import BookingForm from "@/components/BookingForm";

export default function Booking() {
  return (
    <main className="flex flex-col gap-8 row-start-2 items-center ">
      <h1>Book Appointment</h1>
      <BookingForm />
    </main>
  );
}
