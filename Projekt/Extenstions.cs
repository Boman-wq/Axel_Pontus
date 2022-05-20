using Catalog.Dtos;
using Catalog.Models;

namespace Catalog
{
    public static class Extenstions
    {
        public static GameDto AsDto(this Game form)
        {
            return new GameDto{
                Id = form.Id,
                Name = form.Name,
                Description = form.Description,
                Grade = form.Grade,
                Image = form.Image
            };
        }
    }
}