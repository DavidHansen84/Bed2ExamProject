using OnlineClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinicBooking.Data;

namespace OnlineClinicBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PatientController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PatientController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private bool PatientExistsId(int id)
        {
            return (_dataContext.Patients?.Any(Patient => Patient.Id == id)).GetValueOrDefault();
        }
        private bool PatientExistsEmail(string email)
        {
            return (_dataContext.Patients?.Any(Patient => Patient.Email == email)).GetValueOrDefault();
        }
        private bool IsValidBirthdate(DateTime birthdate)
        {
            var today = DateTime.Today;
            var minValidDate = today.AddYears(-120);
            var maxValidDate = today;

            return birthdate >= minValidDate && birthdate <= maxValidDate;
        }
        private bool IsPatientInUse(int id)
        {
            return (_dataContext.Appointments?.Any(Appointment => Appointment.PatientId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Retrieves all Patients.
        /// </summary>
        ///  <response code="200">Returns All the Patients</response>
        /// <response code="400">Error getting Patients</response>
        //GET api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            try
            {
                if (_dataContext.Patients == null)
                {
                    return NotFound(new { message = $"Patients database not found!" });
                }

                var patientsData = await _dataContext.Patients.Include(a => a.Appointments)
                            .ThenInclude(a => a.Category)
                        .Include(a => a.Appointments)
                            .ThenInclude(p => p.Doctor)
                        .Include(a => a.Appointments)
                            .ThenInclude(c => c.Clinic).ToListAsync();
                if (patientsData == null)
                {
                    return NotFound(new { message = $"Patients Data not found." });
                }
                var patientsDTO = patientsData.Select(p => new PatientDTO
                {
                    Id = p.Id,
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                    Email = p.Email,
                    Birthdate = p.Birthdate,
                    Appointments = p.Appointments.Select(a => new PatientAppointmentDTO
                    {
                        Date = a.Date,
                        CategoryId = a.CategoryId,
                        Category = a.Category.Name,
                        DoctorId = a.DoctorId,
                        Doctor = $"{a.Doctor.Firstname} {a.Doctor.Lastname}",
                        ClinicId = a.ClinicId,
                        Clinic = a.Clinic.Name,
                    }).ToList(),
                }).ToList();

                return Ok(patientsDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = $"An error occurred.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves Patient with id.
        /// </summary>
        ///  <response code="200">Returns The requested Patient</response>
        /// <response code="400">Error getting Patient</response>
        /// <response code="404">Patient does not exist</response>
        //GET api/patients/{id}
        [HttpGet("{Id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int Id)
        {
            try
            {

                if (_dataContext.Patients == null)
                {
                    return NotFound(new { message = $"Patients not found." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!PatientExistsId(Id))
                {
                    return NotFound(new { message = $"Patient with ID {Id} not found." });
                }
                var patientData = await _dataContext.Patients.Include(a => a.Appointments)
                            .ThenInclude(a => a.Category)
                        .Include(a => a.Appointments)
                            .ThenInclude(p => p.Doctor)
                        .Include(a => a.Appointments)
                            .ThenInclude(c => c.Clinic).FirstOrDefaultAsync(a => a.Id == Id);
                if (patientData == null)
                {
                    return NotFound(new { message = $"Patient Data not found." });
                }
                var patientDTO = new PatientDTO
                {
                    Id = patientData.Id,
                    Firstname = patientData.Firstname,
                    Lastname = patientData.Lastname,
                    Email = patientData.Email,
                    Birthdate = patientData.Birthdate,
                    Appointments = patientData.Appointments.Select(a => new PatientAppointmentDTO
                    {
                        Date = a.Date,
                        CategoryId = a.CategoryId,
                        Category = a.Category.Name,
                        DoctorId = a.DoctorId,
                        Doctor = $"{a.Doctor.Firstname} {a.Doctor.Lastname}",
                        ClinicId = a.ClinicId,
                        Clinic = a.Clinic.Name,
                    }).ToList(),
                };

                return Ok(patientDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = $"An error occurred.",
                    error = ex.Message
                });
            }
        }
        /// <summary>
        /// Retrieves Patient with email.
        /// </summary>
        /// <response code="200">Returns The requested Patient</response>
        /// <response code="400">Error getting Patient</response>
        /// <response code="404">Patient does not exist</response>
        //GET api/patients/email/{email}
        [HttpGet("email/{Email}")]
        public async Task<ActionResult<PatientDTO>> GetPatientEmail(string Email)
        {
            try
            {
                if (_dataContext.Patients == null)
                {
                    return NotFound(new { message = $"Patients not found." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!PatientExistsEmail(Email))
                {
                    return BadRequest(new { message = "No patients with this email" });
                }
                var patientData = await _dataContext.Patients.Include(a => a.Appointments)
                            .ThenInclude(a => a.Category)
                        .Include(a => a.Appointments)
                            .ThenInclude(p => p.Doctor)
                        .Include(a => a.Appointments)
                            .ThenInclude(c => c.Clinic).FirstOrDefaultAsync(a => a.Email == Email);

                if (patientData == null)
                {
                    return NotFound(new { message = $"Patient Data not found." });
                }
                var patientDTO = new PatientDTO
                {
                    Id = patientData.Id,
                    Firstname = patientData.Firstname,
                    Lastname = patientData.Lastname,
                    Email = patientData.Email,
                    Birthdate = patientData.Birthdate,
                    Appointments = patientData.Appointments.Select(a => new PatientAppointmentDTO
                    {
                        Date = a.Date,
                        CategoryId = a.CategoryId,
                        Category = a.Category.Name,
                        DoctorId = a.DoctorId,
                        Doctor = $"{a.Doctor.Firstname} {a.Doctor.Lastname}",
                        ClinicId = a.ClinicId,
                        Clinic = a.Clinic.Name,
                    }).ToList(),
                };

                return Ok(patientDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = $"An error occurred.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Add a new Patient.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":0,
        ///        "Firstname": "Kari",
        ///        "Lastname": "Normann",
        ///        "Email": "test@mail.com",
        ///        "Birthdate": "2000-11-25"
        ///     }
        /// </remarks>
        /// <response code="201">Patient added</response>
        /// <response code="400">If the Patient is null</response>
        // POST api/Patients
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientAddDTO>> AddPatient(Patient patient)
        {
            try
            {
                if (PatientExistsEmail(patient.Email))
                {
                    return BadRequest(new { message = "Patient with this Email already exist" });
                }
                if (PatientExistsId(patient.Id))
                {
                    return BadRequest(new { message = "Patient with this ID already exist, use 0" });
                }
                if (!IsValidBirthdate(patient.Birthdate))
                {
                    return BadRequest(new { message = "Birthdate formate has to be yyyy-MM-dd. Can't be older that 120 years or born later than today" });
                }
                _dataContext.Patients.Add(patient);

                await _dataContext.SaveChangesAsync();

                var patientDTO = new PatientAddDTO
                {
                    Id = patient.Id,
                    Firstname = patient.Firstname,
                    Lastname = patient.Lastname,
                    Email = patient.Email,
                    Birthdate = patient.Birthdate,
                };
                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, new
                {
                    message = $"Patient {patientDTO.Firstname} created",
                    Patient = patientDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = $"An error occurred.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates a Patient with the id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":5,
        ///        "Firstname": "Kari",
        ///        "Lastname": "Normann",
        ///        "Email": "test@mail.com",
        ///        "Birthdate": "2000-11-26"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the Updated patient</response>
        /// <response code="404">If the patient is not found</response>
        /// <response code="400">If the patient is null</response>
        //PUT api/Patients/{id}
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientDTO>> UpdatePatient(int Id, PatientUpdateDTO patient)
        {
            try
            {
                if (Id != patient.Id)
                {
                    return BadRequest("Patient Id and Params Id need to be the same.");
                }
                if (!PatientExistsId(Id))
                {
                    return NotFound(new { message = $"Appointment with ID {Id} not found." });
                }

                var patientData = await _dataContext.Patients
                        .FirstOrDefaultAsync(a => a.Id == Id);

                // If not all values is entered, use values from database
                patient.Birthdate = patient.Birthdate != default ? patient.Birthdate : patientData.Birthdate;
                patient.Firstname = !string.IsNullOrEmpty(patient.Firstname) ? patient.Firstname : patientData.Firstname;
                patient.Lastname = !string.IsNullOrEmpty(patient.Lastname) ? patient.Lastname : patientData.Lastname;
                patient.Email = !string.IsNullOrEmpty(patient.Email) ? patient.Email : patientData.Email;

                if (!IsValidBirthdate(patient.Birthdate))
                {
                    return BadRequest("Birthdate formate has to be yyyy-MM-dd. Can't be older that 120 years or born later than today");
                }
                if (patientData.Email != patient.Email)
                {
                    if (PatientExistsEmail(patient.Email))
                    {
                        return BadRequest("Patient with this email already exist");
                    }
                }

                var oldPatientDTO = new PatientAddDTO
                {
                    Id = patientData.Id,
                    Firstname = patientData.Firstname,
                    Lastname = patientData.Lastname,
                    Email = patientData.Email,
                    Birthdate = patientData.Birthdate,
                };

                _dataContext.Entry(patientData).CurrentValues.SetValues(patient);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                var patientDTO = new PatientAddDTO
                {
                    Id = patientData.Id,
                    Firstname = patientData.Firstname,
                    Lastname = patientData.Lastname,
                    Email = patientData.Email,
                    Birthdate = patientData.Birthdate,
                };
                // Return old and new patient data
                return Ok(new
                {
                    message = $"Patient '{patientDTO.Id}', '{patientDTO.Firstname} {patientDTO.Lastname}' updated successfully.",
                    oldData = oldPatientDTO,
                    newData = patientDTO
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = $"An error occurred.",
                    error = ex.Message
                });
            }

        }

        /// <summary>
        /// Deletes a Patient.
        /// </summary>
        /// <response code="200">Patient Deleted</response>
        /// <response code="404">Could not find Patient to delete</response>
        //DELETE api/Patient/{id}
        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> DeletePatient(int Id)
        {
            try
            {
                if (_dataContext.Patients == null)
                {
                    return NotFound(new { message = $"Patient database not found!" });
                }
                if (!PatientExistsId(Id))
                {
                    return NotFound(new { message = $"Patient with ID {Id} not found." });
                }
                if (IsPatientInUse(Id))
                {
                    return BadRequest(new
                    {
                        message = $"Patient with ID {Id} is in use! Remove or change from Appointments"
                    });
                }

                var patient = await _dataContext.Patients.FindAsync(Id);

                Console.WriteLine("Patient " + patient.Firstname + " " + patient.Lastname + " deleted");
                _dataContext.Patients.Remove(patient);
                await _dataContext.SaveChangesAsync();
                // Return a message with details about the deleted patient
                return Ok(new { message = $"Patient '{patient.Id}', '{patient.Firstname} {patient.Lastname}' deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = $"An error occurred.",
                    error = ex.Message
                });
            }
        }
    }
}