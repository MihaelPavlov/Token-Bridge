﻿// <auto-generated />
using System;
using Bridge_Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bridge_Project.Data.Migrations
{
    [DbContext(typeof(BridgeContext))]
    partial class BridgeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bridge_Project.Data.Models.BridgeEvent", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BlockNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChainName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimedFromId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<bool>("IsClaimed")
                        .HasColumnType("bit");

                    b.Property<string>("JsonData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublicKeySender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("RequiresClaiming")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ClaimedFromId");

                    b.ToTable("BridgeEvents");
                });

            modelBuilder.Entity("Bridge_Project.Data.Models.BridgeEvent", b =>
                {
                    b.HasOne("Bridge_Project.Data.Models.BridgeEvent", "ClaimedFrom")
                        .WithMany()
                        .HasForeignKey("ClaimedFromId");

                    b.Navigation("ClaimedFrom");
                });
#pragma warning restore 612, 618
        }
    }
}
