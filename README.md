[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/eKcfG4oe)

![](http://images.restapi.co.za/pvt/Noroff-64.png)

# Noroff

# Back-end Development Year 2

### Exam Project 2

This repository does not have any startup code. Use the 2 folders

- Backend
- Frontend

for your respective applications.

Instruction for the course assignment is in the LMS (Moodle) system of Noroff.
[https://lms.noroff.no](https://lms.noroff.no)

![](http://images.restapi.co.za/pvt/ca_important.png)

You will not be able to make any submissions after the course assignment deadline. Make sure to make all your commit **BEFORE** the deadline to this repository.

![](http://images.restapi.co.za/pvt/help.png)

If you need help with any instructions for the course assignment, contact your teacher on **Microsoft Teams**.

**REMEMBER** Your Moodle LMS submission must have your repository link **AND** your Github username in the text file.

### INSTRUCTIONS OF USE

First Download the file and extract it. Then open it in VSCode or other Code editor.

BACKEND

NOTE: This application has a pre-input of doctors, specialities, clinics, patients, categories and appointments. - this for testing purposes.
If this is not wanted, delete or Comment out the PopulateDB call and method in Data/Context.cs

In mysql workbench setup a user for the database (or just use root to skip this part) to setup a new user.

1. Login to root.
2. Administration - Users and Privileges - Add account.
3. Add Login name and Password.
4. Under Schema Privileges press "Select "ALL"" and then Apply.

Then add a new Connection.

1. On the "front page" next to MySQL Connections press +.
2. Add a connection Name.
3. Hostname change to localhost or just leave it as is.
4. Type in the Username for the created user. (or root if you did not create a user).
5. Press "Test Connection" and if all is well press "OK".

#Instructions to run the application

1. Right click the Backend folder in VSCode and then "Open in Integrated Terminal".
2. Make shure to follow other instructions ( 1.Setup MySQL, 2. Connection String 3. Migrations ).
3. In the terminal type "dotnet run" (or dotnet watch).
4. Open the link that shows up in the terminal and add "/swagger" to see the endpoints and descriptions (watch opens it automatically).

#Instructions to create needed Migrations

1. In the terminal for the backend, write "dotnet ef migrations add -c DataContext Initial".
2. then "dotnet ef database update".
3. If the database is already created and it is not correct just enter "dotnet ef database drop" and then do 2. again.

#Connection String structure for MySQL Database connection

In the appsettings.json change the DATABASE, USERNAME AND PASSWORD.
"server=localhost;database=DATABASE;user=USERNAME;password=PASSWORD"

#Test

1. To run a test open a Backend.Test in a new terminal
2. Make shure the Backend is not running. (ctrl + c in the backend terminal will stop it)
3. type "dotnet test".

A test is then run and is supposed to Pass.

---

FRONTEND

How to run

1. In the Frontend folder in VSCode right click "clinic-online-booking" and "Open in Integrated Terminal".
2. Add a .env.local file to the root and add URL (example under)
3. In the Terminal write "npm install".
4. Then write "npm run dev". Or "npm run test" to run the test.
5. Open the link it provides to open the page. Usually "http://localhost:3000".

---

.env.local

NEXT_PUBLIC_BASE_API_URL="path to URL"

ex. NEXT_PUBLIC_BASE_API_URL=http://localhost:5283/api

### EXTERNAL LIBRARIES

MomentJS - https://momentjs.com/ - For handling the Dates

### ENDPOINTS

http://localhost:5283/api/appointment - Endpoint to handle GET and POST operations on the Appointments .
http://localhost:5283/api/appointment/{Id} - Endpoint to handle GET, PUT and DELETE on Appointment.

http://localhost:5283/api/category - Endpoint to handle GET and POST operations on Categories.
http://localhost:5283/api/category/{Id} - Endpoint to handle GET, PUT and DELETE on Categorie.

http://localhost:5283/api/clinic - Endpoint to handle GET and POST operations on Clinics.
http://localhost:5283/api/clinic/{Id} - Endpoint to handle GET, PUT and DELETE on Clinic.

http://localhost:5283/api/doctor - Endpoint to handle GET and POST operations on Doctors.
http://localhost:5283/api/doctor/{Id} - Endpoint to handle GET, PUT and DELETE on Doctor.
http://localhost:5283/api/doctor/search/{Query} - Endpoint handle GET to find a specific doctor based on firstname or lastname .

http://localhost:5283/api/patient - Endpoint to handle GET and POST operations on Patients.
http://localhost:5283/api/patient{Id} - Endpoint to handle GET, PUT and DELETE on Patient.
http://localhost:5283/api/patient/email/{Email} - Endpoint to handle GET to retrieve a patient with email.

http://localhost:5283/api/speciality - Endpoint to handle GET and POST operations on Specialities.
http://localhost:5283/api/speciality{id} - Endpoint to handle GET, PUT and DELETE on Speciality.

http://localhost:5283/swagger/index.html - Swagger documentation for the Backend.

http://localhost:3000/ - Main page to select clinic to make an appointment at a doctor - when clicking a clinic redirects to the specified clinic page.

http://localhost:3000/clinic/{ClinicId} - Clinic page for the clinic with that ID - shows a list of doctors that is associated to that clinic with thair free appointments. Clicking on an appointment redirects to the booking page.

http://localhost:3000/book/{bookingdate}?{ClinicId}&{DoctorId} - Booking page that a patient can book an appointment. Bookingdate is needed, that is selected from the clinic page. Doctor and clinic is already filled out but this can be changed. When filling out E-mail and the patient already exists with that E-mail, all patient info is automatically filled. If patient does not exist it creates the patient and the appointment.

http://localhost:3000/search - Search page that a patient can search for a doctor. Lists all doctors that fit the query, with their Clinic and Speciality. Click to come to the clinic page, where you can choose a time to book an appointment with only that doctor.

http://localhost:3000/doc - Working doc page that display the swagger documentation from "http://localhost:5283/swagger/index.html".

NOTE: the port "5283" and "3000" may vary depending on your setup. But when starting the application, it will show in the terminal.

### REFERENCES

https://stackoverflow.com/questions/34901593/how-to-filter-an-array-from-all-elements-of-another-array <-- had to check how to get only what I needed

---

https://learn.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5 ,
https://medium.com/@nile.bits/asp-net-mvc-understanding-the-purpose-of-data-transfer-objects-dtos-ad1e24caf5c9 <-- Had to learn more about DTOs to simplify and make it more efficient. Used both as info and a bit of exploring and testing I made it work.

---

https://stackoverflow.com/questions/42217121/how-to-start-search-only-when-user-stops-typing <-- Looked up how to delay the search after patient stopped typing

---

got help from chatGTP to make the generateAppointments function in the Frontend - components/GenerateAppointmentTimes.tsx

---

https://www.linkedin.com/learning/advanced-asp-dot-net-core-unit-testing <-- Got help here to setup a test but had to get help from chatGTP also to get it correct.

### PLANNING

WEEK 1:

- Make a rough plan
- Basic Database
- Basic Backend with all Endpoints
- Basic Frontend with appointments front page.
- Make shure it all can communicate together

WEEK 2:

- Reevaluate
- Upgrade Database as needed
- Upgrade Backend code as needed and add documentation. Make shure made appointments is stored in database.
- Upgrade Frontend as needed, make cards, buttons and input. Make shure appointments can be made.

WEEK 3:

- Reevaluate needs
- Upgrade Database and add instructions of use
- Upgrade Backend and add instructions of use
- Upgrade Frontend and add instructions of use
- Testing

WEEK 4:

- Reevaluate needs
- Upgrade Database add extra functionality
- Upgrade Backend add extra functionality
- Upgrade Frontend add extra functionality
- Extra functionality could be to add a Doctor page and add notes, Reseptionist page to make and delete custom (outside the standard time) appointments, patients and doctors,
  Admin page to add and delete Clinics, categories and specialities. Authication to login as reseptionist, doctor and admin.

WEEK 5 (FINAL WEEK):

- Make shure everything is working, all documentation is in order, make final adjustment, Final Commit and submit.

### TO FIX

BACKEND

- Backend API to show related data. Ex. doctors to show clinic name and speciality and not just ID. - fixed, with the use of DTOs. Else it was gonna be stuck in a loop.
- Backend validation to check if doctor belongs to clinic, doctor and patient can't have more than one appointment on the same date. - fixed, was tricky with PUT, but figured it out
- Error handling + messages - fixed
- Deleting Doctors deletes whole appointments, it should not. Check for other occurances of this. - fixed this for all
- Add User functionality for doc., resep. and admin. - Scrapped, not in assignment
- Add Authorizations for Users. - Scrapped, not in assignment

FRONTEND

- If clinic is choosen only show available doctors. - fixed
- Message on the book page to show a message when patient have booked appointment. - fixed
- If email is entered and patient exist the rest of patient info should be filled. - fixed
- If doctor name is pressed redirect to a doctor page with info about that doctor. And a button to come to the booking page with doctor and clinic prefilled. - fixed, decided it was best to redirect to the clinic page with all the appointment dates available for only that doctor.
- Show info about appointment when its created. - Fixed
- Make Appointment dates Selectable in bookingpage. - Scrapped.
- Let patient select day. Sat + Sun and appointments made outside of predefined times unbookable by patients. - fixed
- Doctor view page. Doc can see appointment info and add notes. - Scrapped, not in assignment
- Reseptionist view page. Reseptionist can make CRUD operations to patients, appointments and doctors. - Scrapped, not in assignment
- Admin view that can make CRUD operations to all. - Scrapped, not in assignment
- Login page. - Scrapped, not in assignment

### THOUGHTS

- Decided that the patient need to choose a clinic and then a doctor with available time. Then with that information book an appointment. Makes it easier and more structured for Patient.
- Decided to only have a Date on the appointments and specify that each appointment is 15 minutes in the booking page. This will make it more flexible for a recepionist or a doctor that need to squeese in an emergency appointment.
- Decided to scrap all extra ideas since they where not in the instructions. Added tests instead :)
