using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Model;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMapper _mapper;

        public MoviesController(IMapper mapper)
        {
            _mapper = mapper;
        }

        private readonly IMovieService _movieService;
        private readonly IGenresService _genresService;

        public MoviesController(IGenresService genresService)
        {
            _genresService = genresService;
        }

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        private new List<string> _allwedExtentions = new List<string> { ".jpg", ".png", ".jpeg" };
        private long _maxAllowedPosterSize = 1048576; //1024*1024

       

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _movieService.GetAll();
            //map
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await  _movieService.GetById(id);
            if (movie == null)
                return NotFound("Not Found");

            var dto = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(dto);
        }

        [HttpGet("GetGenreById")]
        public async Task<IActionResult> GetGenreByIdAsync(byte genreId)
        {
            var movies = await _movieService.GetAll(genreId);
            var dto = _mapper.Map<MovieDetailsDto>(movies);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is Required");

            if (!_allwedExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("not allow this extentions");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("file size not allow");

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Incalid Genre ID!");

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();

            _movieService.Add(movie);
            return Ok(movie); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _movieService.GetById(id);
            if (movie == null)
                return NotFound("Not found");

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID!");

            if (dto.Poster != null) 
            {
                if (!_allwedExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("not allow this extentions");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("file size not allow");

                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }
            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.StoreLine = dto.StoreLine;    
            movie.Rate = dto.Rate;
            
            _movieService.Update(movie);
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _movieService.GetById(id);
            if (movie == null)
                return NotFound("Not found");

            _movieService.Delete(movie);
            
            return Ok(movie);
        }
    }
}
