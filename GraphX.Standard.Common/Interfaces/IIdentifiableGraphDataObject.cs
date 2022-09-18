using GraphX.Common.Enums;
using System.Drawing;

namespace GraphX.Common.Interfaces
{
    public interface IIdentifiableGraphDataObject
    {
        /// <summary>
        /// Unique object identifier
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// Skip object in algorithm calc and visual control generation
        /// </summary>
        ProcessingOptionEnum SkipProcessing { get; set; }

        Color Color { get; }
    }
}