import React from "react";

export default function Doc() {
  return (
    <div style={{ width: "100%", height: "100vh" }}>
      <iframe
        src="http://localhost:5283/swagger/index.html"
        style={{ width: "100%", height: "100%", border: "none" }}
        title="Swagger API Documentation"
      />
    </div>
  );
}
