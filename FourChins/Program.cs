[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace FourChins
{
    class Program
    {
        static void Main(string[] args)
        {
            FourChins chins = new FourChins();
            chins.DoTheThing();
        }

    }
}
