using System.Collections.Generic;

namespace DesktopApi.Data.Model.Interfaces
{
    public interface IDataStorage
    {
        List<Elem> Elems { get; set; }

        void Serialize();
        void Deserialize();
    }
}