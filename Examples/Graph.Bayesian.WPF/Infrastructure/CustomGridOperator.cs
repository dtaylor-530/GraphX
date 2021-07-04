using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Bayesian.WPF.Infrastructure
{
    public class CustomPropertyGridOperator : PropertyGridOperator
    {
        protected override PropertyItem CreateCore(PropertyDescriptor pd, PropertyDescriptorCollection properties)
        {         
            var core= base.CreateCore(pd, properties);
            core.HeaderPlacement = PropertyTools.DataAnnotations.HeaderPlacement.Collapsed;
            return core;
        }

        protected override string GetDisplayName(PropertyDescriptor pd, Type declaringType)
        {
            // Use the property name as display name - this will be passed to the GetLocalizedString later
            return pd.Name;
        }

        protected override string GetLocalizedString(string key, Type declaringType)
        {
            // Add a star to show that we have handled this
            // A localization mechanism can be used to localize the strings
            return key + "*";
        }
    }
}
