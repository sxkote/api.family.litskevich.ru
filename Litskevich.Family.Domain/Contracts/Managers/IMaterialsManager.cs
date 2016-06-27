using Litskevich.Family.Domain.Entities;
using SXCore.Domain.Contracts;
using System;

namespace Litskevich.Family.Domain.Contracts.Managers
{
    public interface IMaterialsManager : ICoreManager
    {
        Material GetMaterial(long id);

        Material CreateMaterial(long articleID, string fileCode);
        Material UpdateMaterial(long materialID, DateTime? date = null, string title = "", string comment = "");
        void TransformMaterial(long materialID, string method, string argument);
        void DeleteMaterial(long materialID);
    }
}