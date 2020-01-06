﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserManagement.API.Infrastructure;

namespace UserManagement.API.Migrations
{
    [DbContext(typeof(UserManagementContext))]
    partial class UserManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("UserManagement.API.Entities.AccessRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParticipantWorkflows")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Participants")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubmissionData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TestParticipantsAndData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AccessRole");
                });

            modelBuilder.Entity("UserManagement.API.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AllCampaignsAccess")
                        .HasColumnType("bit");

                    b.Property<bool>("AllSitesAccess")
                        .HasColumnType("bit");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("EmptySiteAccess")
                        .HasColumnType("bit");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Language")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Locked")
                        .HasColumnType("bit");

                    b.Property<string>("MobilePhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("UserManagement.API.Entities.UserAccessRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccessRoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "AccessRoleId");

                    b.HasIndex("AccessRoleId");

                    b.ToTable("UserAccessRole");
                });

            modelBuilder.Entity("UserManagement.API.Entities.UserCampaign", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CampaignId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "CampaignId");

                    b.ToTable("UserCampaign");
                });

            modelBuilder.Entity("UserManagement.API.Entities.UserGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserGroup");
                });

            modelBuilder.Entity("UserManagement.API.Entities.UserUserGroup", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "UserGroupId");

                    b.HasIndex("UserGroupId");

                    b.ToTable("UserUserGroup");
                });

            modelBuilder.Entity("UserManagement.API.Entities.UserAccessRole", b =>
                {
                    b.HasOne("UserManagement.API.Entities.AccessRole", "AccessRole")
                        .WithMany("UserAccessRoles")
                        .HasForeignKey("AccessRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UserManagement.API.Entities.User", "User")
                        .WithMany("UserAccessRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserManagement.API.Entities.UserCampaign", b =>
                {
                    b.HasOne("UserManagement.API.Entities.User", "User")
                        .WithMany("UserCampaigns")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserManagement.API.Entities.UserUserGroup", b =>
                {
                    b.HasOne("UserManagement.API.Entities.UserGroup", "UserGroup")
                        .WithMany("Users")
                        .HasForeignKey("UserGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UserManagement.API.Entities.User", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
