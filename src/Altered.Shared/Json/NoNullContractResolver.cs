using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared.Json
{
    /// <summary>
	/// A <see cref="IContractResolver"/> that creates sub-documents instead of returning null
	/// </summary>
	public sealed class NoNullContractResolver : IContractResolver
    {
        /// <inheritdoc />
        public JsonContract ResolveContract(Type type)
        {
            IContractResolver defaultContractResolver = new DefaultContractResolver();
            JsonContract jsonContract = defaultContractResolver.ResolveContract(type);

            if (jsonContract is JsonObjectContract jsonObjectContract)
            {
                foreach (JsonProperty jsonProperty in jsonObjectContract.Properties)
                {
                    if (!jsonProperty.PropertyType.IsValueType && jsonProperty.PropertyType != typeof(string))
                    {
                        jsonProperty.ValueProvider = new NoNullValueProvider(jsonProperty.PropertyType, jsonProperty.ValueProvider);
                    }
                }
            }

            return jsonContract;
        }
    }
}
