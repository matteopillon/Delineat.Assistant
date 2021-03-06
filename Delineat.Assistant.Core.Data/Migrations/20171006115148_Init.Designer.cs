﻿// <auto-generated />
using Delineat.Assistant.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    [DbContext(typeof(DAAssistantDBContext))]
    [Migration("20171006115148_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ApplicationSetting", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("ApplicationSettings");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Description");

                    b.Property<string>("Domain");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.CustomersNotes", b =>
                {
                    b.Property<int>("CustomerId");

                    b.Property<int>("NoteId");

                    b.HasKey("CustomerId", "NoteId");

                    b.HasIndex("NoteId");

                    b.ToTable("CustomersNotes");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Device", b =>
                {
                    b.Property<int>("DeviceId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<int?>("ExportSyncId");

                    b.Property<string>("HostName");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<string>("Name");

                    b.Property<string>("PushUrl");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("DeviceId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Document", b =>
                {
                    b.Property<int>("DocumentId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<int>("ItemId");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("DocumentId");

                    b.HasIndex("ItemId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentsNotes", b =>
                {
                    b.Property<int>("DocumentId");

                    b.Property<int>("NoteId");

                    b.HasKey("DocumentId", "NoteId");

                    b.HasIndex("NoteId");

                    b.ToTable("DocumentsNotes");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentsTags", b =>
                {
                    b.Property<int>("DocumentId");

                    b.Property<int>("TagId");

                    b.HasKey("DocumentId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("DocumentsTags");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentsTopics", b =>
                {
                    b.Property<int>("DocumentId");

                    b.Property<int>("TopicId");

                    b.HasKey("DocumentId", "TopicId");

                    b.HasIndex("TopicId");

                    b.ToTable("DocumentsTopics");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentVersion", b =>
                {
                    b.Property<int>("DocumentVersionId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int?>("DocumentId");

                    b.Property<int?>("ExportSyncId");

                    b.Property<string>("Extension");

                    b.Property<string>("Filename");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("DocumentVersionId");

                    b.HasIndex("DocumentId");

                    b.ToTable("DocumentVersions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("DocumentVersion");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentVersionMetadata", b =>
                {
                    b.Property<int>("MetadataId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<int?>("DocumentVersionId");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<string>("Key");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<string>("Value");

                    b.HasKey("MetadataId");

                    b.HasIndex("DocumentVersionId");

                    b.ToTable("DocumentVersionMetadatas");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Description");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<string>("ItemSource");

                    b.Property<int>("JobId");

                    b.Property<string>("Path");

                    b.Property<DateTime>("ReferenceDate");

                    b.Property<DateTime?>("UpdateDate");

                    b.Property<string>("Who");

                    b.HasKey("ItemId");

                    b.HasIndex("JobId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ItemsNotes", b =>
                {
                    b.Property<int>("ItemId");

                    b.Property<int>("NoteId");

                    b.HasKey("ItemId", "NoteId");

                    b.HasIndex("NoteId");

                    b.ToTable("ItemsNotes");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ItemsTags", b =>
                {
                    b.Property<int>("ItemId");

                    b.Property<int>("TagId");

                    b.HasKey("ItemId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ItemsTags");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ItemsTopics", b =>
                {
                    b.Property<int>("ItemId");

                    b.Property<int>("TopicId");

                    b.HasKey("ItemId", "TopicId");

                    b.HasIndex("TopicId");

                    b.ToTable("ItemsTopics");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<int?>("CustomerId");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Description");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("GroupId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<string>("Path");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("JobId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("GroupId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.JobGroup", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsCurrent");

                    b.Property<string>("Name");

                    b.Property<string>("Path");

                    b.HasKey("GroupId");

                    b.ToTable("JobGroups");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.JobsNotes", b =>
                {
                    b.Property<int>("JobId");

                    b.Property<int>("NoteId");

                    b.HasKey("JobId", "NoteId");

                    b.HasIndex("NoteId");

                    b.ToTable("JobsNotes");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.JobsTags", b =>
                {
                    b.Property<int>("JobId");

                    b.Property<int>("TagId");

                    b.HasKey("JobId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("JobsTags");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Note", b =>
                {
                    b.Property<int>("NoteId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<int>("Level");

                    b.Property<string>("NoteType");

                    b.Property<DateTime?>("RemindedDate");

                    b.Property<DateTime?>("ReminderDate");

                    b.Property<int>("ReminderType");

                    b.Property<string>("Text");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("NoteId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.NotesReminderRecipient", b =>
                {
                    b.Property<int>("RecipientId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Email");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<int?>("NoteId");

                    b.Property<byte[]>("SentDate");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("RecipientId");

                    b.HasIndex("NoteId");

                    b.ToTable("NotesReminderRecipients");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.NotesTags", b =>
                {
                    b.Property<int>("NoteId");

                    b.Property<int>("TagId");

                    b.HasKey("NoteId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("NotesTags");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.NotesTopics", b =>
                {
                    b.Property<int>("NoteId");

                    b.Property<int>("TopicId");

                    b.HasKey("NoteId", "TopicId");

                    b.HasIndex("TopicId");

                    b.ToTable("NotesTopics");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Specification", b =>
                {
                    b.Property<int>("SpecificationId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CustomerId");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Description");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<int?>("NoteId");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("SpecificationId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("NoteId");

                    b.ToTable("Specifications");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.StoreSyncLog", b =>
                {
                    b.Property<int>("SyncId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Completed");

                    b.Property<int?>("GroupId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<string>("TargetName");

                    b.HasKey("SyncId");

                    b.HasIndex("GroupId");

                    b.ToTable("StoreSyncLogs");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.StoreSyncLogEntry", b =>
                {
                    b.Property<int>("LogEntryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Message");

                    b.Property<int?>("StoreSyncLogSyncId");

                    b.HasKey("LogEntryId");

                    b.HasIndex("StoreSyncLogSyncId");

                    b.ToTable("StoreSyncLogEntries");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Description");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Thumbnail", b =>
                {
                    b.Property<int>("ThumbnailId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int?>("DocumentVersionId");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<string>("Title");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("ThumbnailId");

                    b.HasIndex("DocumentVersionId");

                    b.ToTable("Thumbnails");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Thumbnail");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Topic", b =>
                {
                    b.Property<int>("TopicId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color");

                    b.Property<DateTime?>("DeleteDate");

                    b.Property<string>("Description");

                    b.Property<int?>("ExportSyncId");

                    b.Property<int?>("ImportSyncId");

                    b.Property<DateTime>("InsertDate");

                    b.Property<int?>("JobId");

                    b.Property<DateTime?>("UpdateDate");

                    b.HasKey("TopicId");

                    b.HasIndex("JobId");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentVersionData", b =>
                {
                    b.HasBaseType("Delineat.Workflow.Core.SqlServer.Models.DocumentVersion");

                    b.Property<byte[]>("Data");

                    b.ToTable("DocumentVersions");

                    b.HasDiscriminator().HasValue("DocumentVersionData");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ThumbnailData", b =>
                {
                    b.HasBaseType("Delineat.Workflow.Core.SqlServer.Models.Thumbnail");

                    b.Property<byte[]>("Image");

                    b.ToTable("Thumbnails");

                    b.HasDiscriminator().HasValue("ThumbnailData");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.CustomersNotes", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Customer", "Customer")
                        .WithMany("Notes")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany("Customers")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Document", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Item", "Item")
                        .WithMany("Documents")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentsNotes", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Document", "Document")
                        .WithMany("Notes")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany("Documents")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentsTags", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Document", "Document")
                        .WithMany("Tags")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Tag", "Tag")
                        .WithMany("Documents")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentsTopics", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Document", "Document")
                        .WithMany("Topics")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Topic", "Topic")
                        .WithMany("Documents")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentVersion", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Document", "Document")
                        .WithMany("Versions")
                        .HasForeignKey("DocumentId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.DocumentVersionMetadata", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.DocumentVersion", "DocumentVersion")
                        .WithMany()
                        .HasForeignKey("DocumentVersionId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Item", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Job", "Job")
                        .WithMany("Items")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ItemsNotes", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Item", "Item")
                        .WithMany("Notes")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany("Items")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ItemsTags", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Item", "Item")
                        .WithMany("Tags")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Tag", "Tag")
                        .WithMany("Items")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.ItemsTopics", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Item", "Item")
                        .WithMany("Topics")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Topic", "Topic")
                        .WithMany("Items")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Job", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Customer", "Customer")
                        .WithMany("Job")
                        .HasForeignKey("CustomerId");

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.JobGroup", "Group")
                        .WithMany("Jobs")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.JobsNotes", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Job", "Job")
                        .WithMany("Notes")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany("Jobs")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.JobsTags", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Job", "Job")
                        .WithMany("Tags")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Tag", "Tag")
                        .WithMany("Jobs")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.NotesReminderRecipient", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany("NotesReminderRecipients")
                        .HasForeignKey("NoteId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.NotesTags", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany("Tags")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Tag", "Tag")
                        .WithMany("Notes")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.NotesTopics", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany("Topics")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Topic", "Topic")
                        .WithMany("Notes")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Specification", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Customer")
                        .WithMany("Specifications")
                        .HasForeignKey("CustomerId");

                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Note", "Note")
                        .WithMany()
                        .HasForeignKey("NoteId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.StoreSyncLog", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.JobGroup", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.StoreSyncLogEntry", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.StoreSyncLog")
                        .WithMany("Entries")
                        .HasForeignKey("StoreSyncLogSyncId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Thumbnail", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.DocumentVersion", "DocumentVersion")
                        .WithMany("Thumbnails")
                        .HasForeignKey("DocumentVersionId");
                });

            modelBuilder.Entity("Delineat.Workflow.Core.SqlServer.Models.Topic", b =>
                {
                    b.HasOne("Delineat.Workflow.Core.SqlServer.Models.Job", "Job")
                        .WithMany("Topics")
                        .HasForeignKey("JobId");
                });
#pragma warning restore 612, 618
        }
    }
}
