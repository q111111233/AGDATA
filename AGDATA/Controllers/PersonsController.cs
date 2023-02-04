using AGDATA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AGDATA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly PersonContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly string personKey = "personKey";

        public PersonsController(PersonContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            if (_memoryCache.TryGetValue(personKey, out PersonContext cachPersons))
            {
                return await cachPersons.Persons.ToListAsync();
            }

            if (_dbContext.Persons == null)
            {
                return NotFound();
            }

            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));

            await _memoryCache.Set(personKey, _dbContext.Persons.ToListAsync(), cacheOptions);

            return await _dbContext.Persons.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            if (_memoryCache.TryGetValue(personKey, out PersonContext cachPersons))
            {
                return await cachPersons.Persons.FindAsync(id);
            }

            if (_dbContext.Persons == null)
            {
                return NotFound();
            }
            var person = await _dbContext.Persons.FindAsync(id);

            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));

            _memoryCache.Set(personKey, person, cacheOptions);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            _dbContext.Persons.Add(person);
            await _dbContext.SaveChangesAsync();

            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
            _memoryCache.Set(personKey, _dbContext.Persons, cacheOptions);

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(person).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
            _memoryCache.Set(personKey, _dbContext.Persons, cacheOptions);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            if (_dbContext.Persons == null)
            {
                return NotFound();
            }

            var person = await _dbContext.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            _dbContext.Persons.Remove(person);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        private bool PersonExists(int id)
        {
            return (_dbContext.Persons?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
