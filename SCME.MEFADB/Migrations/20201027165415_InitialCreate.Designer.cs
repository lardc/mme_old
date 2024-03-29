﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SCME.MEFADB;

namespace SCME.MEFADB.Migrations
{
    [DbContext(typeof(MonitoringContext))]
    [Migration("20201027165415_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SCME.MEFADB.Tables.MmeCode", b =>
                {
                    b.Property<int>("MmeCodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MME_CODE_ID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnName("MME_CODE")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MmeCodeId");

                    b.ToTable("MME_CODES");
                });

            modelBuilder.Entity("SCME.MEFADB.Tables.MonitoringEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Data1")
                        .HasColumnType("bigint");

                    b.Property<long>("Data2")
                        .HasColumnType("bigint");

                    b.Property<long>("Data3")
                        .HasColumnType("bigint");

                    b.Property<string>("Data4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MmeCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MonitoringEventTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MonitoringEventTypeId");

                    b.ToTable("MonitoringEvents");
                });

            modelBuilder.Entity("SCME.MEFADB.Tables.MonitoringEventType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EventName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MonitoringEventTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            EventName = "MME_ERROR"
                        },
                        new
                        {
                            Id = 2,
                            EventName = "MME_START"
                        },
                        new
                        {
                            Id = 3,
                            EventName = "MME_TEST"
                        },
                        new
                        {
                            Id = 4,
                            EventName = "MME_SYNC"
                        });
                });

            modelBuilder.Entity("SCME.MEFADB.Tables.MonitoringStat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("KeyData")
                        .HasColumnType("datetime2");

                    b.Property<string>("MmeCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MonitoringStatTypeId")
                        .HasColumnType("int");

                    b.Property<int>("ValueData")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MonitoringStatTypeId");

                    b.ToTable("MonitoringStats");
                });

            modelBuilder.Entity("SCME.MEFADB.Tables.MonitoringStatType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("StatName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MonitoringStatTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            StatName = "DAY_HOURS"
                        },
                        new
                        {
                            Id = 2,
                            StatName = "TOTAL_HOURS"
                        },
                        new
                        {
                            Id = 3,
                            StatName = "LAST_START_HOURS"
                        });
                });

            modelBuilder.Entity("SCME.MEFADB.Tables.MonitoringEvent", b =>
                {
                    b.HasOne("SCME.MEFADB.Tables.MonitoringEventType", "MonitoringEventType")
                        .WithMany()
                        .HasForeignKey("MonitoringEventTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SCME.MEFADB.Tables.MonitoringStat", b =>
                {
                    b.HasOne("SCME.MEFADB.Tables.MonitoringStatType", "MonitoringStatType")
                        .WithMany()
                        .HasForeignKey("MonitoringStatTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
