namespace RoRamu.Decoupler.DotNet
{
    using System;

    public class ParameterValue
    {
        public Type Type { get; }

        public string Name { get; }

        public object Value { get; }

        public static ParameterValue Create<T>(string name, T value)
        {
            return new ParameterValue(typeof(T), name, value);
        }

        private ParameterValue(Type type, string name, object value)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Name = name;
            this.Value = value;
        }


        public bool TryGetValue<T>(out object value)
        {
            if (this.Value is T val)
            {
                value = val;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}