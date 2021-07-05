using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTO;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly IMapper _mapper;
        private readonly string containerName = "people";

        public PeopleController
            (
                ApplicationDbContext context,
                IFileStorageService fileStorage,
                IMapper mapper
            )
        {
            _context = context;
            _fileStorage = fileStorage;
            _mapper = mapper;
        }

        [HttpGet(Name = "getPeople")]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.People.AsQueryable();

            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage); // returns the total number of record pages

            var people = await queryable.Paginate(pagination).ToListAsync(); // Sets the maximum amount of records to show per page

            return _mapper.Map<List<PersonDTO>>(people);
        }

        [HttpGet("{id:int}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            var person = await _context.People.FirstOrDefaultAsync(x => x.ID == id);

            if (person == null)
            {
                return NotFound();
            }

            var personDTO = _mapper.Map<PersonDTO>(person);
            return personDTO;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreation)
        {
            var person = _mapper.Map<Person>(personCreation);

            if (personCreation.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreation.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreation.Picture.FileName);
                    person.Picture = await _fileStorage.SaveFile(content, extension, containerName, personCreation.Picture.ContentType);
                }
            }

            _context.Add(person);
            await _context.SaveChangesAsync();

            var personDTO = _mapper.Map<PersonDTO>(person);

            return new CreatedAtRouteResult("getPerson", new { personDTO.ID }, personDTO);
        }

        [HttpPut("{id}")] // HttpPut does a complete update - If a field isn't sent as part of the update, it sets them to null/default
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm] PersonCreationDTO personCreationDTO)
        {
            var personDB = await _context.People.FirstOrDefaultAsync(x => x.ID == id);

            if (personDB == null) { return NotFound(); }

            personDB = _mapper.Map(personCreationDTO, personDB);

            // Check to see if the update contains a new picture
            // If a new picture is sent, we will update
            // If not, this will be skipped

            if (personCreationDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.Picture.FileName);
                    personDB.Picture =
                        await _fileStorage.EditFile(content, extension, containerName,
                                                            personDB.Picture,
                                                            personCreationDTO.Picture.ContentType);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await _context.People.FirstOrDefaultAsync(x => x.ID == id); // Get the entity from the database with the matching ID

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<PersonPatchDTO>(entityFromDB);

            patchDocument.ApplyTo(entityDTO, ModelState); // Applies the changes from the patchDocument (updates) to the entityDTO

            var isValid = TryValidateModel(entityDTO); // Check that the updates have the correct names for the model

            if (!isValid)
            {
                return BadRequest(ModelState); // If the updated model isn't valid, return the error.
            }

            _mapper.Map(entityDTO, entityFromDB); // Appling the changes back to the entity from the database

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.People.AnyAsync(x => x.ID == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Person() { ID = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}