using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }

            var user = new User
            {
                Username = "admin",
                Role = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<User>();

            user.PasswordHash =
                passwordHasher.HashPassword(
                    user,
                    "Admin@123");

            context.Users.Add(user);

            await context.SaveChangesAsync();
        }
    }
}
