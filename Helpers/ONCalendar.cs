using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public static class DateTimeExtension
	{
		public static DateTime StartOfWeek (this DateTime dt, DayOfWeek startOfWeek)
		{
			int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
			return dt.AddDays(-1 * diff).Date;
		}
		public static DateTime EndOfWeek (this DateTime dt, DayOfWeek startOfWeek)
		{
			int diff = (7 - (dt.DayOfWeek - startOfWeek)) % 7;
			return dt.AddDays(1 * diff).Date;
		}
	}

	public static class ONCalendar
	{
		public static Dictionary<int, Dictionary<string, string>> GetByYear (int year)
		{

			Dictionary<int, Dictionary<string, string>> WWs = new Dictionary<int, Dictionary<string, string>>();

			DateTime jan1 = new DateTime(year, 1, 1);
			DateTime dec31 = new DateTime(year, 1, 1);

			DayOfWeek Jan1Day = jan1.DayOfWeek;
			DayOfWeek Dec31Day = dec31.DayOfWeek;

			int numberDaysInAYear = (new DateTime(year, 12, 31)).DayOfYear;


			Dictionary<string, string> startAndEnd;

			int ww = 0;

			while (numberDaysInAYear > 0)
			{
				startAndEnd = new Dictionary<string, string>();
				ww = ((numberDaysInAYear / 7) + 1);


				if (ww == 1)
				{
					startAndEnd.Add("start", jan1.AddDays(numberDaysInAYear - 1).StartOfWeek(Jan1Day).ToString("yyyy-MM-dd"));
					startAndEnd.Add("startDay", jan1.AddDays(numberDaysInAYear - 1).StartOfWeek(Jan1Day).DayOfWeek.ToString());
				}
				else
				{
					startAndEnd.Add("start", jan1.AddDays(numberDaysInAYear - 1).StartOfWeek(DayOfWeek.Saturday).ToString("yyyy-MM-dd"));
					startAndEnd.Add("startDay", jan1.AddDays(numberDaysInAYear - 1).StartOfWeek(DayOfWeek.Saturday).DayOfWeek.ToString());
				}


				if (ww == 52)
				{
					startAndEnd.Add("end", jan1.AddDays((numberDaysInAYear + 7) - 1).EndOfWeek(Dec31Day).ToString("yyyy-MM-dd"));
					startAndEnd.Add("endDay", jan1.AddDays(numberDaysInAYear - 1).EndOfWeek(Dec31Day).DayOfWeek.ToString());
				}
				else
				{
					startAndEnd.Add("end", jan1.AddDays(numberDaysInAYear - 1).EndOfWeek(DayOfWeek.Friday).ToString("yyyy-MM-dd"));
					startAndEnd.Add("endDay", jan1.AddDays(numberDaysInAYear - 1).EndOfWeek(DayOfWeek.Friday).DayOfWeek.ToString());
				}


				WWs.Add(ww, startAndEnd);


				numberDaysInAYear -= 7;
			}

			return WWs;
		}


		public static Dictionary<string, string> GetByWW (int ww, int year)
		{
			var wws = GetByYear(year);
			return wws[ww];
		}


		public static Dictionary<int, Dictionary<string, string>> GetByDate (DateTime inputDate)
		{
			Dictionary<int, Dictionary<string, string>> WWs = new Dictionary<int, Dictionary<string, string>>();

			Dictionary<string, string> startAndEnd = new Dictionary<string, string>();

			startAndEnd.Add("start", inputDate.StartOfWeek(DayOfWeek.Saturday).ToString("yyyy-MM-dd"));
			startAndEnd.Add("startDay", inputDate.StartOfWeek(DayOfWeek.Saturday).DayOfWeek.ToString());


			startAndEnd.Add("end", inputDate.EndOfWeek(DayOfWeek.Friday).ToString("yyyy-MM-dd"));
			startAndEnd.Add("endDay", inputDate.EndOfWeek(DayOfWeek.Friday).DayOfWeek.ToString());


			int ww = 1;

			if (inputDate.DayOfWeek == DayOfWeek.Saturday || inputDate.DayOfWeek == DayOfWeek.Sunday)
			{
				ww = (int)Math.Round((double)((double)inputDate.DayOfYear / (double)7), 2) + 2;
			}
			else if (inputDate.DayOfWeek == DayOfWeek.Friday)
			{
				ww = (int)Math.Round((double)((double)inputDate.DayOfYear / (double)7));
			}
			else
			{
				ww = (int)Math.Round((double)((double)inputDate.DayOfYear / (double)7)) + 1;
			}

			WWs.Add(ww, startAndEnd);

			return WWs;
		}


		public static int GetWWOnlyByDate (DateTime inputDate)
		{
			int ww = 1;

			if (inputDate.DayOfWeek == DayOfWeek.Saturday || inputDate.DayOfWeek == DayOfWeek.Sunday)
			{
				ww = (int)Math.Round((double)((double)inputDate.DayOfYear / (double)7), 2) + 2;
			}
			else if (inputDate.DayOfWeek == DayOfWeek.Friday)
			{
				ww = (int)Math.Round((double)((double)inputDate.DayOfYear / (double)7));
			}
			else
			{
				ww = (int)Math.Round((double)((double)inputDate.DayOfYear / (double)7)) + 1;
			}

			return ww;
		}

	}
}
