using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class ImdbContext : DbContext
{
    public DbSet<Users> User { get; set; }
    public DbSet<UserRatings> UsersRating { get; set; }
    public DbSet<UserHistory> UsersHistory { get; set; }
    public DbSet<Attends> Attend { get; set; }
    public DbSet<AttributeAlts> AttributeAlt { get; set; }
    public DbSet<Attributes> Attribute { get; set; }
    public DbSet<Bookmarks> Bookmark { get; set; }
    public DbSet<BoxofficeTitles> BoxofficeTitle { get; set; }
    public DbSet<Crew> Crew { get; set; }
    public DbSet<DvdReleases> DvdReleases { get; set; }
    public DbSet<Episodes> Episodes { get; set; }
    public DbSet<Genres> Genre { get; set; }
    public DbSet<ImdbRatings> ImdbRating { get; set; }
    public DbSet<MaturityRatings> MaturityRating { get; set; }
    public DbSet<Productions> Production { get; set; }
    public DbSet<Regions> Region { get; set; }
    public DbSet<Runtimes> Runtime { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<TitleAwards> TitleAward { get; set; }
    public DbSet<TitleGenres> TitleGenre { get; set; }
    public DbSet<TitleMaturityRatings> TitleMaturityRating { get; set; }
    public DbSet<TitlePosters> TitlePoster { get; set; }
    public DbSet<TitleWebsites> TitleWebsite { get; set; }
    public DbSet<TitleRegions> TitleRegion { get; set; }
    public DbSet<WordIndex> WordIndex { get; set; }
    public DbSet<AlternateTitles> AlternateTitle { get; set; }
    public DbSet<Titles> Title { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseNpgsql("host=newtlike.com;db=rucdb;uid=rucdb;pwd=testdb;Client Encoding=UTF8",
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User mapping
        modelBuilder.Entity<Users>().ToTable("users");
        modelBuilder.Entity<Users>().HasKey(u => u.Username);
        modelBuilder.Entity<Users>().Property(x => x.Username).HasColumnName("username");
        modelBuilder.Entity<Users>().Property(x => x.Pswd).HasColumnName("pswd");

        // UsersRating mapping
        modelBuilder.Entity<UserRatings>().ToTable("user_ratings");
        modelBuilder.Entity<UserRatings>().HasKey(ur => new { ur.Username, ur.TitleId });
        modelBuilder.Entity<UserRatings>().Property(x => x.Username).HasColumnName("username");
        modelBuilder.Entity<UserRatings>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<UserRatings>().Property(x => x.Rating).HasColumnName("rating");

        // UsersHistory mapping
        modelBuilder.Entity<UserHistory>().ToTable("user_history");
        modelBuilder.Entity<UserHistory>().HasKey(uh => new { uh.Username, uh.TitleId, uh.Date });
        modelBuilder.Entity<UserHistory>().Property(x => x.Username).HasColumnName("username");
        modelBuilder.Entity<UserHistory>().Property(x => x.Date).HasColumnName("date_time");
        modelBuilder.Entity<UserHistory>().Property(x => x.TitleId).HasColumnName("title_id");
        
        // Attends mapping
        modelBuilder.Entity<Attends>().ToTable("attends");
        modelBuilder.Entity<Attends>().HasKey(a => new { a.TitleId, a.CrewId });
        modelBuilder.Entity<Attends>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<Attends>().Property(x => x.CrewId).HasColumnName("crew_id");
        modelBuilder.Entity<Attends>().Property(x => x.CrewRole).HasColumnName("crew_role");
        modelBuilder.Entity<Attends>().Property(x => x.Job).HasColumnName("job");
        modelBuilder.Entity<Attends>().Property(x => x.CrewCharacter).HasColumnName("crew_character");

        // AttributeAlts mapping
        modelBuilder.Entity<AttributeAlts>().ToTable("attribute_alts");
        modelBuilder.Entity<AttributeAlts>().HasKey(aa => new { aa.TitleId, aa.AltsOrdering });
        modelBuilder.Entity<AttributeAlts>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<AttributeAlts>().Property(x => x.AltsOrdering).HasColumnName("alts_ordering");
        modelBuilder.Entity<AttributeAlts>().Property(x => x.Attribute).HasColumnName("attribute");

        // Attributes mapping
        modelBuilder.Entity<Attributes>().ToTable("attributes");
        modelBuilder.Entity<Attributes>().HasKey(attr => attr.Attribute);
        modelBuilder.Entity<Attributes>().Property(x => x.Attribute).HasColumnName("attribute");

        // Bookmarks mapping
        modelBuilder.Entity<Bookmarks>().ToTable("bookmarks");
        modelBuilder.Entity<Bookmarks>().Property(x => x.Username).HasColumnName("username");
        modelBuilder.Entity<Bookmarks>().Property(x => x.TitleId).HasColumnName("title_id");
        // Composity key for Bookmarks
        modelBuilder.Entity<Bookmarks>().HasKey(b => new { b.Username, b.TitleId });

        // BoxofficeTitles mapping
        modelBuilder.Entity<BoxofficeTitles>().ToTable("boxoffice_title");
        modelBuilder.Entity<BoxofficeTitles>().HasKey(bt => bt.TitleId);
        modelBuilder.Entity<BoxofficeTitles>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<BoxofficeTitles>().Property(x => x.BoxOffice).HasColumnName("box_office");

        // Crew mapping
        modelBuilder.Entity<Crew>().ToTable("crew");
        modelBuilder.Entity<Crew>().HasKey(c => c.CrewId);
        modelBuilder.Entity<Crew>().Property(x => x.CrewId).HasColumnName("id");
        modelBuilder.Entity<Crew>().Property(x => x.Fullname).HasColumnName("full_name");
        modelBuilder.Entity<Crew>().Property(x => x.BirthYear).HasColumnName("birthyear");
        modelBuilder.Entity<Crew>().Property(x => x.DeathYear).HasColumnName("deathyear");
        modelBuilder.Entity<Crew>().Property(x => x.AverageRating).HasColumnName("average_rating");

        // DvdReleases mapping
        modelBuilder.Entity<DvdReleases>().ToTable("dvd_release");
        modelBuilder.Entity<DvdReleases>().HasKey(dr => dr.TitleId);
        modelBuilder.Entity<DvdReleases>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<DvdReleases>().Property(x => x.Dvd).HasColumnName("dvd");
        
        // Episodes mapping
        modelBuilder.Entity<Episodes>().ToTable("episodes");
        modelBuilder.Entity<Episodes>().HasKey(e => e.EpisodeId);
        modelBuilder.Entity<Episodes>().Property(x => x.EpisodeId).HasColumnName("episode_id");
        modelBuilder.Entity<Episodes>().Property(x => x.SeriesId).HasColumnName("series_id");
        modelBuilder.Entity<Episodes>().Property(x => x.SeasonNumber).HasColumnName("season_number");
        modelBuilder.Entity<Episodes>().Property(x => x.EpisodeNumber).HasColumnName("episode_number");

        // Genres mapping
        modelBuilder.Entity<Genres>().ToTable("genres");
        modelBuilder.Entity<Genres>().HasKey(g => g.Genre);
        modelBuilder.Entity<Genres>().Property(x => x.Genre).HasColumnName("genre");

        // ImdbRatings mapping
        modelBuilder.Entity<ImdbRatings>().ToTable("imdb_ratings");
        modelBuilder.Entity<ImdbRatings>().HasKey(ir => ir.TitleId);
        modelBuilder.Entity<ImdbRatings>().Property(x => x.TitleId).HasColumnName("titles_id");
        modelBuilder.Entity<ImdbRatings>().Property(x => x.UserRating).HasColumnName("user_rating");
        modelBuilder.Entity<ImdbRatings>().Property(x => x.NumUserRatings).HasColumnName("num_user_ratings");
        
        // MaturityRatings mapping
        modelBuilder.Entity<MaturityRatings>().ToTable("maturity_ratings");
        modelBuilder.Entity<MaturityRatings>().HasKey(mr => mr.TitleId);
        modelBuilder.Entity<MaturityRatings>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<MaturityRatings>().Property(x => x.MaturityRating).HasColumnName("maturity_rating");

        // ProductionTitles mapping
        modelBuilder.Entity<ProductionTitles>().ToTable("productions_titles");
        modelBuilder.Entity<ProductionTitles>().HasKey(pt => new { pt.TitleId, pt.Production });
        modelBuilder.Entity<ProductionTitles>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<ProductionTitles>().Property(x => x.Production).HasColumnName("production");

        // productions mapping
        modelBuilder.Entity<Productions>().ToTable("productions");
        modelBuilder.Entity<Productions>().HasKey(p => p.Production);
        modelBuilder.Entity<Productions>().Property(x => x.Production).HasColumnName("production");

        // Regions mapping
        modelBuilder.Entity<Regions>().ToTable("regions");
        modelBuilder.Entity<Regions>().HasKey(r => r.Region);
        modelBuilder.Entity<Regions>().Property(x => x.Region).HasColumnName("region");
        modelBuilder.Entity<Regions>().Property(x => x.Language).HasColumnName("language");

        // Runtimes mapping
        modelBuilder.Entity<Runtimes>().ToTable("runtimes");
        modelBuilder.Entity<Runtimes>().HasKey(rt => rt.TitleId);
        modelBuilder.Entity<Runtimes>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<Runtimes>().Property(x => x.Runtime).HasColumnName("runtime");

        // Series mapping
        modelBuilder.Entity<Series>().ToTable("series");
        modelBuilder.Entity<Series>().HasKey(s => s.SeriesId);
        modelBuilder.Entity<Series>().Property(x => x.SeriesId).HasColumnName("series_id");
        modelBuilder.Entity<Series>().Property(x => x.Episodes).HasColumnName("episodes");
        modelBuilder.Entity<Series>().Property(x => x.Seasons).HasColumnName("sesaons");   

        // TitleAwards mapping
        modelBuilder.Entity<TitleAwards>().ToTable("title_awards");
        modelBuilder.Entity<TitleAwards>().HasKey(ta => ta.TitleId);
        modelBuilder.Entity<TitleAwards>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<TitleAwards>().Property(x => x.Awards).HasColumnName("awards");

        // TitleGenres mapping
        modelBuilder.Entity<TitleGenres>().ToTable("title_genres");
        modelBuilder.Entity<TitleGenres>().HasKey(tg => new { tg.TitleId, tg.Genre });
        modelBuilder.Entity<TitleGenres>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<TitleGenres>().Property(x => x.Genre).HasColumnName("genre");

        // TitleMaturityRatings mapping
        modelBuilder.Entity<TitleMaturityRatings>().ToTable("title_maturity_ratings");
        modelBuilder.Entity<TitleMaturityRatings>().HasKey(tmr => tmr.TitleId);
        modelBuilder.Entity<TitleMaturityRatings>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<TitleMaturityRatings>().Property(x => x.MaturityRating).HasColumnName("maturity_rating");

        // TitlePosters mapping
        modelBuilder.Entity<TitlePosters>().ToTable("title_posters");
        modelBuilder.Entity<TitlePosters>().HasKey(tp => tp.TitleId);
        modelBuilder.Entity<TitlePosters>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<TitlePosters>().Property(x => x.Poster).HasColumnName("poster");

        // TitleWebsites mapping
        modelBuilder.Entity<TitleWebsites>().ToTable("title_websites");
        modelBuilder.Entity<TitleWebsites>().HasKey(tw => tw.TitleId);
        modelBuilder.Entity<TitleWebsites>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<TitleWebsites>().Property(x => x.Website).HasColumnName("website");

        // TitleRegions mapping
        modelBuilder.Entity<TitleRegions>().ToTable("title_regions");
        modelBuilder.Entity<TitleRegions>().HasKey(tr => new { tr.TitleId, tr.Region });
        modelBuilder.Entity<TitleRegions>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<TitleRegions>().Property(x => x.Region).HasColumnName("region");

        // WordIndex mapping
        modelBuilder.Entity<WordIndex>().ToTable("word_index");
        modelBuilder.Entity<WordIndex>().HasKey(wi => new { wi.TitleId, wi.Word, wi.Field });
        modelBuilder.Entity<WordIndex>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<WordIndex>().Property(x => x.Word).HasColumnName("word");
        modelBuilder.Entity<WordIndex>().Property(x => x.Field).HasColumnName("field");
        modelBuilder.Entity<WordIndex>().Property(x => x.Lexeme).HasColumnName("lexeme");

        // AlternateTitles mapping
        modelBuilder.Entity<AlternateTitles>().ToTable("alternate_titles");
        modelBuilder.Entity<AlternateTitles>().HasKey(at => new { at.TitleId, at.Ordering });
        modelBuilder.Entity<AlternateTitles>().Property(x => x.TitleId).HasColumnName("title_id");
        modelBuilder.Entity<AlternateTitles>().Property(x => x.Ordering).HasColumnName("alts_ordering");
        modelBuilder.Entity<AlternateTitles>().Property(x => x.AltsTitle).HasColumnName("alts_title");
        modelBuilder.Entity<AlternateTitles>().Property(x => x.Types).HasColumnName("types");
        modelBuilder.Entity<AlternateTitles>().Property(x => x.IsOriginalTitle).HasColumnName("IsOriginalTitle");

        // Titles mapping
        modelBuilder.Entity<Titles>().ToTable("titles");
        modelBuilder.Entity<Titles>().HasKey(t => t.Id);
        modelBuilder.Entity<Titles>().Property(x => x.Id).HasColumnName("id");
        modelBuilder.Entity<Titles>().Property(x => x.Title).HasColumnName("title");
        modelBuilder.Entity<Titles>().Property(x => x.TitleType).HasColumnName("titletype");
        modelBuilder.Entity<Titles>().Property(x => x.Plot).HasColumnName("plot");
        modelBuilder.Entity<Titles>().Property(x => x.Year).HasColumnName("year");
        modelBuilder.Entity<Titles>().Property(x => x.StartYear).HasColumnName("startyear");
        modelBuilder.Entity<Titles>().Property(x => x.EndYear).HasColumnName("endyear");
        modelBuilder.Entity<Titles>().Property(x => x.Release_Date).HasColumnName("release_date");
        modelBuilder.Entity<Titles>().Property(x => x.OriginalTitle).HasColumnName("originaltitle");
        modelBuilder.Entity<Titles>().Property(x => x.IsAdult).HasColumnName("isadult");
        modelBuilder.Entity<Titles>().Property(x => x.Rating).HasColumnName("rating");
        modelBuilder.Entity<Titles>().Property(x => x.Votes).HasColumnName("votes");
    }
}