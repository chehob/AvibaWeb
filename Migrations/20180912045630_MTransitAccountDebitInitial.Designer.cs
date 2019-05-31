﻿// <auto-generated />
using System;
using AvibaWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AvibaWeb.Migrations
{
    [DbContext(typeof(AppIdentityDbContext))]
    [Migration("20180912045630_MTransitAccountDebitInitial")]
    partial class MTransitAccountDebitInitial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AvibaWeb.DomainModels.AcceptedCollector", b =>
                {
                    b.Property<string>("ProviderId");

                    b.Property<string>("CollectorId");

                    b.HasKey("ProviderId", "CollectorId");

                    b.HasIndex("CollectorId");

                    b.ToTable("AcceptedCollectors");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.AppRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<decimal>("Balance")
                        .HasColumnType("Money");

                    b.Property<int?>("BookingMappingId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<byte[]>("Photo");

                    b.Property<string>("Position");

                    b.Property<int?>("PushAllUserId");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserITN");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Card", b =>
                {
                    b.Property<int>("CardId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Number");

                    b.Property<string>("UserId");

                    b.HasKey("CardId");

                    b.HasIndex("UserId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Collection", b =>
                {
                    b.Property<int>("CollectionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("Money");

                    b.Property<string>("CollectorId");

                    b.Property<string>("Comment");

                    b.Property<string>("DeskIssuedId");

                    b.Property<int>("OperationType");

                    b.Property<int>("PaymentType");

                    b.Property<string>("ProviderId");

                    b.HasKey("CollectionId");

                    b.HasIndex("CollectorId");

                    b.HasIndex("DeskIssuedId");

                    b.HasIndex("ProviderId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.CollectionOperation", b =>
                {
                    b.Property<int>("CollectionOperationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CollectionId");

                    b.Property<DateTime>("OperationDateTime");

                    b.Property<int>("OperationTypeId");

                    b.HasKey("CollectionOperationId");

                    b.HasIndex("CollectionId");

                    b.HasIndex("OperationTypeId");

                    b.ToTable("CollectionOperations");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.CollectionOperationType", b =>
                {
                    b.Property<int>("CollectionOperationTypeId");

                    b.Property<string>("Description");

                    b.HasKey("CollectionOperationTypeId");

                    b.ToTable("CollectionOperationTypes");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Counterparty", b =>
                {
                    b.Property<string>("ITN");

                    b.Property<string>("Address");

                    b.Property<string>("BIK");

                    b.Property<string>("BankAccount");

                    b.Property<string>("BankName");

                    b.Property<string>("CorrespondentAccount");

                    b.Property<string>("Email");

                    b.Property<string>("KPP");

                    b.Property<string>("ManagementName");

                    b.Property<string>("ManagementPosition");

                    b.Property<string>("Name");

                    b.Property<string>("OGRN");

                    b.Property<string>("Phone");

                    b.Property<int?>("TypeId");

                    b.HasKey("ITN");

                    b.HasIndex("TypeId");

                    b.ToTable("Counterparties");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.CounterpartyType", b =>
                {
                    b.Property<int>("CounterpartyTypeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsActive");

                    b.HasKey("CounterpartyTypeId");

                    b.ToTable("CounterpartyTypes");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Desk", b =>
                {
                    b.Property<string>("DeskId")
                        .HasMaxLength(10);

                    b.Property<string>("Description")
                        .HasMaxLength(50);

                    b.Property<int?>("GroupId");

                    b.Property<bool>("IsActive");

                    b.HasKey("DeskId");

                    b.HasIndex("GroupId");

                    b.ToTable("Desks");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.DeskGroup", b =>
                {
                    b.Property<int>("DeskGroupId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name");

                    b.HasKey("DeskGroupId");

                    b.ToTable("DeskGroups");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Expenditure", b =>
                {
                    b.Property<int>("ExpenditureId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("Money");

                    b.Property<int>("DeskGroupId");

                    b.Property<string>("Name");

                    b.Property<int>("ObjectId");

                    b.Property<int>("TypeId");

                    b.HasKey("ExpenditureId");

                    b.HasIndex("DeskGroupId");

                    b.HasIndex("ObjectId");

                    b.HasIndex("TypeId");

                    b.ToTable("Expenditures");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.ExpenditureObject", b =>
                {
                    b.Property<int>("ExpenditureObjectId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsActive");

                    b.HasKey("ExpenditureObjectId");

                    b.ToTable("ExpenditureObjects");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.ExpenditureOperation", b =>
                {
                    b.Property<int>("ExpenditureOperationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ExpenditureId");

                    b.Property<DateTime>("OperationDateTime");

                    b.Property<int>("OperationTypeId");

                    b.HasKey("ExpenditureOperationId");

                    b.HasIndex("ExpenditureId");

                    b.ToTable("ExpenditureOperations");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.ExpenditureType", b =>
                {
                    b.Property<int>("ExpenditureTypeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsActive");

                    b.HasKey("ExpenditureTypeId");

                    b.ToTable("ExpenditureTypes");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.FinancialAccount", b =>
                {
                    b.Property<int>("FinancialAccountId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Balance")
                        .HasColumnType("Money");

                    b.Property<string>("Description");

                    b.Property<bool>("IsActive");

                    b.Property<int>("OrganizationId");

                    b.HasKey("FinancialAccountId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("FinancialAccounts");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.FinancialAccountOperation", b =>
                {
                    b.Property<int>("FinancialAccountOperationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<string>("CounterpartyId");

                    b.Property<string>("Description");

                    b.Property<int>("FinancialAccountId");

                    b.Property<DateTime>("InsertDateTime");

                    b.Property<DateTime>("OperationDateTime");

                    b.Property<string>("UserId")
                        .HasMaxLength(128);

                    b.HasKey("FinancialAccountOperationId");

                    b.HasIndex("CounterpartyId");

                    b.HasIndex("FinancialAccountId");

                    b.HasIndex("UserId");

                    b.ToTable("FinancialAccountOperations");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Organization", b =>
                {
                    b.Property<int>("OrganizationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsActive");

                    b.HasKey("OrganizationId");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.TransitAccount", b =>
                {
                    b.Property<int>("TransitAccountId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Balance")
                        .HasColumnType("Money");

                    b.Property<bool>("IsActive");

                    b.HasKey("TransitAccountId");

                    b.ToTable("TransitAccounts");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.TransitAccountDebit", b =>
                {
                    b.Property<int>("TransitAccountDebitId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("OperationDateTime");

                    b.Property<int>("TransitAccountId");

                    b.HasKey("TransitAccountDebitId");

                    b.HasIndex("TransitAccountId");

                    b.ToTable("TransitAccountDebits");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.VDeskBalance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Balance");

                    b.Property<string>("DeskId");

                    b.Property<string>("DeskName");

                    b.HasKey("Id");

                    b.ToTable("VDeskBalances");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.AcceptedCollector", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppUser", "Collector")
                        .WithMany()
                        .HasForeignKey("CollectorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AvibaWeb.DomainModels.AppUser", "Provider")
                        .WithMany("AcceptedCollectors")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Card", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppUser", "User")
                        .WithMany("Cards")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Collection", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppUser", "Collector")
                        .WithMany()
                        .HasForeignKey("CollectorId");

                    b.HasOne("AvibaWeb.DomainModels.Desk", "DeskIssued")
                        .WithMany()
                        .HasForeignKey("DeskIssuedId");

                    b.HasOne("AvibaWeb.DomainModels.AppUser", "Provider")
                        .WithMany()
                        .HasForeignKey("ProviderId");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.CollectionOperation", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.Collection", "Collection")
                        .WithMany("Operations")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AvibaWeb.DomainModels.CollectionOperationType", "OperationType")
                        .WithMany()
                        .HasForeignKey("OperationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Counterparty", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.CounterpartyType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Desk", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.DeskGroup", "Group")
                        .WithMany("Desks")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.Expenditure", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.DeskGroup", "DeskGroup")
                        .WithMany()
                        .HasForeignKey("DeskGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AvibaWeb.DomainModels.ExpenditureObject", "Object")
                        .WithMany()
                        .HasForeignKey("ObjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AvibaWeb.DomainModels.ExpenditureType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.ExpenditureOperation", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.Expenditure", "Expenditure")
                        .WithMany("Operations")
                        .HasForeignKey("ExpenditureId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.FinancialAccount", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.Organization", "Organization")
                        .WithMany("Accounts")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.FinancialAccountOperation", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.Counterparty", "Counterparty")
                        .WithMany()
                        .HasForeignKey("CounterpartyId");

                    b.HasOne("AvibaWeb.DomainModels.FinancialAccount", "Account")
                        .WithMany()
                        .HasForeignKey("FinancialAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AvibaWeb.DomainModels.AppUser", "PayeeUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("AvibaWeb.DomainModels.TransitAccountDebit", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.TransitAccount", "Account")
                        .WithMany()
                        .HasForeignKey("TransitAccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AvibaWeb.DomainModels.AppUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("AvibaWeb.DomainModels.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
