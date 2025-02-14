using OnlineClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinicBooking.Data;

namespace OnlineClinicBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClinicController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ClinicController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private bool ClinicExists(int id)
        {
            return (_dataContext.Clinics?.Any(Clinic => Clinic.Id == id)).GetValueOrDefault();
        }
        private bool ClinicNameExists(string name)
        {
            return (_dataContext.Clinics?.Any(Clinic => Clinic.Name == name)).GetValueOrDefault();
        }
        private bool IsClinicInUse(int id)
        {
            if (_dataContext.Appointments.Any(Appointment => Appointment.ClinicId == id))
                return true;
            if (_dataContext.Doctors.Any(Doctor => Doctor.ClinicId == id))
                return true;
            return false;
        }

        /// <summary>
        /// Retrieves all Clinics.
        /// </summary>
        ///  <response code="200">Returns All the Clinics</response>
        /// <response code="400">Error getting Clinics</response>
        //GET api/Clinics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinics()
        {
            try
            {
                if (_dataContext.Clinics == null)
                {
                    return NotFound(new { message = $"Clinics not found." });
                }

                var clinicsData = await _dataContext.Clinics
                    .Include(d => d.Doctors)
                    .Include(a => a.Appointments)
                        .ThenInclude(a => a.Category)
                    .Include(a => a.Appointments)
                        .ThenInclude(p => p.Patient)

                    .ToListAsync();
                if (clinicsData == null)
                {
                    return NotFound(new { message = $"Clinics Data not found." });
                }
                var clinicsDTO = clinicsData.Select(c => new ClinicDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Doctors = c.Doctors.Select(d => new ClinicDoctorDTO
                    {
                        Id = d.Id,
                        Fullname = d.Firstname + " " + d.Lastname,

                    }).ToList(),
                    Appointments = c.Appointments.Select(a => new ClinicAppointmentDTO
                    {
                        Id = a.Id,
                        Date = a.Date,
                        CategoryId = a.Category.Id,
                        Category = a.Category.Name,
                        DoctorId = a.DoctorId,
                        Doctor = $"{a.Doctor.Firstname} {a.Doctor.Lastname}",
                        PatientId = a.PatientId,
                        Patient = $"{a.Patient.Firstname} {a.Patient.Lastname}",

                    }).ToList(),
                }).ToList();


                return Ok(clinicsDTO);
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
        /// Retrieves Clinic with id.
        /// </summary>
        ///  <response code="200">Returns The requested Clinic</response>
        /// <response code="400">Error getting Clinic</response>
        /// <response code="404">Clinic does not exist</response>
        //GET api/categories/{id}
        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinic(int Id)
        {
            try
            {
                if (_dataContext.Clinics == null)
                {
                    return NotFound(new { message = $"Clinics not found." });
                }
                if (!ClinicExists(Id))
                {
                    return NotFound(new { message = $"Clinic with ID {Id} not found." });
                }

                var clinicData = await _dataContext.Clinics
                    .Include(d => d.Doctors)
                    .Include(a => a.Appointments)
                        .ThenInclude(a => a.Category)
                    .Include(a => a.Appointments)
                        .ThenInclude(p => p.Patient)
                    .FirstOrDefaultAsync(a => a.Id == Id);

                if (clinicData == null)
                {
                    return NotFound(new { message = $"Clinic Data not found." });
                }

                var clinicDTO = new ClinicDTO
                {
                    Id = clinicData.Id,
                    Name = clinicData.Name,
                    Doctors = clinicData.Doctors.Select(d => new ClinicDoctorDTO
                    {
                        Id = d.Id,
                        Fullname = d.Firstname + " " + d.Lastname,

                    }).ToList(),
                    Appointments = clinicData.Appointments.Select(a => new ClinicAppointmentDTO
                    {
                        Id = a.Id,
                        Date = a.Date,
                        CategoryId = a.Category.Id,
                        Category = a.Category.Name,
                        DoctorId = a.DoctorId,
                        Doctor = $"{a.Doctor.Firstname} {a.Doctor.Lastname}",
                        PatientId = a.PatientId,
                        Patient = $"{a.Patient.Firstname} {a.Patient.Lastname}",

                    }).ToList(),
                };


                return Ok(clinicDTO);
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
        /// Add a new Clinic.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":0,
        ///        "Name": "New Clinic"
        ///     }
        /// </remarks>
        /// <response code="201">Clinic added</response>
        /// <response code="400">If the Clinic is null</response>
        /// <response code="401">If the User is Unauthorized</response>
        // POST api/Clinics
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClinicAddDTO>> AddClinic(Clinic clinic)
        {
            try
            {
                if (_dataContext.Clinics == null)
                {
                    return NotFound(new { message = $"Clinics not found." });
                }
                if (ClinicExists(clinic.Id))
                {
                    return BadRequest(new { message = $"Clinic with ID {clinic.Id} already exists." });
                }
                if (ClinicNameExists(clinic.Name))
                {
                    return BadRequest(new { message = $"Clinic with Name {clinic.Name} already exist." });
                }
                _dataContext.Clinics.Add(clinic);
                await _dataContext.SaveChangesAsync();

                var clinicDTO = new ClinicAddDTO
                {
                    Id = clinic.Id,
                    Name = clinic.Name,
                };
                return CreatedAtAction(nameof(GetClinic), new { id = clinic.Id }, new
                {
                    message = $"Clinic {clinicDTO.Name} created",
                    Clinic = clinicDTO
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
        /// Updates a Clinic with the id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":4,
        ///        "Name": "Clinic D"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the Updated clinic</response>
        /// <response code="404">If the clinic is not found</response>
        /// <response code="400">If the clinic is null</response>
        /// <response code="401">If the User Unauthorized</response>
        //PUT api/Clinics/{id}
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClinicDTO>> UpdateClinic(int Id, Clinic clinic)
        {
            try
            {
                if (Id != clinic.Id)
                {
                    return BadRequest("Clinic Id and Params Id need to be the same.");
                }
                if (!ClinicExists(Id))
                {
                    return NotFound(new { message = $"Clinic with ID {clinic.Id} not found!" });
                }
                if (ClinicNameExists(clinic.Name))
                {
                    return NotFound(new { message = $"Clinic with Name {clinic.Name} already exist." });
                }
                var clinicData = await _dataContext.Clinics
                        .FirstOrDefaultAsync(a => a.Id == Id);

                var oldClinicDTO = new ClinicAddDTO
                {
                    Id = clinicData.Id,
                    Name = clinicData.Name,

                };
                _dataContext.Entry(clinicData).CurrentValues.SetValues(clinic);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                var clinicDTO = new ClinicAddDTO
                {
                    Id = clinicData.Id,
                    Name = clinicData.Name,
                };
                // Return old and new clinic data
                return Ok(new
                {
                    message = $"Clinic '{clinic.Id}', '{clinic.Name}' updated successfully.",
                    oldData = oldClinicDTO,
                    newData = clinicDTO
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
        /// Deletes a Clinic.
        /// </summary>
        /// <response code="200">Clinic Deleted</response>
        /// <response code="404">Could not find Clinic to delete</response>
        /// <response code="401">If the User Unauthorized</response>
        //DELETE api/Clinic/{id}
        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicDTO>> DeleteClinic(int Id)
        {
            try
            {
                if (_dataContext.Clinics == null)
                {
                    return NotFound();
                }
                if (!ClinicExists(Id))
                {
                    return NotFound(new { message = $"Clinic with ID {Id} not found!" });
                }
                if (IsClinicInUse(Id))
                {
                    return BadRequest(new
                    {
                        message = $"Clinic with ID {Id} is in use! Remove or change from Appointments and/or Doctors"
                    });
                }
                var clinic = await _dataContext.Clinics.FindAsync(Id);

                var clinicDTO = new ClinicDTO
                {
                    Id = clinic.Id,
                    Name = clinic.Name,
                };
                Console.WriteLine("Clinic " + clinicDTO.Name + " deleted");
                _dataContext.Clinics.Remove(clinic);
                await _dataContext.SaveChangesAsync();
                // Return a message with details about the deleted clinic
                return Ok(new { message = $"Clinic '{clinicDTO.Id}', '{clinicDTO.Name}' deleted successfully." });
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