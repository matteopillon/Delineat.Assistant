using Delineat.Assistant.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data
{
    public class DAAssistantDBContext : DbContext
    {
        private readonly string connectionString;

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<NotesReminderRecipient> NotesReminderRecipients { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<DocumentVersionMetadata> DocumentVersionMetadatas { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<DocumentVersionData> DocumentVersionDatas { get; set; }
        public DbSet<StoreSyncLog> StoreSyncLogs { get; set; }
        public DbSet<StoreSyncLogEntry> StoreSyncLogEntries { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }
        public DbSet<ThumbnailData> ThumbnailDatas { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<JobGroup> JobGroups { get; set; }

        public DbSet<DocumentsTopics> DocumentsTopics { get; set; }
        public DbSet<ItemsTopics> ItemsTopics { get; set; }
        public DbSet<NotesTopics> NotesTopics { get; set; }
        public DbSet<DocumentsTags> DocumentsTags { get; set; }
        public DbSet<ItemsTags> ItemsTags { get; set; }
        public DbSet<JobsTags> JobsTags { get; set; }
        public DbSet<NotesTags> NotesTags { get; set; }

        public DbSet<DocumentsNotes> DocumentsNotes { get; set; }
        public DbSet<CustomersNotes> CustomersNotes { get; set; }
        public DbSet<JobsNotes> JobsNotes { get; set; }
        public DbSet<ItemsNotes> ItemsNotes { get; set; }

        public DbSet<WorkLog> WorkLogs { get; set; }
        public DbSet<DocumentsWorkLogs> DocumentsWorkLogs { get; set; }
        public DbSet<ItemsWorkLogs> ItemsWorkLogs { get; set; }
        public DbSet<WorkLogType> WorkLogTypes { get; set; }

        public DbSet<JobCode> JobCodes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<DayWorkLog> DayWorkLogs { get; set; }

        public DbSet<DayWorkType> DayWorkTypes { get; set; }

       
        public DbSet<Role> Roles { get; set; }

        public DbSet<UserCredential> UserCredentials { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<JobType> JobTypes { get; set; }

        public DbSet<WeekWorkHours> WeekWorkHours { get; set; }


        public DbSet<HolidayDate> HolidayDates { get; set; }

        public DbSet<ExtraField> ExtraFields { get; set; }

        public DbSet<ExtraFieldDomainValue> ExtraFieldValues { get; set; }

        public DbSet<JobExtraFieldValue> JobExtraFields { get; set; }

        public DAAssistantDBContext(DbContextOptions<DAAssistantDBContext> contextOptions) : base(contextOptions)
        {

        }

        public DAAssistantDBContext(string connectionString)
        {
            this.connectionString = connectionString;
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this.connectionString);

               
            }
#if DEBUG
            optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] {
                                                new DebugLoggerProvider() }));
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobGroup>().ToTable(nameof(JobGroups));
            modelBuilder.Entity<Customer>().ToTable(nameof(Customers));
            modelBuilder.Entity<Document>().ToTable(nameof(Documents));
            modelBuilder.Entity<Item>().ToTable(nameof(Items));
            modelBuilder.Entity<Job>().ToTable(nameof(Jobs));


            modelBuilder.Entity<Job>().OwnsOne(j => j.CustomerInfo);
            modelBuilder.Entity<Note>().ToTable(nameof(Notes));
            modelBuilder.Entity<NotesReminderRecipient>().ToTable(nameof(NotesReminderRecipients));
            modelBuilder.Entity<Specification>().ToTable(nameof(Specifications));
            modelBuilder.Entity<Tag>().ToTable(nameof(Tags));
            modelBuilder.Entity<Topic>().ToTable(nameof(Topics));
            modelBuilder.Entity<ApplicationSetting>().ToTable(nameof(ApplicationSettings));
            modelBuilder.Entity<DocumentVersionMetadata>().ToTable(nameof(DocumentVersionMetadatas));
            modelBuilder.Entity<DocumentVersion>().ToTable(nameof(DocumentVersions));
            modelBuilder.Entity<DocumentVersionData>().ToTable(nameof(DocumentVersions));
            modelBuilder.Entity<StoreSyncLog>().ToTable(nameof(StoreSyncLogs));
            modelBuilder.Entity<StoreSyncLogEntry>().ToTable(nameof(StoreSyncLogEntries));
            modelBuilder.Entity<Device>().ToTable(nameof(Devices));
            modelBuilder.Entity<Thumbnail>().ToTable(nameof(Thumbnails));
            modelBuilder.Entity<ThumbnailData>().ToTable(nameof(Thumbnails));
            modelBuilder.Entity<WorkLog>().ToTable(nameof(WorkLogs));
            modelBuilder.Entity<WorkLogType>().ToTable(nameof(WorkLogTypes));
            modelBuilder.Entity<JobCode>().ToTable(nameof(JobCodes));
            modelBuilder.Entity<User>().ToTable(nameof(Users));

            modelBuilder.Entity<DayWorkLog>().ToTable(nameof(DayWorkLogs));
            modelBuilder.Entity<DayWorkType>().ToTable(nameof(DayWorkTypes));

            modelBuilder.Entity<Permission>().ToTable(nameof(Permissions));

            modelBuilder.Entity<UserCredential>().ToTable(nameof(UserCredentials));

            modelBuilder.Entity<Role>().ToTable(nameof(Roles));
            modelBuilder.Entity<JobType>().ToTable(nameof(JobTypes));
            modelBuilder.Entity<WeekWorkHours>().ToTable(nameof(WeekWorkHours));
            modelBuilder.Entity<HolidayDate>().ToTable(nameof(HolidayDates));

            modelBuilder.Entity<ExtraField>().ToTable(nameof(ExtraFields));
            modelBuilder.Entity<ExtraFieldDomainValue>().ToTable(nameof(ExtraFieldValues));
            modelBuilder.Entity<JobExtraFieldValue>().ToTable(nameof(JobExtraFields));



            #region Notes
            modelBuilder.Entity<DocumentsNotes>().HasKey(ot => new { ot.DocumentId, ot.NoteId });
            modelBuilder.Entity<DocumentsNotes>()
               .HasOne(ot => ot.Document)
               .WithMany(o => o.Notes)
               .HasForeignKey(ot => ot.DocumentId);

            modelBuilder.Entity<JobsNotes>().HasKey(ot => new { ot.JobId, ot.NoteId }); ;
            modelBuilder.Entity<JobsNotes>()
              .HasOne(ot => ot.Job)
              .WithMany(o => o.Notes)
              .HasForeignKey(ot => ot.JobId);

            modelBuilder.Entity<ItemsNotes>().HasKey(ot => new { ot.ItemId, ot.NoteId }); ;
            modelBuilder.Entity<ItemsNotes>()
              .HasOne(ot => ot.Item)
              .WithMany(o => o.Notes)
              .HasForeignKey(ot => ot.ItemId);

            modelBuilder.Entity<CustomersNotes>().HasKey(ot => new { ot.CustomerId, ot.NoteId }); ;
            modelBuilder.Entity<CustomersNotes>()
              .HasOne(ot => ot.Customer)
              .WithMany(o => o.Notes)
              .HasForeignKey(ot => ot.CustomerId);
            #endregion

            #region Topics
            modelBuilder.Entity<DocumentsTopics>().HasKey(ot => new { ot.DocumentId, ot.TopicId });
            modelBuilder.Entity<DocumentsTopics>()
               .HasOne(ot => ot.Document)
               .WithMany(o => o.Topics)
               .HasForeignKey(ot => ot.DocumentId);

            modelBuilder.Entity<NotesTopics>().HasKey(ot => new { ot.NoteId, ot.TopicId }); ;
            modelBuilder.Entity<NotesTopics>()
              .HasOne(ot => ot.Note)
              .WithMany(o => o.Topics)
              .HasForeignKey(ot => ot.NoteId);

            modelBuilder.Entity<ItemsTopics>().HasKey(ot => new { ot.ItemId, ot.TopicId }); ;
            modelBuilder.Entity<ItemsTopics>()
              .HasOne(ot => ot.Item)
              .WithMany(o => o.Topics)
              .HasForeignKey(ot => ot.ItemId);
            #endregion

            #region Tags
            modelBuilder.Entity<DocumentsTags>().HasKey(ot => new { ot.DocumentId, ot.TagId });
            modelBuilder.Entity<DocumentsTags>()
               .HasOne(ot => ot.Document)
               .WithMany(o => o.Tags)
               .HasForeignKey(ot => ot.DocumentId);

            modelBuilder.Entity<NotesTags>().HasKey(ot => new { ot.NoteId, ot.TagId }); ;
            modelBuilder.Entity<NotesTags>()
              .HasOne(ot => ot.Note)
              .WithMany(o => o.Tags)
              .HasForeignKey(ot => ot.NoteId);

            modelBuilder.Entity<ItemsTags>().HasKey(ot => new { ot.ItemId, ot.TagId }); ;
            modelBuilder.Entity<ItemsTags>()
              .HasOne(ot => ot.Item)
              .WithMany(o => o.Tags)
              .HasForeignKey(ot => ot.ItemId);

            modelBuilder.Entity<JobsTags>().HasKey(ot => new { ot.JobId, ot.TagId }); ;
            modelBuilder.Entity<JobsTags>()
              .HasOne(ot => ot.Job)
              .WithMany(o => o.Tags)
              .HasForeignKey(ot => ot.JobId);
            #endregion

            #region WorkLogs
            modelBuilder.Entity<DocumentsWorkLogs>().HasKey(ot => new { ot.DocumentId, ot.WorkLogId });
            modelBuilder.Entity<DocumentsWorkLogs>()
               .HasOne(ot => ot.Document)
               .WithMany(o => o.WorkLogs)
               .HasForeignKey(ot => ot.DocumentId);

            modelBuilder.Entity<ItemsWorkLogs>().HasKey(ot => new { ot.ItemId, ot.WorkLogId }); ;
            modelBuilder.Entity<ItemsWorkLogs>()
              .HasOne(ot => ot.Item)
              .WithMany(o => o.WorkLogs)
              .HasForeignKey(ot => ot.ItemId);

            modelBuilder.Entity<NotesWorkLogs>().HasKey(ot => new { ot.NoteId, ot.WorkLogId }); ;
            modelBuilder.Entity<NotesWorkLogs>()
              .HasOne(nt => nt.Note)
              .WithMany(n => n.WorkLogs)
              .HasForeignKey(nt => nt.NoteId);

            modelBuilder.Entity<JobsWorkLogs>().HasKey(ot => new { ot.JobId, ot.WorkLogId }); ;
            modelBuilder.Entity<JobsWorkLogs>()
              .HasOne(ot => ot.Job)
              .WithMany(o => o.WorkLogs)
              .HasForeignKey(ot => ot.JobId);


           
            #endregion

            #region JobCodes

            #endregion

            #region Credentials          

            modelBuilder.Entity<UserCredential>().HasIndex(nameof(UserCredential.Username)).IsUnique();
            #endregion
        }

    }
}
