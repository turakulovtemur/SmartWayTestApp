using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartWayTestAppplication.Models;
using SmartWayTestAppplication.Seed.Models.Sample;

namespace SmartWayTestAppplication.Seed
{
    public static class SeedDataHelpers
    {
        public static async Task EnsureSeedDataAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var serviceScope = services.CreateScope();

            using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var sampleDataConfiguration = configuration.GetSection(nameof(SampleDataConfiguration))
                    .Get<SampleDataConfiguration>();

            if (sampleDataConfiguration == null) return;

            await EnsureSeedSampleData(
                context,
                userManager,
                roleManager,
                sampleDataConfiguration,
                passwordHasher);
        }

        private static async Task EnsureSeedSampleData(
            ApplicationContext context,
            UserManager<User> userManager, RoleManager<Role> roleManager,
            SampleDataConfiguration sampleDataConfiguration, IPasswordHasher<User> passwordHasher)
        {
            var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Roles
                if (!await roleManager.Roles.AnyAsync() && sampleDataConfiguration.Roles.Any())
                    foreach (var role in sampleDataConfiguration.Roles.Select(r => new Role { Name = r.Name }))
                        await roleManager.CreateAsync(role);

                // Users
                if (!await userManager.Users.AnyAsync() && sampleDataConfiguration.Users.Any())
                    foreach (var user in sampleDataConfiguration.Users)
                    {
                        var identityUser = new User
                        {
                            Login = user.UserName,
                            UserName = user.UserName,
                            Password = user.Password,
                            Name = user.UserName
                        };

                        var hashedPassword = passwordHasher.HashPassword(identityUser, user.Password);
                        identityUser.SecurityStamp = Guid.NewGuid().ToString();
                        identityUser.PasswordHash = hashedPassword;

                        var result = await userManager.CreateAsync(identityUser);

                        if (!result.Succeeded) continue;

                        foreach (var role in user.Roles) await userManager.AddToRoleAsync(identityUser, role);
                    }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
