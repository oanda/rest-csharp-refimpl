using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTest.Testing;

namespace ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var tests = new RestTest();
			var task = tests.RunTest();
			task.Wait();
			Console.WriteLine("Tests complete");
			Console.ReadLine();
		}
	}
}
