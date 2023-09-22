using System.Threading.Tasks;


namespace Modules.Firebase
{
    internal interface IFirebaseModule
    {
        int InitializationPriority { get; }
        Task Initialize(LLFirebaseSettings settings = null);
    }
}
