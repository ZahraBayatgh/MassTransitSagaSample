﻿// <auto-generated />
using System;
using AzureProcessor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AzureProcessor.Migrations
{
    [DbContext(typeof(ProductCatalogDbContext))]
    partial class ProductCatalogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Contracts.Dtos.ProductDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("InitialOnHand")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductStatus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ProductDto");
                });

            modelBuilder.Entity("Contracts.StateMachines.ProductCatalogState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CurrentState")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("CorrelationId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductCatalogState");
                });

            modelBuilder.Entity("Contracts.StateMachines.ProductCatalogState", b =>
                {
                    b.HasOne("Contracts.Dtos.ProductDto", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });
#pragma warning restore 612, 618
        }
    }
}
