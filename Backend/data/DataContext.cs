using OnlineClinicBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinicBooking.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        { }

        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Clinic> Clinics { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One Category has many Appointments
            modelBuilder.Entity<Category>()
                .HasMany(category => category.Appointments)
                .WithOne(Appointment => Appointment.Category)
                .HasForeignKey(Appointment => Appointment.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // One Clinic has many Doctors
            modelBuilder.Entity<Clinic>()
                .HasMany(clinic => clinic.Doctors)
                .WithOne(Doctor => Doctor.Clinic)
                .HasForeignKey(Doctor => Doctor.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);

            // One Clinic has many Appointments
            modelBuilder.Entity<Clinic>()
                .HasMany(clinic => clinic.Appointments)
                .WithOne(Appointment => Appointment.Clinic)
                .HasForeignKey(Appointment => Appointment.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);

            // One Appointment has one Clinic
            modelBuilder.Entity<Appointment>()
                .HasOne(appointment => appointment.Clinic)
                .WithMany(clinic => clinic.Appointments)
                .HasForeignKey(Appointment => Appointment.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);

            // One Appointment has one Doctor
            modelBuilder.Entity<Appointment>()
                .HasOne(appointment => appointment.Doctor)
                .WithMany(doctor => doctor.Appointments)
                .HasForeignKey(Appointment => Appointment.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            // One Appointment has one Category
            modelBuilder.Entity<Appointment>()
                .HasOne(appointment => appointment.Category)
                .WithMany(category => category.Appointments)
                .HasForeignKey(Appointment => Appointment.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // One Appointment has one Patient
            modelBuilder.Entity<Appointment>()
                .HasOne(appointment => appointment.Patient)
                .WithMany(patient => patient.Appointments)
                .HasForeignKey(Appointment => Appointment.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            PopulateDB(modelBuilder);
        }
        private void PopulateDB(ModelBuilder modelBuilder)
        {
            // Populate Specialitys
            modelBuilder.Entity<Speciality>().HasData(
                new Speciality { Id = 1, Name = "Surgery" },
                new Speciality { Id = 2, Name = "Family Medicine" },
                new Speciality { Id = 3, Name = "Emergency Medicine" },
                new Speciality { Id = 4, Name = "Cardiology" },
                new Speciality { Id = 5, Name = "Nephrology" }
            );

            // Populate Patients
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, Firstname = "Harry", Lastname = "Potter", Email = "Harry.Potter@Hogwarts.wiz", Birthdate = new DateTime(1980, 7, 31) },
                new Patient { Id = 2, Firstname = "Jon", Lastname = "Snow", Email = "JonSnow@WinterIs.com", Birthdate = new DateTime(1950, 12, 14) },
                new Patient { Id = 3, Firstname = "Monkey", Lastname = "Luffy", Email = "monkeyDLuffy@PirateKing.sea", Birthdate = new DateTime(2005, 5, 5) },
                new Patient { Id = 4, Firstname = "Scott", Lastname = "McAll", Email = "ScottMcAll@TrueAlpha.com", Birthdate = new DateTime(1994, 9, 16) }
            );

            // Populate Doctors
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, Firstname = "Gregory", Lastname = "House", ClinicId = 1, SpecialityId = 5 },
                new Doctor { Id = 2, Firstname = "Shaun", Lastname = "Murphy", ClinicId = 2, SpecialityId = 1 },
                new Doctor { Id = 3, Firstname = "John", Lastname = "Watson", ClinicId = 3, SpecialityId = 2 },
                new Doctor { Id = 4, Firstname = "Stephen", Lastname = "Strange", ClinicId = 2, SpecialityId = 4 },
                new Doctor { Id = 5, Firstname = "Leonard", Lastname = "McCoy", ClinicId = 1, SpecialityId = 3 },
                new Doctor { Id = 6, Firstname = "Miranda", Lastname = "Bailey", ClinicId = 3, SpecialityId = 1 }

            );

            // Populate Clinics
            modelBuilder.Entity<Clinic>().HasData(
                new Clinic { Id = 1, Name = "Clinic A" },
                new Clinic { Id = 2, Name = "Clinic B" },
                new Clinic { Id = 3, Name = "Clinic C" }
            );

            // Populate Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Emergency" },
                new Category { Id = 2, Name = "Check up" },
                new Category { Id = 3, Name = "Follow up" },
                new Category { Id = 4, Name = "Diagnosis" }
            );

            var today = DateTime.Today;
            // Populate Appointments
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, Date = today.AddHours(12), CategoryId = 1, ClinicId = 1, DoctorId = 1, PatientId = 1, PatientNote = "Was dead, but resurrected. Lasting effects?" },
                new Appointment { Id = 2, Date = today.AddHours(12).AddMinutes(30), CategoryId = 2, ClinicId = 2, DoctorId = 2, PatientId = 2, PatientNote = "Was dead, but resurrected. Lasting effects?" },
                new Appointment { Id = 3, Date = today.AddHours(13), CategoryId = 3, ClinicId = 3, DoctorId = 3, PatientId = 3, }
            );
        }
    }
}


