using OnlineClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinicBooking.Data;

namespace OnlineClinicBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DoctorController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public DoctorController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private bool DoctorExists(int id)
        {
            return (_dataContext.Doctors?.Any(Doctor => Doctor.Id == id)).GetValueOrDefault();
        }
        private bool ClinicExists(int? id)
        {
            if (id == 0)
            {
                return true;
            }
            return (_dataContext.Clinics?.Any(Clinic => Clinic.Id == id)).GetValueOrDefault();
        }
        private bool SpecialityExists(int? id)
        {
            if (id == 0)
            {
                return true;
            }
            return (_dataContext.Specialities?.Any(Speciality => Speciality.Id == id)).GetValueOrDefault();
        }
        private bool IsDoctorInUse(int id)
        {
            return (_dataContext.Appointments?.Any(Appointment => Appointment.DoctorId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Retrieves all Doctors.
        /// </summary>
        ///  <response code="200">Returns All the Doctors</response>
        /// <response code="400">Error getting Doctors</response>
        //GET api/Doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
        {
            try
            {
                if (_dataContext.Doctors == null)
                {
                    return NotFound(new { message = $"Doctors not found." });
                }

                var doctorsData = await _dataContext.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .ToListAsync();

                if (doctorsData == null)
                {
                    return NotFound(new { message = $"Doctors Data not found." });
                }

                var doctorsDTO = doctorsData.Select(d => new DoctorDTO
                {
                    Id = d.Id,
                    Firstname = d.Firstname,
                    Lastname = d.Lastname,
                    ClinicId = d?.ClinicId ?? 0,
                    Clinic = d.Clinic?.Name ?? "Unknown Clinic",
                    SpecialityId = d?.SpecialityId ?? 0,
                    Speciality = d.Speciality?.Name ?? "Unknown Speciality",

                }).ToList();


                return Ok(doctorsDTO);

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
        /// Retrieves Doctor with id.
        /// </summary>
        ///  <response code="200">Returns The requested Doctor</response>
        /// <response code="400">Error getting Doctor</response>
        /// <response code="404">Doctor does not exist</response>
        //GET api/doctors/{id}
        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctor(int Id)
        {
            try
            {
                if (_dataContext.Doctors == null)
                {
                    return NotFound(new { message = $"Doctors not found." });
                }
                if (!DoctorExists(Id))
                {
                    return NotFound(new { message = $"Doctor with ID {Id} not found." });
                }
                var doctorsData = await _dataContext.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync(d => d.Id == Id);

                if (doctorsData == null)
                {
                    return NotFound(new { message = $"Doctors Data not found." });
                }

                var doctorDTO = new DoctorDTO
                {
                    Id = doctorsData.Id,
                    Firstname = doctorsData.Firstname,
                    Lastname = doctorsData.Lastname,
                    ClinicId = doctorsData?.ClinicId ?? 0,
                    Clinic = doctorsData.Clinic?.Name ?? "Unknown Clinic",
                    SpecialityId = doctorsData?.SpecialityId ?? 0,
                    Speciality = doctorsData.Speciality?.Name ?? "Unknown Speciality",
                };


                return Ok(doctorDTO);
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
        /// Retrieves Doctor that contains a query in its first or lastname
        /// </summary>
        ///  <response code="200">Returns The requested Doctor</response>
        /// <response code="400">Error getting Doctor</response>
        /// <response code="404">Doctor does not exist</response>
        //GET api/doctors/search/{query}
        [HttpGet("search/{Query}")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctor(string Query)
        {
            try
            {
                if (_dataContext.Doctors == null)
                {
                    return NotFound(new { message = $"Doctors not found." });
                }

                var doctorsData = await _dataContext.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .Where(d => d.Firstname.Contains(Query) || d.Lastname.Contains(Query) || (d.Firstname + " " + d.Lastname).Contains(Query)).ToListAsync();

                if (doctorsData == null || !doctorsData.Any())
                {
                    return NotFound(new { message = $"Doctor with name containing '{Query}' not found." });
                }

                var doctorsDTO = doctorsData.Select(d => new DoctorDTO
                {
                    Id = d.Id,
                    Firstname = d.Firstname,
                    Lastname = d.Lastname,
                    ClinicId = d?.ClinicId ?? 0,
                    Clinic = d.Clinic?.Name ?? "Unknown Clinic",
                    SpecialityId = d?.SpecialityId ?? 0,
                    Speciality = d.Speciality?.Name ?? "Unknown Speciality",

                }).ToList();


                return Ok(doctorsDTO);
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
        /// Add a new Doctor.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":0,
        ///        "Firstname": "Ola",
        ///        "Lastname": "Normann",
        ///        "ClinicId": 1,
        ///        "SpecialityId": 2
        ///     }
        /// </remarks>
        /// <response code="201">Doctor added</response>
        /// <response code="400">If the Doctor is null</response>
        /// <response code="401">If the User is Unauthorized</response>
        // POST api/Doctors
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DoctorDTO>> AddDoctor(Doctor doctor)
        {

            if (_dataContext.Doctors == null)
            {
                return NotFound(new { message = $"Doctors not found." });
            }
            if (DoctorExists(doctor.Id))
            {
                return NotFound(new { message = $"Doctor with ID {doctor.Id} already exist." });
            }
            if (doctor.ClinicId != null)
            {
                if (!ClinicExists(doctor.ClinicId))
                {
                    return NotFound(new { message = $"Clinic with ClinicID {doctor.ClinicId} Does not exist." });
                }
            }
            if (doctor.SpecialityId != null)
            {
                if (!SpecialityExists(doctor.SpecialityId))
                {
                    return NotFound(new { message = $"Speciality with Speciality {doctor.SpecialityId} Does not exist." });
                }
            }
            try
            {
                _dataContext.Doctors.Add(doctor);
                await _dataContext.SaveChangesAsync();

                await _dataContext.Entry(doctor).Reference(a => a.Clinic).LoadAsync();
                await _dataContext.Entry(doctor).Reference(a => a.Speciality).LoadAsync();

                var doctorDTO = new DoctorDTO()
                {
                    Id = doctor.Id,
                    Firstname = doctor.Firstname,
                    Lastname = doctor.Lastname,
                    ClinicId = doctor.Clinic?.Id ?? 0,
                    Clinic = doctor.Clinic?.Name ?? "",
                    SpecialityId = doctor.Speciality?.Id ?? 0,
                    Speciality = doctor.Speciality?.Name ?? "",
                };

                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, new
                {
                    message = $"Doctor {doctorDTO.Firstname} created",
                    Doctor = doctorDTO
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
        /// Updates a Doctor with the id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":7,
        ///        "Firstname": "Ola",
        ///        "Lastname": "Nordmann",
        ///        "ClinicId": 1,
        ///        "SpecialityId": 3
        ///     }
        /// </remarks>
        /// <response code="200">Returns the Updated doctor</response>
        /// <response code="404">If the doctor is not found</response>
        /// <response code="400">If the doctor is null</response>
        /// <response code="401">If the User Unauthorized</response>
        //PUT api/Doctors/{id}
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DoctorDTO>> UpdateDoctor(int Id, DoctorUpdateDTO doctor)
        {
            try
            {
                if (_dataContext.Doctors == null)
                {
                    return NotFound(new { message = "Doctors database not found." });
                }
                if (Id != doctor.Id)
                {
                    return BadRequest(new { message = "Doctor Id in the route and body must match." });
                }

                var doctorsData = await _dataContext.Doctors
                    .Include(d => d.Clinic)
                    .Include(d => d.Speciality)
                    .FirstOrDefaultAsync(d => d.Id == Id);

                if (doctorsData == null)
                {
                    return NotFound(new { message = $"Doctor with ID {Id} not found." });
                }

                // If not all values are entered, use existing values from the database
                doctor.ClinicId = doctor.ClinicId ?? doctorsData.ClinicId;
                doctor.SpecialityId = doctor.SpecialityId ?? doctorsData.SpecialityId;
                doctor.Firstname = !string.IsNullOrEmpty(doctor.Firstname) ? doctor.Firstname : doctorsData.Firstname;
                doctor.Lastname = !string.IsNullOrEmpty(doctor.Lastname) ? doctor.Lastname : doctorsData.Lastname;

                if (doctor.ClinicId != null)
                {
                    if (!ClinicExists(doctor.ClinicId))
                    {
                        return NotFound(new { message = $"Clinic with ClinicID {doctor.ClinicId} Does not exist." });
                    }
                }
                if (doctor.SpecialityId != null)
                {
                    if (!SpecialityExists(doctor.SpecialityId))
                    {
                        return NotFound(new { message = $"Speciality with Speciality {doctor.SpecialityId} Does not exist." });
                    }
                }

                var oldDoctorDTO = new DoctorDTO
                {
                    Id = doctorsData.Id,
                    Firstname = doctorsData.Firstname,
                    Lastname = doctorsData.Lastname,
                    ClinicId = doctorsData.Clinic?.Id ?? 0,
                    Clinic = doctorsData.Clinic?.Name ?? "Unknown Clinic",
                    SpecialityId = doctorsData.Speciality?.Id ?? 0,
                    Speciality = doctorsData.Speciality?.Name ?? "Unknown Speciality",
                };

                _dataContext.Entry(doctorsData).CurrentValues.SetValues(doctor);

                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                await _dataContext.Entry(doctorsData).Reference(d => d.Speciality).LoadAsync();
                await _dataContext.Entry(doctorsData).Reference(d => d.Clinic).LoadAsync();

                var newDoctorDTO = new DoctorDTO
                {
                    Id = doctorsData.Id,
                    Firstname = doctorsData.Firstname,
                    Lastname = doctorsData.Lastname,
                    ClinicId = doctorsData.Clinic?.Id ?? 0,
                    Clinic = doctorsData.Clinic?.Name ?? "Unknown Clinic",
                    SpecialityId = doctorsData.Speciality?.Id ?? 0,
                    Speciality = doctorsData.Speciality?.Name ?? "Unknown Speciality",
                };

                return Ok(new
                {
                    message = $"Doctor '{newDoctorDTO.Id}', '{newDoctorDTO.Firstname} {newDoctorDTO.Lastname}' updated successfully.",
                    oldData = oldDoctorDTO,
                    newData = newDoctorDTO
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
        /// Deletes a Doctor.
        /// </summary>
        /// <response code="200">Doctor Deleted</response>
        /// <response code="404">Could not find Doctor to delete</response>
        /// <response code="401">If the User Unauthorized</response>
        //DELETE api/Doctor/{id}
        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Doctor>> DeleteDoctor(int Id)
        {
            try
            {
                if (_dataContext.Doctors == null)
                {
                    return NotFound(new { message = $"Doctors not found." });
                }
                if (!DoctorExists(Id))
                {
                    return NotFound(new { message = $"Doctor with ID {Id} not found." });
                }
                if (IsDoctorInUse(Id))
                {
                    return BadRequest(new
                    {
                        message = $"Doctor with ID {Id} is in use! Remove or change from Appointments"
                    });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var doctor = await _dataContext.Doctors.FindAsync(Id);

                _dataContext.Doctors.Remove(doctor);
                await _dataContext.SaveChangesAsync();
                // Return a message with details about the deleted doctor
                return Ok(new { message = $"Doctor '{doctor.Id}', '{doctor.Firstname} {doctor.Lastname}' deleted successfully." });
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