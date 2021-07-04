using System;

namespace Graph.Bayesian.WPF.Models.Vertices
{
    public class MonitorPropertyChangeAttribute : Attribute
    {
        public MonitorPropertyChangeAttribute(bool registerChanges)
        {
            RegisterChanges = registerChanges;
        }

        public bool RegisterChanges { get; }
    }
}