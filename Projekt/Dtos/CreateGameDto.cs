using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Catalog.Dtos
{
    public record CreateGameDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get ; set; }

        [Required]
        [Range(1, 10)]
        public int Grade { get; set; }

        public string Image { get; set; }
        
    }
}