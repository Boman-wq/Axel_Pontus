using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{
    public record GameDto(Guid id, [Required] string Name, [Required] string Description, [Required][Range(1, 10)] int Grade, string Image);
    public record CreateGameDto([Required] string Name, [Required] string Description, [Required][Range(1,10)] int Grade, string Image);
    public record UpdateGameDto([Required] string Name, [Required] string Description, [Required][Range(1, 10)] int Grade, string Image);
}
