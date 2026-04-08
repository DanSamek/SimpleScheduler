using System.ComponentModel.DataAnnotations;

namespace SimpleScheduler.Entities;

internal class DoId
{
    [Key]
    public int Id { get; set; }
}