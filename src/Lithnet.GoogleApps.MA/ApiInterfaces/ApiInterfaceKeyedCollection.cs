using System.Collections.ObjectModel;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceKeyedCollection : KeyedCollection<string, IApiInterface>
    {
        /// <summary>When implemented in a derived class, extracts the key from the specified element.</summary>
        /// <returns>The key for the specified element.</returns>
        /// <param name="item">The element from which to extract the key.</param>
        protected override string GetKeyForItem(IApiInterface item)
        {
            return item.Api;
        }
    }
}
