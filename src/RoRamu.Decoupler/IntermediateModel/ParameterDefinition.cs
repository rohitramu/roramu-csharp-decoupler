namespace RoRamu.Decoupler
{
    /// <summary>
    /// Represents an input parameter in an operation.
    /// </summary>
    public class ParameterDefinition
    {
        /// <summary>
        /// The name of the parameter.  May be null in situations where parameters are purely positional.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of the parameter.  May be null in situations where parameters do not have a well-defined type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Creates a new parameter definition.
        /// </summary>
        /// <param name="name">The name of the parameter.  May be null in situations where parameters are purely positional.</param>
        /// <param name="type">The type of the parameter.  May be null in situations where parameters do not have a well-defined type.</param>
        public ParameterDefinition(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}