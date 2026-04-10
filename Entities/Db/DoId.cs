using System.ComponentModel.DataAnnotations;

namespace SimpleScheduler.Entities.Db;

internal class DoId
{
    [Key]
    public int Id { get; set; }
}