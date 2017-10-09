using System.IO;
using System.Xml.Serialization;
using EBookReader.Model;

namespace EBookReader.Util
{
    public class Serializer
    {
        public static void Serialize(State state)
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(State));
                writer = new StreamWriter("..\\..\\Resources\\State.xml", false);
                serializer.Serialize(writer, state);
            }
            finally
            {
                writer?.Close();
            }
        }

        public static State Deserialize()
        {
            if (!File.Exists("..\\..\\Resources\\State.xml"))
            {
                return null;
            }

            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(State));
                reader = new StreamReader("..\\..\\Resources\\State.xml");
                return (State) serializer.Deserialize(reader);
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}