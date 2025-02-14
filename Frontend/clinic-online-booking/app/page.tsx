import Clinics from "@/components/HomePage";

export default function Home() {
  return (
    <main className="flex flex-col gap-8 row-start-2 items-center ">
      <h1>Chose one of our Online Clinics to book an Appointment</h1>
      <Clinics />
    </main>
  );
}
