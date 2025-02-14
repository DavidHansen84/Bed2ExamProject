using OnlineClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinicBooking.Data;

namespace OnlineClinicBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private bool CategoryExists(int id)
        {
            return (_dataContext.Categories?.Any(Category => Category.Id == id)).GetValueOrDefault();
        }
        private bool CategoryNameExists(string name)
        {
            return (_dataContext.Categories?.Any(Category => Category.Name == name)).GetValueOrDefault();
        }
        private bool IsCategoryInUse(int id)
        {
            return (_dataContext.Appointments?.Any(Appointment => Appointment.CategoryId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Retrieves all Categories.
        /// </summary>
        ///  <response code="200">Returns All the Categories</response>
        /// <response code="400">Error getting Categories</response>
        //GET api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                if (_dataContext.Categories == null)
                {
                    return NotFound(new { message = $"Category not found!" });
                }
                var categories = await _dataContext.Categories.ToListAsync();

                return Ok(categories);
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
        /// Retrieves Category with id.
        /// </summary>
        ///  <response code="200">Returns The requested Category</response>
        /// <response code="400">Error getting Category</response>
        /// <response code="404">Category does not exist</response>
        //GET api/categories/{id}
        [HttpGet("{Id}")]
        public async Task<ActionResult<Category>> GetCategory(int Id)
        {
            try
            {
                if (_dataContext.Categories == null)
                {
                    return NotFound(new { message = $"Category database not found!" });
                }
                if (!CategoryExists(Id))
                {
                    return NotFound(new { message = $"Category with ID {Id} not found!" });
                }
                var category = await _dataContext.Categories.FindAsync(Id);

                return category;
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
        /// Add a new Category.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":0,
        ///        "Name": "New Category"
        ///     }
        /// </remarks>
        /// <response code="201">Category added</response>
        /// <response code="400">If the Category is null</response>
        /// <response code="401">If the User is Unauthorized</response>
        // POST api/Categories
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> AddCategory(Category category)
        {
            try
            {
                if (_dataContext.Categories == null)
                {
                    return NotFound(new { message = $"Category database not found!" });
                }
                if (CategoryNameExists(category.Name))
                {
                    return BadRequest(new { message = $"Category with Name {category.Name} already exist." });
                }
                if (CategoryExists(category.Id))
                {
                    return BadRequest(new
                    {
                        message = $"Category with id {category.Id} already exist!"
                    });
                }
                _dataContext.Categories.Add(category);
                await _dataContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new
                {
                    message = $"Category {category.Name} created",
                    Category = category
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
        /// Updates a Category with the id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Id":5,
        ///        "Name": "Other"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the Updated category</response>
        /// <response code="404">If the category is not found</response>
        /// <response code="400">If the category is null</response>
        /// <response code="401">If the User Unauthorized</response>
        //PUT api/Categories/{id}
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> UpdateCategory(int Id, Category category)
        {
            try
            {
                if (Id != category.Id)
                {
                    return BadRequest(new { message = $"Category Id and Params Id need to be the same." });
                }
                if (_dataContext.Categories == null)
                {
                    return NotFound(new { message = $"Category database not found!" });
                }
                if (CategoryNameExists(category.Name))
                {
                    return BadRequest(new { message = $"Category with Name {category.Name} already exist." });
                }
                if (!CategoryExists(Id))
                {
                    return NotFound(new
                    {
                        message = $"Category with id {Id} not found!"
                    });
                }
                var oldCategory = await _dataContext.Categories.AsNoTracking().FirstOrDefaultAsync(d => d.Id == Id);
                //Updates entity properties that have been modified
                _dataContext.Update(category);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                // Return old and new category data
                return Ok(new
                {
                    message = $"Category '{category.Id}', '{category.Name}' updated successfully.",
                    oldData = oldCategory,
                    newData = category
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
        /// Deletes a Category.
        /// </summary>
        /// <response code="200">Category Deleted</response>
        /// <response code="404">Could not find Category to delete</response>
        /// <response code="401">If the User Unauthorized</response>
        //DELETE api/Category/{id}
        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> DeleteCategory(int Id)
        {
            try
            {
                if (_dataContext.Categories == null)
                {
                    return NotFound(new { message = $"Category database not found!" });
                }
                if (!CategoryExists(Id))
                {
                    return NotFound(new
                    {
                        message = $"Category with id {Id} not found!"
                    });
                }
                if (IsCategoryInUse(Id))
                {
                    return BadRequest(new
                    {
                        message = $"Category with ID {Id} is in use! Remove or change from Appointments"
                    });
                }

                var category = await _dataContext.Categories.FindAsync(Id);

                Console.WriteLine("Category " + category.Name + " deleted");
                _dataContext.Categories.Remove(category);
                await _dataContext.SaveChangesAsync();
                // Return a message with details about the deleted category
                return Ok(new { message = $"Category '{category.Id}', '{category.Name}' deleted successfully." });
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