// Utilities/EnumBindingSourceExtension.cs
using System;
using System.Windows.Markup;

namespace TaskManager.Utilities
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public Type EnumType
        {
            get => _enumType;
            set
            {
                if (value == _enumType) return;
                if (value != null && !value.IsEnum)
                    throw new ArgumentException("Type must be an enum");
                _enumType = value;
            }
        }

        public EnumBindingSourceExtension() { }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("Type must be an enum");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_enumType == null)
                throw new InvalidOperationException("EnumType must be specified");

            return Enum.GetValues(_enumType);
        }
    }
}