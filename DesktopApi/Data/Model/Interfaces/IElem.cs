
namespace DesktopApi.Data.Model.Interfaces
{
    public interface IElem
    {
        int Id { get; set; }
        string Name { get; set; }
        string Path { get; set; }
        string Icon { get; set; }
        string Category { get; set; }
        PathType Type { get; set; }
        uint UseCount { get; set; }
        bool IsFavourite { get; set; }

        bool Exist();
    }
}