using OnlineClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinicBooking.Data;

namespace OnlineClinicBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SpecialityController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public SpecialityController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private bool SpecialityExists(int id)
        {
            return (_dataContext.Specialities?.Any(Speciality => Speciality.Id == id)).GetValueOrDefault();
        }
        private bool SpecialityNameExists(string name)
        {
            return (_dataContext.Specialities?.Any(Speciality => Speciality.Name == name)).GetValueOrDefault();
        }
        private bool IsSpecialityInUse(int id)
        {
            return (_dataContext.Doctors?.Any(Doctor => Doctor.SpecialityId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Retrieves all Specialities.
        /// </summary>
        ///  <response code="200">Returns All the Specialities</response>
        /// <response code="400">Error getting Specialities</response>
        //GET api/Specialities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Speciality>>> GetSpecialities()
        {
            try
            {
                if (_dataContext.Specialities == null)
                {
                    return NotFound(new { message = $"Speciality not found!" });
                }

                return await _dataContext.Specialities.ToListAsync();
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
        /// Retrieves Speciality with id.
        /// </summary>
        ///  <response code="200">Returns The requested Speciality</response>
        /// <response code="400">Error getting Speciality</response>
        /// <response code="404">Speciality does not exist</response>
        //GET api/categories/{id}
        [HttpGet("{Id}")]
        public async Task<ActionResult<Speciality>> GetSpeciality(int Id)
        {
            try
            {
                if (_dataContext.Specialities == null)
                {
                    return NotFound(new { message = $"Speciality database not found!" });
                }
                if (!SpecialityExists(Id))
                {
                    return NotFound(new { message = $"Speciality with ID {Id} not found!" });
                }
                var speciality = await _dataContext.Specialities.FindAsync(Id);

                return speciality;
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
        /// Add a new Speciality.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":0,
        ///        "Name": "New Speciality"
        ///     }
        /// </remarks>
        /// <response code="201">Speciality added</response>
        /// <response code="400">If the Speciality is null</response>
        // POST api/Specialities
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Speciality>> AddSpeciality(Speciality speciality)
        {
            try
            {
                if (_dataContext.Specialities == null)
                {
                    return NotFound(new { message = $"Speciality database not found!" });
                }
                if (SpecialityExists(speciality.Id))
                {
                    return BadRequest(new { message = $"Speciality with Id {speciality.Id} already exist." });
                }
                if (SpecialityNameExists(speciality.Name))
                {
                    return BadRequest(new
                    {
                        message = $"Speciality with name {speciality.Name} already exist!"
                    });
                }
                _dataContext.Specialities.Add(speciality);
                await _dataContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetSpeciality), new { id = speciality.Id }, new
                {
                    message = $"Speciality {speciality.Name} created",
                    Speciality = speciality
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
        /// Updates a Speciality with the id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":6,
        ///        "Name": "Diagnostic"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the Updated speciality</response>
        /// <response code="404">If the speciality is not found</response>
        /// <response code="400">If the speciality is null</response>
        //PUT api/Specialities/{id}
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Speciality>> UpdateSpeciality(int Id, Speciality speciality)
        {
            try
            {
                if (Id != speciality.Id)
                {
                    return BadRequest(new { message = $"Speciality Id and Params Id need to be the same." });
                }
                if (_dataContext.Specialities == null)
                {
                    return NotFound(new { message = $"Specialities database not found!" });
                }
                if (SpecialityNameExists(speciality.Name))
                {
                    return BadRequest(new { message = $"Speciality with Name {speciality.Name} already exist." });
                }
                if (!SpecialityExists(Id))
                {
                    return NotFound(new
                    {
                        message = $"Speciality with id {Id} not found!"
                    });
                }
                var oldSpeciality = await _dataContext.Specialities.AsNoTracking().FirstOrDefaultAsync(d => d.Id == Id);
                //Updates entity properties that have been modified
                _dataContext.Update(speciality);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                // Return old and new speciality data
                return Ok(new
                {
                    message = $"Speciality '{speciality.Id}', '{speciality.Name}' updated successfully.",
                    oldData = oldSpeciality,
                    newData = speciality
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
        /// Deletes a Speciality.
        /// </summary>
        /// <response code="200">Speciality Deleted</response>
        /// <response code="404">Could not find Speciality to delete</response>
        //DELETE api/Speciality/{id}
        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Speciality>> DeleteSpeciality(int Id)
        {
            try
            {
                if (_dataContext.Specialities == null)
                {
                    return NotFound(new { message = $"Specialities database not found!" });
                }
                if (!SpecialityExists(Id))
                {
                    return NotFound(new
                    {
                        message = $"Speciality with id {Id} not found!"
                    });
                }
                if (IsSpecialityInUse(Id))
                {
                    return BadRequest(new
                    {
                        message = $"Speciality with ID {Id} is in use! Remove or change from Doctors"
                    });
                }
                var speciality = await _dataContext.Specialities.FindAsync(Id);

                Console.WriteLine("Speciality " + speciality.Name + " deleted");
                _dataContext.Specialities.Remove(speciality);
                await _dataContext.SaveChangesAsync();
                // Return a message with details about the deleted speciality
                return Ok(new { message = $"Speciality '{speciality.Id}', '{speciality.Name}' deleted successfully." });
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