namespace MoviesApi.Dtos
{
    public class GenreDto
    {
        [MaxLength(100)]
        public string name { get; set; }
    }
}
