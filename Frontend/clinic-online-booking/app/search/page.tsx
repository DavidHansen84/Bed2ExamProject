import SearchComponent from "@/components/SearchComponent";

export default function Home() {
  return (
    <main className="flex flex-col items-center gap-8">
      <h1 className="text-2xl font-bold">Search for a Doctor</h1>
      <SearchComponent />
    </main>
  );
}
