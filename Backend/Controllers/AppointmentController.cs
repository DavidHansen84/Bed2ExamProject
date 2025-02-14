using OnlineClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinicBooking.Data;
namespace OnlineAppointmentBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AppointmentController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AppointmentController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private bool AppointmentExists(int id)
        {
            return (_dataContext.Appointments?.Any(Appointment => Appointment.Id == id)).GetValueOrDefault();
        }
        private bool DoctorExists(int id)
        {
            return (_dataContext.Doctors?.Any(Doctor => Doctor.Id == id)).GetValueOrDefault();
        }
        private bool CategoryExists(int id)
        {
            return (_dataContext.Categories?.Any(Category => Category.Id == id)).GetValueOrDefault();
        }
        private bool ClinicExists(int id)
        {
            return (_dataContext.Clinics?.Any(Clinic => Clinic.Id == id)).GetValueOrDefault();
        }
        private bool PatientExists(int id)
        {
            return (_dataContext.Patients?.Any(Patient => Patient.Id == id)).GetValueOrDefault();
        }
        private bool AppointmentExistsDoctor(DateTime date, int doctorId)
        {
            return _dataContext.Appointments.Any(a => a.Date == date && a.DoctorId == doctorId);
        }
        private bool AppointmentExistsPatient(DateTime date, int patientId)
        {
            return _dataContext.Appointments.Any(a => a.Date == date && a.PatientId == patientId);
        }
        private bool ClinicHasDoctor(int doctorId, int clinicId)
        {
            var doctor = _dataContext.Doctors?.FirstOrDefault(d => d.Id == doctorId);

            return doctor.ClinicId == clinicId;
        }
        private bool IsValidDate(DateTime date)
        {
            var today = DateTime.Today;
            var minValidDate = today;
            var maxValidDate = today.AddYears(50);

            return date >= minValidDate && date <= maxValidDate;
        }

        /// <summary>
        /// 
        /// Retrieves all Appointments.
        /// </summary>
        ///  <response code="200">Returns All the Appointments</response>
        /// <response code="400">Error getting Appointments</response>
        //GET api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointments()
        {
            try
            {
                if (_dataContext.Appointments == null)
                {
                    return NotFound(new { message = $"Appointments not found." });
                }

                var appointmentsData = await _dataContext.Appointments
                    .Include(a => a.Category)
                    .Include(a => a.Clinic)
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .ToListAsync();

                if (appointmentsData == null)
                {
                    return NotFound(new { message = $"Appointments Data not found." });
                }
                var appointmentsDTO = appointmentsData.Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    Date = a.Date,
                    CategoryId = a.Category.Id,
                    Category = a.Category.Name,
                    ClinicId = a.Clinic.Id,
                    Clinic = a.Clinic.Name,
                    DoctorId = a.Doctor.Id,
                    Doctor = $"{a.Doctor.Firstname} {a.Doctor.Lastname}",
                    PatientId = a.Patient.Id,
                    Patient = $"{a.Patient.Firstname} {a.Patient.Lastname}",
                    PatientNote = a?.PatientNote ?? "",
                    DoctorNote = a?.DoctorNote ?? "",
                }).ToList();

                return Ok(appointmentsDTO);
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


        // https://learn.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5 ,
        // https://medium.com/@nile.bits/asp-net-mvc-understanding-the-purpose-of-data-transfer-objects-dtos-ad1e24caf5c9
        // used these 2 sites to setup DTOs

        /// <summary>
        /// Retrieves Appointment with id.
        /// </summary>
        ///  <response code="200">Returns The requested Appointment</response>
        /// <response code="400">Error getting Appointment</response>
        /// <response code="404">Appointment does not exist</response>
        //GET api/Appointments/{id}
        [HttpGet("{Id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(int Id)
        {
            try
            {
                if (_dataContext.Appointments == null)
                {
                    return NotFound(new { message = $"Appointments not found." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!AppointmentExists(Id))
                {
                    return NotFound(new { message = $"Appointment with ID {Id} not found." });
                }

                var appointmentData = await _dataContext.Appointments
            .Include(a => a.Category)
            .Include(a => a.Clinic)
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == Id);

                if (appointmentData == null)
                {
                    return NotFound(new { message = $"Appointment Data not found." });
                }

                var appointmentDTO = new AppointmentDTO
                {
                    Id = appointmentData.Id,
                    Date = appointmentData.Date,
                    CategoryId = appointmentData.CategoryId,
                    Category = appointmentData.Category.Name,
                    ClinicId = appointmentData.Clinic.Id,
                    Clinic = appointmentData.Clinic.Name,
                    DoctorId = appointmentData.Doctor.Id,
                    Doctor = $"{appointmentData.Doctor.Firstname} {appointmentData.Doctor.Lastname}",
                    PatientId = appointmentData.Patient.Id,
                    Patient = $"{appointmentData.Patient.Firstname} {appointmentData.Patient.Lastname}",
                    PatientNote = appointmentData?.PatientNote ?? "",
                    DoctorNote = appointmentData?.DoctorNote ?? "",
                };

                return Ok(appointmentDTO);
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
        /// Add a new Appointment.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":0,
        ///        "Date": "2024-11-25T12:30:00",
        ///        "CategoryId": 1,
        ///        "ClinicId": 1,
        ///        "PatientId": 1,
        ///        "DoctorId": 1,
        ///        "PatientNote": "Hurt Hand on glass"
        ///     }
        /// </remarks>
        /// <response code="201">Appointment added</response>
        /// <response code="400">If the Appointment is null</response>
        /// <response code="500">Internal server error, Usually wrong or incomplete data input</response>
        // POST api/Appointments
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentDTO>> AddAppointment(Appointment appointment)
        {
            try
            {
                if (_dataContext.Appointments == null)
                {
                    return NotFound(new { message = $"Appointments database not found. Unable to add" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (AppointmentExists(appointment.Id))
                {
                    return BadRequest(new { message = $"Appointment with ID {appointment.Id} already exist." });
                }
                if (!DoctorExists(appointment.DoctorId))
                {
                    return BadRequest(new { message = $"Doctor with ID {appointment.DoctorId} does not exist. Please provide a valid DoctorId" });
                }
                if (!CategoryExists(appointment.CategoryId))
                {
                    return BadRequest(new { message = $"Category with ID {appointment.CategoryId} does not exist. Please provide a valid CategoryId" });
                }
                if (!ClinicExists(appointment.ClinicId))
                {
                    return BadRequest(new { message = $"Clinic with ID {appointment.ClinicId} does not exist. Please provide a valid ClinicId" });
                }
                if (!PatientExists(appointment.PatientId))
                {
                    return BadRequest(new { message = $"patient with ID {appointment.PatientId} does not exist. Please provide a valid PatientId" });
                }
                if (!IsValidDate(appointment.Date))
                {
                    return BadRequest(new { message = $"Date needs to be provided. Date cannot be in the past or too far into the future!" });
                }

                var doctor = _dataContext.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                var clinic = _dataContext.Clinics.FirstOrDefault(c => c.Id == appointment.ClinicId);
                if (!ClinicHasDoctor(appointment.DoctorId, appointment.ClinicId))
                {
                    return BadRequest(new { message = $"Doctor {doctor.Firstname} {doctor.Lastname} does not work in clinic {clinic.Name}" });
                }
                if (AppointmentExistsDoctor(appointment.Date, appointment.DoctorId))
                {
                    return BadRequest(new { message = $"An appointment already exist with this date and doctor" });
                }
                if (AppointmentExistsPatient(appointment.Date, appointment.PatientId))
                {
                    return BadRequest(new { message = $"An appointment already exist with this date and patient" });
                }

                _dataContext.Appointments.Add(appointment);
                await _dataContext.SaveChangesAsync();

                await _dataContext.Entry(appointment).Reference(a => a.Category).LoadAsync();
                await _dataContext.Entry(appointment).Reference(a => a.Clinic).LoadAsync();
                await _dataContext.Entry(appointment).Reference(a => a.Doctor).LoadAsync();
                await _dataContext.Entry(appointment).Reference(a => a.Patient).LoadAsync();

                var appointmentDTO = new AppointmentDTO()
                {
                    Id = appointment.Id,
                    Date = appointment.Date,
                    CategoryId = appointment.CategoryId,
                    Category = appointment.Category.Name,
                    ClinicId = appointment.Clinic.Id,
                    Clinic = appointment.Clinic.Name,
                    DoctorId = appointment.Doctor.Id,
                    Doctor = $"{appointment.Doctor.Firstname} {appointment.Doctor.Lastname}",
                    PatientId = appointment.Patient.Id,
                    Patient = $"{appointment.Patient.Firstname} {appointment.Patient.Lastname}",
                    PatientNote = appointment?.PatientNote ?? "",
                    DoctorNote = appointment?.DoctorNote ?? "",
                };

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, new
                {
                    Message = "Appointment created successfully.",
                    Appointment = appointmentDTO
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
        /// Updates a Appointment with the id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":4,
        ///        "Date": "2024-11-25T12:00:00",
        ///        "CategoryId": 1,
        ///        "ClinicId": 2,
        ///        "PatientId": 1,
        ///        "DoctorId": 2,
        ///        "DoctorNote": "Cleaned and put on bandage"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the Updated appointment</response>
        /// <response code="404">If the appointment is not found</response>
        /// <response code="400">If the appointment is null</response>
        //PUT api/Appointments/{id}
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointment(int Id, Appointment appointment)
        {
            try
            {
                if (Id != appointment.Id)
                {
                    return BadRequest(new { message = $"Appointment Id and Params Id need to be the same." });
                }
                if (_dataContext.Appointments == null)
                {
                    return NotFound(new { message = $"Appointments database not found!" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!AppointmentExists(Id))
                {
                    return BadRequest(new { message = $"Appointment with ID {Id} not found." });
                }
                var appointmentData = await _dataContext.Appointments
                                   .Include(a => a.Category)
                                   .Include(a => a.Clinic)
                                   .Include(a => a.Doctor)
                                   .Include(a => a.Patient)
                                   .FirstOrDefaultAsync(a => a.Id == Id);

                if (appointmentData == null)
                {
                    return NotFound(new { message = $"Appointment with ID {Id} not found." });
                }

                // If not all values is entered, use values from database
                appointment.DoctorId = appointment.DoctorId != 0 ? appointment.DoctorId : appointmentData.DoctorId;
                appointment.ClinicId = appointment.ClinicId != 0 ? appointment.ClinicId : appointmentData.ClinicId;
                appointment.PatientId = appointment.PatientId != 0 ? appointment.PatientId : appointmentData.PatientId;
                appointment.CategoryId = appointment.CategoryId != 0 ? appointment.CategoryId : appointmentData.CategoryId;
                appointment.Date = appointment.Date != default ? appointment.Date : appointmentData.Date;
                appointment.PatientNote = !string.IsNullOrEmpty(appointment.PatientNote) ? appointment.PatientNote : appointmentData.PatientNote;
                appointment.DoctorNote = !string.IsNullOrEmpty(appointment.DoctorNote) ? appointment.DoctorNote : appointmentData.DoctorNote;

                if (!DoctorExists(appointment.DoctorId))
                {
                    return NotFound(new { message = $"Doctor with ID {appointment.DoctorId} does not exist." });
                }

                if (!CategoryExists(appointment.CategoryId))
                {
                    return BadRequest(new { message = $"Category with ID {appointment.CategoryId} does not exist." });
                }


                if (!ClinicExists(appointment.ClinicId))
                {
                    return BadRequest(new { message = $"Clinic with ID {appointment.ClinicId} does not exist." });
                }

                if (!PatientExists(appointment.PatientId))
                {
                    return BadRequest(new { message = $"patient with ID {appointment.PatientId} does not exist." });
                }

                var doctor = _dataContext.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                var clinic = _dataContext.Clinics.FirstOrDefault(c => c.Id == appointment.ClinicId);

                if (!ClinicHasDoctor(appointment.DoctorId, appointment.ClinicId))
                {
                    return BadRequest(new { message = $"Doctor {doctor.Firstname} {doctor.Lastname} does not work in clinic {clinic.Name}." });
                }

                if (appointmentData.Date != appointment.Date || appointmentData.DoctorId != appointment.DoctorId)
                {
                    if (AppointmentExistsDoctor(appointment.Date, appointment.DoctorId))
                    {
                        return BadRequest(new { message = $"An appointment already exist with this date and doctor." });
                    }
                }
                if (appointmentData.Date != appointment.Date || appointmentData.PatientId != appointment.PatientId)
                {

                    if (AppointmentExistsPatient(appointment.Date, appointment.PatientId))
                    {
                        return BadRequest(new { message = $"An appointment already exist with this date and patient." });
                    }
                }

                var oldAppointmentDTO = new AppointmentDTO
                {
                    Id = appointmentData.Id,
                    Date = appointmentData.Date,
                    CategoryId = appointmentData.CategoryId,
                    Category = appointmentData.Category.Name,
                    ClinicId = appointmentData.Clinic.Id,
                    Clinic = appointmentData.Clinic.Name,
                    DoctorId = appointmentData.Doctor.Id,
                    Doctor = $"{appointmentData.Doctor.Firstname} {appointmentData.Doctor.Lastname}",
                    PatientId = appointmentData.Patient.Id,
                    Patient = $"{appointmentData.Patient.Firstname} {appointmentData.Patient.Lastname}",
                    PatientNote = appointmentData.PatientNote ?? "",
                    DoctorNote = appointmentData.DoctorNote ?? "",
                };

                _dataContext.Entry(appointmentData).CurrentValues.SetValues(appointment);

                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                await _dataContext.Entry(appointmentData).Reference(a => a.Category).LoadAsync();
                await _dataContext.Entry(appointmentData).Reference(a => a.Clinic).LoadAsync();
                await _dataContext.Entry(appointmentData).Reference(a => a.Doctor).LoadAsync();
                await _dataContext.Entry(appointmentData).Reference(a => a.Patient).LoadAsync();

                var appointmentDTO = new AppointmentDTO
                {
                    Id = appointmentData.Id,
                    Date = appointmentData.Date,
                    CategoryId = appointmentData.CategoryId,
                    Category = appointmentData.Category.Name,
                    ClinicId = appointmentData.Clinic.Id,
                    Clinic = appointmentData.Clinic.Name,
                    DoctorId = appointmentData.Doctor.Id,
                    Doctor = $"{appointmentData.Doctor.Firstname} {appointmentData.Doctor.Lastname}",
                    PatientId = appointmentData.Patient.Id,
                    Patient = $"{appointmentData.Patient.Firstname} {appointmentData.Patient.Lastname}",
                    PatientNote = appointmentData.PatientNote ?? "",
                    DoctorNote = appointmentData.DoctorNote ?? "",
                };

                return Ok(new
                {
                    message = $"Appointment '{appointment.Id}' updated successfully.",
                    oldData = oldAppointmentDTO,
                    newData = appointmentDTO
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
        /// Deletes a Appointment.
        /// </summary>
        /// <response code="200">Appointment Deleted</response>
        /// <response code="404">Could not find Appointment to delete</response>
        //DELETE api/Appointment/{id}
        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentDTO>> DeleteAppointment(int Id)
        {
            try
            {
                if (_dataContext.Appointments == null)
                {
                    return NotFound(new { message = $"Appointments not found!" });
                }
                if (!AppointmentExists(Id))
                {
                    return BadRequest(new { message = $"Appointment with ID {Id} not found." });
                }
                var appointment = await _dataContext.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == Id);

                var appointmentDTO = new AppointmentDTO
                {
                    Id = appointment.Id,
                    Date = appointment.Date,
                    Patient = appointment.Patient != null ? $"{appointment.Patient.Firstname} {appointment.Patient.Lastname}" : "Unknown Patient",
                };
                if (appointment is null)
                {
                    return NotFound();
                }
                Console.WriteLine("Appointment " + appointmentDTO.Id + " with patient " + appointmentDTO.Patient + " deleted");
                _dataContext.Appointments.Remove(appointment);
                await _dataContext.SaveChangesAsync();

                return Ok(new { message = $"Appointment '{appointmentDTO.Id}', with patient '{appointmentDTO.Patient}' at '{appointmentDTO.Date}' deleted successfully." });
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