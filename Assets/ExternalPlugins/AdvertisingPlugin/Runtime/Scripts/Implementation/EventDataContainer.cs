using System.Collections.Generic;

namespace Modules.Advertising
{
    public static class EventDataContainer
    {
        private static Dictionary<string, object> _data;


        public static Dictionary<string, object> Data
        {
            get => _data;
            set => _data = value;
        }
    }
}