using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogics.Config
{
    public class ChartConfigModel
    {
        public struct DiagramSeries
        {
            public string Name { get; set; }
            public double[] Values { get; set; }
        }

        public string DiagTitle { get; set; }

        public List<DiagramSeries> Data { get; set; }
    }
}
