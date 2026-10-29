using AdminApi.Domain.Entities;

namespace AdminApi.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Vehicles.Any())
        {
            return;
        }

        context.Vehicles.AddRange(
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "34BMW001",
                Brand = "BMW",
                Model = "320i",
                IsActive = true,
                Year = 2021
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "34BMW002",
                Brand = "BMW",
                Model = "520i",
                IsActive = true,
                Year = 2022
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "06BMW003",
                Brand = "BMW",
                Model = "X5",
                IsActive = false,
                Year = 2020
            },

            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "35MAZ001",
                Brand = "Mazda",
                Model = "CX-5",
                IsActive = true,
                Year = 2021
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "34MAZ002",
                Brand = "Mazda",
                Model = "Mazda 3",
                IsActive = true,
                Year = 2022
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "16MAZ003",
                Brand = "Mazda",
                Model = "CX-30",
                IsActive = false,
                Year = 2020
            },

            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "06TOY001",
                Brand = "Toyota",
                Model = "Corolla",
                IsActive = true,
                Year = 2022
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "34TOY002",
                Brand = "Toyota",
                Model = "Hilux",
                IsActive = true,
                Year = 2021
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "35TOY003",
                Brand = "Toyota",
                Model = "RAV4",
                IsActive = true,
                Year = 2023
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "41TOY004",
                Brand = "Toyota",
                Model = "Proace",
                IsActive = false,
                Year = 2019
            },

            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "34HON001",
                Brand = "Honda",
                Model = "Civic",
                IsActive = true,
                Year = 2021
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "06HON002",
                Brand = "Honda",
                Model = "CR-V",
                IsActive = true,
                Year = 2022
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "35HON003",
                Brand = "Honda",
                Model = "HR-V",
                IsActive = false,
                Year = 2020
            },
            new Vehicle
            {
                Id = Guid.NewGuid(),
                Plate = "16HON004",
                Brand = "Honda",
                Model = "Jazz",
                IsActive = true,
                Year = 2019
            }
        );
        context.SaveChanges();

    }
}