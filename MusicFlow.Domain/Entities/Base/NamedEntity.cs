using MusicFlow.Domain.Entities.Base.Interfaces;

namespace MusicFlow.Domain.Entities.Base
{
    public class NamedEntity : INamedEntity
    {
	    public long Id { get; set; }
	    
	    public string Name { get; set; }
    }
}
