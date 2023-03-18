using PlatformService.Data;
using PlatformService.Interfaces;
using PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Repositories;
public class PlatformRepo : IPlatformRepo
{
    private readonly AppDbContext _context;

    public PlatformRepo(AppDbContext context)
    {
        _context = context;
    }
    public void Create(PlatformModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        _context.Platforms.Add(model);
    }

    public IEnumerable<PlatformModel> GetAll()
    {
        return _context.Platforms.ToList<PlatformModel>();
    }

    public PlatformModel GetById(int id)
    {
        return _context.Platforms.FirstOrDefault(obj => obj.Id == id) ?? default!;
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() > 0;
    }
}