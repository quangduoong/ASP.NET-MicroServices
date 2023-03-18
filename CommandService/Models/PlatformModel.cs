using System.ComponentModel.DataAnnotations;

namespace CommandService.Models;

public class PlatformModel
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int ExternalId { get; set; }

    [Required]
    public string Name { get; set; } = default!;
    public ICollection<CommandModel> Commands { get; set; } = default!;
}