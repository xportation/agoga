using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AgogaSim
{
    public class ConverterUtil
    {
        public static TimeSpan TimeFromString(string time)
        {
            if (time.Length != 4)
                return TimeSpan.Zero;

            int hour = Convert.ToInt32(time.Substring(0, 2));
            int minutes = Convert.ToInt32(time.Substring(2, 2));
            return new TimeSpan(hour, minutes, 0);
        }
    
        public static DateTime DateFromString(string date)
		{
			if (date.Length != 10)
                return new DateTime();

			int year = Convert.ToInt32(date.Substring(0, 4));
			int month = Convert.ToInt32(date.Substring(5, 2));
			int day = Convert.ToInt32(date.Substring(8, 2));
            return new DateTime(year, month, day);
		}
	}

	public class Company
	{
        [JsonProperty("empresa")]
		public string Id { get; set; }

        [JsonProperty("nome")]
		public string Name { get; set; }
		
        [JsonProperty("inicia_dia")]
        public int StartDay { get; set; }

        [JsonProperty("inicia_mes_anterior")]
		public bool StartMonthBefore { get; set; }
	}

	public class Detail
	{
        string valueStr = null;

		[JsonProperty("valor")]
		public string Value
		{
            get { return valueStr; }
			set
			{
				valueStr = value;
                valueStr = valueStr.Insert(valueStr.Length-2, ":");
			}
		}
        
        [JsonProperty("tipo")]
		public string Description { get; set; }
	}

	public class Punch
	{
        string timeStr = null;

		[JsonProperty("hora")]
		public string TimeStr 
        { 
            set 
            {
                timeStr = value;
                Time = ConverterUtil.TimeFromString(timeStr);
            }
        }

		[JsonIgnore]
        public TimeSpan Time { get; set; }

		[JsonProperty("equipamento")]
		public string Device { get; set; }
	}

	public class DayReport
	{
        [JsonIgnore]
		public DateTime Day { get; set; }

        public void SetDay(string day)
        {
            Day = ConverterUtil.DateFromString(day);
        }

		[JsonProperty("batidas")]
		public IList<Punch> Punches { get; set; }

		[JsonProperty("resultado")]
		public IList<Detail> Resume { get; set; }

		[JsonProperty("justificativa")]
		public string Justification { get; set; }

        IList<string> hireHours = null;

		[JsonProperty("HORAS_CONTRATUAIS")]
        public IList<string> HireHours 
        { 
            set
            {
                hireHours = value;
                var hours = new List<TimeSpan>();
                foreach (string h in hireHours)
                {
                    var hour = ConverterUtil.TimeFromString(h.Replace(":",""));
                    hours.Add(hour);
                }
                if (hours.Count == 4)
                {
                    DailyInterval = hours[2] - hours[1];
                    DailyHours = (hours[1] - hours[0]) + (hours[3] - hours[2]);
                }
            }
        }

        [JsonIgnore]
		public TimeSpan DailyHours { get; set; }
        [JsonIgnore]
		public TimeSpan DailyInterval { get; set; }
	}

	public class Person
	{
        [JsonProperty("matricula")]
		public string Id { get; set; }

        [JsonProperty("nome")]
		public string Name { get; set; }

        [JsonProperty("cargo")]
		public string JobDescription { get; set; }

        string hireDateStr = null;

		[JsonProperty("dt_admissao")]
		public string HireDateStr
		{
			set
			{
				hireDateStr = value;
                HireDate = ConverterUtil.DateFromString(hireDateStr);
			}
		}

        [JsonIgnore]
		public DateTime HireDate { get; set; }
	}

	public class Report
	{
        private IList<DayReport> days;

        [JsonProperty("empresa")]
		public Company Company { get; set; }

		[JsonProperty("funcionario")]
		public Person Person { get; set; }

		[JsonProperty("resultado")]
		public IList<Detail> Resume { get; set; }

		[JsonIgnore]
		public IList<DayReport> Days 
        { 
            get { return days; } 
            set
            {
                days = value;
                foreach (DayReport day in days)
                {
                    if (day.Day == DateTime.Today.AddDays(-3))
                        Today = day;
                }
            }
        }

        [JsonIgnore]
        public DayReport Today { get; set; }
	}
}
