using Microsoft.EntityFrameworkCore;
using MusicFlow.Domain.Entities;

namespace MusicFlow.DAL.Context
{
	public class MusicFlowDb : DbContext
	{
		public DbSet<Chat> Chats { get; set; }

		public MusicFlowDb(DbContextOptions<MusicFlowDb> opt) : base(opt) { }
	}
}
