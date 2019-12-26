using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using ESavingsFactory;
using System.DirectoryServices;


namespace TestingEnv
{

	class Program
	{

		static void Main (string[] args)
		{

			//var cals = Helpers.ONCalendar.GetByYear(2019);

			//foreach (var cal in cals)
			//{
			//	var startAndEnd = cal.Value;


			//	Console.WriteLine(cal.Key +" STart: "+ startAndEnd["start"] + "End: "+ startAndEnd["end"]);
			//}


			var cal2 = Helpers.ONCalendar.GetByDate(new DateTime(2019, 12, 31));

			foreach (var cal in cal2)
			{
				var startAndEnd = cal.Value;


				Console.WriteLine(cal.Key +" STart: "+ startAndEnd["start"] + "End: "+ startAndEnd["end"]);
			}

			int currentWW = cal2.FirstOrDefault().Key + 1;

			Console.WriteLine(currentWW);


			//DateTime dec31 = new DateTime(2019, 1, 1);
			//DayOfWeek Dec31Day = dec31.DayOfWeek;

			//Console.WriteLine(Dec31Day);

			//var cal = Helpers.ONCalendar.GetByWW(1, 2019);

			//Console.WriteLine(cal["start"] +"-"+ cal["end"]);


			Console.Read();

		}
	}
}
