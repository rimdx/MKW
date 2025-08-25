using MKW.Core.Storage;
using MKW.GUI.Images;

namespace MKW.GUI
{
    public class DatabaseUserViewModel
    {
        public required bool IsAdmin { get; init; }
        public required UserId Id { get; init; }
        public required string Name { get; init; }

        public object Icon => IsAdmin ? new Admin() : new User();
    }
}
