using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminApi.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{

    public Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AnyAsync(
                x => x.Email == email,
                cancellationToken);
    }

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> GetByPasswordResetTokenHashAsync(
    string tokenHash,
    CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(
            x => x.PasswordResetTokenHash == tokenHash,
            cancellationToken);
    }

    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}