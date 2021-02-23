using Marcware.JudgeMyPhoto.Classes;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// To perform migrations, use the following commands
// Add-Migration Name -Context JudgeMyPhotoDbContext -OutputDir Migrations
// Update-Database -Context JudgeMyPhotoDbContext
namespace Marcware.JudgeMyPhoto.Entities.Context
{
    /// <summary>
    /// Context of the database holding the photos
    /// </summary>
    public class JudgeMyPhotoDbContext : IdentityDbContext<ApplicationUser>
    {
        string connectionString = string.Empty;

        public JudgeMyPhotoDbContext(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString(AppSettingKeys.DefaultDbConnectionString);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }

        /// <summary>
        /// Photograps submitted for the competition
        /// </summary>
        public DbSet<Photograph> Photographs { get; set; }

        /// <summary>
        /// Photo categories
        /// </summary>
        public DbSet<PhotoCategory> PhotoCategories { get; set; }

        /// <summary>
        /// Photo votes
        /// </summary>
        public DbSet<PhotoVote> PhotoVotes { get; set; }

        public ProcessResult<bool> SaveUpdates(params object[] entities)
        {
            ProcessResult<bool> result = new ProcessResult<bool>();
            //TODO: Handle any errors
            UpdateRange(entities);
            SaveChanges();            
            return result;
        }

        public ProcessResult<bool> SaveAdditions(params object[] entities)
        {
            ProcessResult<bool> result = new ProcessResult<bool>();
            //TODO: Handle any errors
            AddRange(entities);
            SaveChanges();
            return result;
        }

        public ProcessResult<bool> SaveRemoves(params object[] entities)
        {
            ProcessResult<bool> result = new ProcessResult<bool>();
            //TODO: Handle any errors
            RemoveRange(entities);
            SaveChanges();
            return result;
        }
    }
}
