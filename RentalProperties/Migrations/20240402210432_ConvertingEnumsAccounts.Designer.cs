﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentalProperties.DATA;

#nullable disable

namespace RentalProperties.Migrations
{
    [DbContext(typeof(RentalPropertiesDBContext))]
    [Migration("20240402210432_ConvertingEnumsAccounts")]
    partial class ConvertingEnumsAccounts
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RentalProperties.Models.Apartment", b =>
                {
                    b.Property<int>("ApartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApartmentId"));

                    b.Property<bool>("AnimalsAccepted")
                        .HasColumnType("bit");

                    b.Property<string>("ApartmentNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<int>("NbOfBaths")
                        .HasColumnType("int");

                    b.Property<int>("NbOfBeds")
                        .HasColumnType("int");

                    b.Property<int>("NbOfParkingSpots")
                        .HasColumnType("int");

                    b.Property<decimal>("PriceAnnounced")
                        .HasColumnType("numeric(8, 2)");

                    b.Property<int>("PropertyId")
                        .HasColumnType("int");

                    b.HasKey("ApartmentId");

                    b.HasIndex("PropertyId");

                    b.ToTable("Apartments");
                });

            modelBuilder.Entity("RentalProperties.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentId"));

                    b.Property<int>("ApartmentId")
                        .HasColumnType("int");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("VisitDate")
                        .HasColumnType("date");

                    b.HasKey("AppointmentId");

                    b.HasIndex("ApartmentId");

                    b.HasIndex("TenantId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("RentalProperties.Models.EventInProperty", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventId"));

                    b.Property<int?>("ApartmentId")
                        .HasColumnType("int");

                    b.Property<string>("EventDescription")
                        .HasColumnType("text");

                    b.Property<string>("EventTitle")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<int>("PropertyId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("ReportDate")
                        .HasColumnType("date");

                    b.HasKey("EventId");

                    b.HasIndex("ApartmentId");

                    b.HasIndex("PropertyId");

                    b.ToTable("EventsInProperties");
                });

            modelBuilder.Entity("RentalProperties.Models.MessageFromTenant", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MessageId"));

                    b.Property<string>("AnswerFromManager")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ApartmentId")
                        .HasColumnType("int");

                    b.Property<string>("MessageSent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("MessageId");

                    b.HasIndex("ApartmentId");

                    b.HasIndex("TenantId");

                    b.ToTable("MessagesFromTenants");
                });

            modelBuilder.Entity("RentalProperties.Models.Rental", b =>
                {
                    b.Property<int>("TenantId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("ApartmentId")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<DateOnly>("FirstDayRental")
                        .HasColumnType("date")
                        .HasColumnOrder(3);

                    b.Property<DateOnly>("LastDayRental")
                        .HasColumnType("date");

                    b.Property<decimal>("PriceRent")
                        .HasColumnType("numeric(8, 2)");

                    b.Property<int>("RentalStatus")
                        .HasColumnType("int");

                    b.HasKey("TenantId", "ApartmentId", "FirstDayRental")
                        .HasName("PK_Rentals");

                    b.HasIndex("ApartmentId");

                    b.ToTable("Rentals");
                });

            modelBuilder.Entity("RentalProperties.Property", b =>
                {
                    b.Property<int>("PropertyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PropertyId"));

                    b.Property<string>("AddressNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressStreet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ManagerId")
                        .HasColumnType("int");

                    b.Property<string>("Neighbourhood")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PropertyId");

                    b.HasIndex("ManagerId")
                        .IsUnique();

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("RentalProperties.UserAccount", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateOnly>("DateCreated")
                        .HasColumnType("date");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserStatus")
                        .HasColumnType("int");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("RentalProperties.Models.Apartment", b =>
                {
                    b.HasOne("RentalProperties.Property", "Property")
                        .WithMany("Apartments")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Property");
                });

            modelBuilder.Entity("RentalProperties.Models.Appointment", b =>
                {
                    b.HasOne("RentalProperties.Models.Apartment", "Apartment")
                        .WithMany("Appointments")
                        .HasForeignKey("ApartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RentalProperties.UserAccount", "Tenant")
                        .WithMany("Appointments")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Apartment");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("RentalProperties.Models.EventInProperty", b =>
                {
                    b.HasOne("RentalProperties.Models.Apartment", "Apartment")
                        .WithMany("EventsInApartment")
                        .HasForeignKey("ApartmentId");

                    b.HasOne("RentalProperties.Property", "Property")
                        .WithMany("EventsInProperty")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Apartment");

                    b.Navigation("Property");
                });

            modelBuilder.Entity("RentalProperties.Models.MessageFromTenant", b =>
                {
                    b.HasOne("RentalProperties.Models.Apartment", "Apartment")
                        .WithMany("Messages")
                        .HasForeignKey("ApartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RentalProperties.UserAccount", "Tenant")
                        .WithMany("Messages")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Apartment");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("RentalProperties.Models.Rental", b =>
                {
                    b.HasOne("RentalProperties.Models.Apartment", "Apartment")
                        .WithMany("Rentals")
                        .HasForeignKey("ApartmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("RentalProperties.UserAccount", "Tenant")
                        .WithMany("Rentals")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Apartment");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("RentalProperties.Property", b =>
                {
                    b.HasOne("RentalProperties.UserAccount", "Manager")
                        .WithOne()
                        .HasForeignKey("RentalProperties.Property", "ManagerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("RentalProperties.Models.Apartment", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("EventsInApartment");

                    b.Navigation("Messages");

                    b.Navigation("Rentals");
                });

            modelBuilder.Entity("RentalProperties.Property", b =>
                {
                    b.Navigation("Apartments");

                    b.Navigation("EventsInProperty");
                });

            modelBuilder.Entity("RentalProperties.UserAccount", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Messages");

                    b.Navigation("Rentals");
                });
#pragma warning restore 612, 618
        }
    }
}
