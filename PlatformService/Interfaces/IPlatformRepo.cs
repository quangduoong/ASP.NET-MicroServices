using PlatformService.Models;

namespace PlatformService.Interfaces;
public interface IPlatformRepo
{
    bool SaveChanges();
    IEnumerable<PlatformModel> GetAll();
    PlatformModel GetById(int id);
    void Create(PlatformModel model);
}