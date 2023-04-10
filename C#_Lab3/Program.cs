using C__Lab3;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

internal class Program
{
    public static List<Car> myCars = new List<Car>()
        {
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };
    static void Main(string[] args)
    {
        // Zad 1
        var A6 =
            from car in myCars
            where car.model == "A6"
            select new
            {
                engineType = (car.motor.model == "TDI" ? "disel" : "petrol"),
                hppl = car.motor.horsePower / car.motor.displacement
            };

        var GroupByEngineType =
            from a in A6
            group a by new { a.engineType } into g
            select new
            {
                g.Key.engineType,
                avr = g.Sum(s => s.hppl) / g.Count()
            };
        Console.WriteLine("Zad 1: \n");
        foreach (var engine in GroupByEngineType)
            Console.WriteLine("{0}: {1} \n", engine.engineType, engine.avr);


        // Zad 2
        SerializationAndDesentralization();
        // Zad 3
        XPathExpressions();
        // Zad 4
        createXmlFromLinq();
        // Zad 5
        createHtmlTable();
        // Zad 6
        modifyXml();
    }
    static void SerializationAndDesentralization()
    {
        // serializacja
        string path = Path.Combine(Directory.GetCurrentDirectory(), "CarsCollection.xml");
        XmlSerializer xmlSer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
        using (TextWriter writer = new StreamWriter(path))
            xmlSer.Serialize(writer, myCars);


        // deserializacja
        List<Car> deserializedCars = new List<Car>();
        using (Stream reader = new FileStream(path, FileMode.Open))
            deserializedCars = (List<Car>)xmlSer.Deserialize(reader);

        foreach (Car car in deserializedCars)
            Console.WriteLine(car.model);
    }
    static void XPathExpressions()
    {
        XElement rootNode = XElement.Load("CarsCollection.xml");
        string XPathExpression1 = "sum(car/engine[@model != 'TDI']/horsePower) div count(car/engine[@model != 'TDI']/horsePower)";
        double avgHP = (double)rootNode.XPathEvaluate(XPathExpression1);
        Console.WriteLine("Zad3a: {0}", avgHP);

        string XPathExpression2 = "car[not(model = following-sibling::car/model)]";
        IEnumerable<XElement> models = rootNode.XPathSelectElements(XPathExpression2);
        Console.WriteLine("Zad3b: ");
        foreach (var model in models)
            Console.WriteLine((string)model.Element((XName)"model"));
    }
    static void createXmlFromLinq()
    {
        IEnumerable<XElement> nodes = myCars.Select(car =>
            new XElement("car",
                new XElement("model", car.model),
                new XElement("engine",
                    new XElement("displacement", car.motor.displacement),
                    new XElement("horsePower", car.motor.horsePower),
                    new XAttribute("model", car.motor.model)
                ),
                new XElement("year", car.year)
        ));
        XElement root = new XElement("cars", nodes);
        root.Save("CarsFromLinq.xml");

    }

    static void createHtmlTable()
    {
        XElement template = XElement.Load("template.html");

        IEnumerable<XElement> cars = myCars.Select(car =>
            new XElement("tr",
                new XElement("td", new XAttribute("style", "border: 2px solid black"), car.model),
                new XElement("td", new XAttribute("style", "border: 2px solid black"), car.motor.model),
                new XElement("td", new XAttribute("style", "border: 2px solid black"), car.motor.displacement),
                new XElement("td", new XAttribute("style", "border: 2px solid black"), car.motor.horsePower),
                new XElement("td", new XAttribute("style", "border: 2px solid black"), car.year)
            ));

        XElement table = new XElement("table", new XAttribute("style", "border: 2px solid black"), cars);
        template.Element("{http://www.w3.org/1999/xhtml}body").Add(table);
        template.Save("XHTMLTable.html");
    }

    static void modifyXml()
    {
        XmlDocument carsXml = new XmlDocument();
        carsXml.Load("CarsCollection.xml");


        foreach (XmlNode carNode in carsXml.SelectNodes("cars/car"))
        {

            XmlNode horsePower = carNode.SelectSingleNode("engine/horsePower");
            XmlNode hp = carsXml.CreateElement("hp");
            hp.InnerText = horsePower.InnerText;

            XmlNode year = carNode.SelectSingleNode("year");
            XmlElement modelNode = (XmlElement)carNode.SelectSingleNode("model");
            modelNode.SetAttribute("year", year.InnerText);

            XmlNode engineNode = carNode.SelectSingleNode("engine");
            engineNode.InsertBefore(hp, horsePower);
            carNode.RemoveChild(year);
            engineNode.RemoveChild(horsePower);
        }

        carsXml.Save("ModdedCarsCollection.xml");
    }
}
