using Microsoft.EntityFrameworkCore;

namespace PmjayAgra.Blazor.Data;

public class AgraDbContext : DbContext
{
 public AgraDbContext(DbContextOptions<AgraDbContext> options) : base(options)
 {
 }

 public DbSet<Agra1Dto> Agra1 { get; set; } = null!;

 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
 base.OnModelCreating(modelBuilder);

 // Keyless mapping for the Agra1 table and explicit column selection
 modelBuilder.Entity<Agra1Dto>(eb =>
 {
 eb.HasNoKey();
 eb.ToTable("Agra1");

 eb.Property(e => e.src_family_id).HasColumnName("src_family_id");
 eb.Property(e => e.src_member_id).HasColumnName("src_member_id");
 eb.Property(e => e.name).HasColumnName("name");
 eb.Property(e => e.relation).HasColumnName("relation");
 eb.Property(e => e.father_guardian_name).HasColumnName("father_guardian_name");
 eb.Property(e => e.rural_urban_flag).HasColumnName("rural_urban_flag");
 eb.Property(e => e.district_name).HasColumnName("district_name");
 eb.Property(e => e.blockname).HasColumnName("blockname");
 eb.Property(e => e.source_address).HasColumnName("source_address");
 eb.Property(e => e.addressasperadhaarofapproved).HasColumnName("addressasperadhaarofapproved");
 eb.Property(e => e.village_ward_lgd_code).HasColumnName("village_ward_lgd_code");
 eb.Property(e => e.source_type).HasColumnName("source_type");
 eb.Property(e => e.card_status_member).HasColumnName("card_status_member");
 eb.Property(e => e.card_status_family).HasColumnName("card_status_family");
 eb.Property(e => e.memberbelongtozeropovery).HasColumnName("memberbelongtozeropovery");
 });
 }
}