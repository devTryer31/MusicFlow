using System.ComponentModel.DataAnnotations;
using MusicFlow.Domain.Entities.Base.Interfaces;

namespace MusicFlow.Domain.Entities.Base
{
    public class Entity : IEntity
    {
       [Key]
	    public long Id { get; set; }
    }
}
