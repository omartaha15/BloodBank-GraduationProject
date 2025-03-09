using BloodBank.Core.Entities;
using BloodBank.Core.Entities.BloodBank.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
        public DbSet<BloodRequest> BloodRequests { get; set; }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating( modelBuilder );

            // Donation configuration
            modelBuilder.Entity<Donation>( entity =>
            {
                entity.HasOne( d => d.Donor )
                      .WithMany( u => u.Donations )
                      .HasForeignKey( d => d.DonorId )
                      .OnDelete( DeleteBehavior.Restrict );
            } );

            // BloodTest configuration
            modelBuilder.Entity<BloodTest>( entity =>
            {
                // Relationship: one-to-one between Donor and BloodTest.
                entity.HasOne( bt => bt.Donor )
                      .WithOne( u => u.BloodTest )
                      .HasForeignKey<BloodTest>( bt => bt.DonorId )
                      .OnDelete( DeleteBehavior.Cascade );

                // New: Relationship with Hospital.
                // A hospital can have many blood tests; we disable cascade delete for this relation.
                entity.HasOne( bt => bt.Hospital )
                      .WithMany() // No navigation property on User for this relationship.
                      .HasForeignKey( bt => bt.HospitalId )
                      .OnDelete( DeleteBehavior.Restrict );
            } );

            // BloodUnit configuration
            modelBuilder.Entity<BloodUnit>( entity =>
            {
                entity.HasOne( bu => bu.Donation )
                      .WithOne( d => d.BloodUnit )
                      .HasForeignKey<BloodUnit>( bu => bu.DonationId )
                      .OnDelete( DeleteBehavior.Cascade );
            } );
        }
    }
}
