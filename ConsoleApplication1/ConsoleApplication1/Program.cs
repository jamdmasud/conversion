using Suprema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program : UnitTest
    {
        private AccessControl ac = new AccessControl();

        protected override void runImpl(UInt32 deviceID)
        {
            ac.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for access control";
            program.run();
            Console.ReadKey();
        }
    }
}
