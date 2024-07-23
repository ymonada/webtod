using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using webtod.DataAccess.Entities;
namespace webtod.DataAccess;

public class ApplicationDbContext: DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("db"));
    }

    public DbSet<NoteEntity> Notes => Set<NoteEntity>();
}