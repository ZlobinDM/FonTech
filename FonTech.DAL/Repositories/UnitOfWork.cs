using FonTech.Domain.Entity;
using FonTech.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FonTech.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    // private bool _disposed;

    public UnitOfWork(ApplicationDbContext context, IBaseRepository<User> users, IBaseRepository<UserRole> userRoles)
    {
        _context = context;
        Users = users;
        UserRoles = userRoles;
    }

    // public void Dispose()
    // {
    //     GC.SuppressFinalize(this);
    // }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public IBaseRepository<User> Users { get; set; }
    public IBaseRepository<UserRole> UserRoles { get; set; }
}