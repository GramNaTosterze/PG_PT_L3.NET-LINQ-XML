using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace C__Lab3
{
    [XmlRoot(ElementName = "engine")]
    public class Engine : IComparable
    {
        public double displacement;
        public double horsePower;
        [XmlAttribute]
        public string model;
        public Engine(double displacement, double horsePower, string model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }
        public Engine() { }

        public int CompareTo(Object obj)
        {
            Engine en = (Engine)obj;
            return this.horsePower.CompareTo(en.horsePower);
        }
    }
}
