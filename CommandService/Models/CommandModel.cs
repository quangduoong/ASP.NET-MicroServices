using System.ComponentModel.DataAnnotations;

namespace CommandService.Models;

public class CommandModel
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string HowTo { get; set; } = default!;

    [Required]
    public string CommandLine { get; set; } = default!;

    [Required]
    public int PlatformId { get; set; }
    public PlatformModel Platform { get; set; } = default!;
}