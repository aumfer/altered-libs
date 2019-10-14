using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared.Json
{
    /// <summary>
	/// A value provider that returns a new value instead of returning null
	/// </summary>
	public sealed class NoNullValueProvider : IValueProvider
    {
        /// <summary>
        /// The type of object to return a value for
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// A internal value provider whose null value handling is overridden
        /// </summary>
        private readonly IValueProvider _jsonPropertyValueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoNullValueProvider"/> class.
        /// </summary>
        /// <param name="type">The type of object to return a value for</param>
        /// <param name="jsonPropertyValueProvider">A internal value provider whose null value handling is overridden</param>
        public NoNullValueProvider(Type type, IValueProvider jsonPropertyValueProvider)
        {
            _type = type;
            _jsonPropertyValueProvider = jsonPropertyValueProvider;
        }

        /// <inheritdoc />
        public void SetValue(object target, object value)
        {
            _jsonPropertyValueProvider.SetValue(target, value);
        }

        /// <inheritdoc />
        public object GetValue(object target)
        {
            object value;

            if (_jsonPropertyValueProvider.GetValue(target) != null)
            {
                value = _jsonPropertyValueProvider.GetValue(target);
            }
            else
            {
                value = Activator.CreateInstance(_type);
                SetValue(target, value);
            }

            return value;
        }
    }
}
