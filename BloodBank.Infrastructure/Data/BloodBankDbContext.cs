using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Infrastructure.Data
{
    public class BloodBankDbContext : IdentityDbContext<User>
    {
        public BloodBankDbContext ( DbContextOptions<BloodBankDbContext> options )
            : base( options )
        {
        }

        public DbSet<Donation> Donations { get; set; }
        public DbSet<BloodTest> BloodTests { get; set; }
        public DbSet<BloodUnit> BloodUnits { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating( modelBuilder );

            // Configure entities
            modelBuilder.Entity<Donation>( entity =>
            {
                entity.HasOne( d => d.Donor )
                    .WithMany( u => u.Donations )
                    .HasForeignKey( d => d.DonorId )
                    .OnDelete( DeleteBehavior.Restrict );
            } );

            modelBuilder.Entity<Appointment>( entity =>
            {
                entity.HasOne( a => a.Donor )
                    .WithMany( u => u.Appointments )
                    .HasForeignKey( a => a.DonorId )
                    .OnDelete( DeleteBehavior.Restrict );
            } );

            modelBuilder.Entity<BloodTest>( entity =>
            {
                entity.HasOne( bt => bt.Donation )
                    .WithOne( d => d.BloodTest )
                    .HasForeignKey<BloodTest>( bt => bt.DonationId )
                    .OnDelete( DeleteBehavior.Cascade );
            } );

            modelBuilder.Entity<BloodUnit>( entity =>
            {
                entity.HasOne( bu => bu.Donation )
                    .WithOne( d => d.BloodUnit )
                    .HasForeignKey<BloodUnit>( bu => bu.DonationId )
                    .OnDelete( DeleteBehavior.Cascade );
            } );

            // Add any additional configurations here
        }
    }
}
