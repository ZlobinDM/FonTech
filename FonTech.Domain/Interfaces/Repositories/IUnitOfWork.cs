using FonTech.Domain.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace FonTech.Domain.Interfaces.Repositories;

public interface IUnitOfWork 
{
    Task<IDbContextTransaction> BeginTransactionAsync();

    Task<int> SaveChangesAsync();

    IBaseRepository<User> Users { get; set; }
    IBaseRepository<UserRole> UserRoles { get; set; }
}