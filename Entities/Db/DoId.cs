using System.ComponentModel.DataAnnotations;

namespace SimpleScheduler.Entities.Db;

public class DoId
{
    [Key]
    public int Id { get; set; }
}