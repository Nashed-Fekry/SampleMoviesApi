using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Model;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenersController : ControllerBase
    {
        private readonly IGenresService _genresService;

        public GenersController(IGenresService genresService)
        {
            _genresService = genresService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genresService.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CraeteAsync(GenreDto dto)
        {
            var genre = new Genre { Name = dto.name };
            await _genresService.Add(genre);
            
            return Ok(genre); 
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> UpdateAsync(byte id ,[FromBody] GenreDto dto)
        {
            var genre = await _genresService.GetById(id);
            if(genre == null)
            {
                return NotFound($"No gengre with ID {id}");
            }
            genre.Name = dto.name;
            _genresService.Update(genre);
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genresService.GetById(id);
            if(genre == null)
            {
                return NotFound($"No gengre with ID {id}");
            }
            _genresService.Delete(genre);
            
            return Ok();
        }
    }
}
