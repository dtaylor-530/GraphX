using System;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    internal struct StringValue
    {
        public StringValue(string value)
        {
            if (value.Length > 32)
            {
                throw new Exception("Length exceeds limit of 32");
            }
            Value = new Guid(value);

        }

        public Guid Value { get; }

        public static implicit operator string(StringValue author)
        {
            return author.Value.ToString();
        }
    }
}
