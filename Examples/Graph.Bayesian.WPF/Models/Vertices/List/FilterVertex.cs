using Graph.Bayesian.WPF.Infrastructure;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Fasterflect;
using Graph.Bayesian.WPF.ViewModel;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    public record SelectKeyRecordFilter(string Value, string[]? Properties = default, bool IsCaseSensitive = false, bool IsRegex = false) : IFilter<SelectableKeyRecord>
    {
        public bool Filter(SelectableKeyRecord record)
        {
            if (Properties == null)
            {
                if (IsRegex)
                {
                    return Regex.Match(record.Key, Value).Success;
                }
                return record.Key.Contains(Value, IsCaseSensitive ?  StringComparison.InvariantCulture: StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                bool match = false;
                var em = Properties.OfType<string>().GetEnumerator();

                while (!match && em.MoveNext() && record.GetPropertyValue(em.Current).ToString() is { } prop)
                {
                    if (IsRegex)
                        match = Regex.Match(prop, Value).Success;
                    else
                        match = prop.Contains(Value, IsCaseSensitive ? StringComparison.InvariantCulture: StringComparison.InvariantCultureIgnoreCase);
                }
                return match;
            }
        }
    }

    public interface IFilter<T>
    {
        bool Filter(T value);
    }


    public record FilterMessage(string From, string To, DateTime Sent, Filter Filter) : Message(From, To, Sent, Filter);


    public class FilterVertex : Vertex
    {
        private string input = string.Empty;

        public FilterVertex()
        {
            Out.OnNext(CreateMessage(input));
        }

        public string FilterText
        {
            get => input;
            set
            {
                input = value;
                OnPropertyChanged(nameof(FilterText));
                Out.OnNext(CreateMessage(value));
            }
        }

        protected virtual Message CreateMessage(string value)
        {
            return new ListInputMessage(this.ID.ToString(), string.Empty, DateTime.Now, new FilterInput<SelectableKeyRecord>(new SelectKeyRecordFilter(value)));
        }
    }
}
