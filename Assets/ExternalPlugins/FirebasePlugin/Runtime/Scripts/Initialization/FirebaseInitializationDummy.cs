using System.Threading.Tasks;


namespace Modules.Firebase.Initialization
{
    internal class FirebaseInitializationDummy : IFirebaseInitialization
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
    }
}
