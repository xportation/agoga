using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            get { return timeStr; }
            set 
            {
                Time = ConverterUtil.TimeFromString(value);
				timeStr = value.Insert(value.Length - 2, ":");
            }
        }

		[JsonIgnore]
        public TimeSpan Time { get; set; }

		[JsonProperty("equipamento")]
		public string Device { get; set; }
	}

	public class DayReport
	{
		IList<string> hireHours = null;
		IList<Detail> resume = null;
		IList<Punch> punches = null;

        [JsonIgnore]
		public DateTime Day { get; set; }

        public void SetDay(string day)
        {
            Day = ConverterUtil.DateFromString(day);
        }

		[JsonProperty("batidas")]
		public IList<Punch> Punches 
        { 
            get { return punches;  } 
            set
            {
                punches = (from punch in value orderby punch.Time select punch).ToList();
				
                var strBuilder = new StringBuilder();
				for (var i = 0; i < punches.Count(); i++)
				{
					var punch = punches[i];
                    strBuilder.AppendFormat("{0}", punch.TimeStr);
					if (i < punches.Count() - 1)
                        strBuilder.Append(", ");
				}
                if (strBuilder.Length > 0)
                    PunchesStr = strBuilder.ToString();
                else
                    PunchesStr = "--:--";
                
				defineWorkedHours();
            }
        }

		[JsonProperty("resultado")]
		public IList<Detail> Resume 
        { 
            get { return resume; } 
            set
            {
                resume = value;

				var strBuilder = new StringBuilder();
                for (var i = 0; i < resume.Count(); i++)
                {
                    var detail = resume[i];
                    strBuilder.AppendFormat("{0,8} {1}", detail.Value, detail.Description);
                    if (i < resume.Count()-1)
                        strBuilder.AppendLine();
                }
                if (strBuilder.Length > 0)
                    ResumeStr = strBuilder.ToString();
                else 
                    ResumeStr = null;
            }
        }

		[JsonProperty("status")]
		public IList<string> Status { get; set; }

		[JsonProperty("justificativa")]
		public string Justification { get; set; }

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
                DayHiredHours = TimeSpan.Zero;
                DayHiredInterval = TimeSpan.Zero;
                if (hours.Count == 4)
                {
                    DayHiredInterval = hours[2] - hours[1];
                    DayHiredHours = (hours[1] - hours[0]) + (hours[3] - hours[2]);
                }
            }
        }

        void defineWorkedHours() 
        { 
            if (this.Punches.Count <= 1)
            {
                WorkedHours = TimeSpan.Zero;
                IntervalDid = TimeSpan.Zero;
				return;               
            }

			var periods = new List<TimeSpan>();
            for (var index = 0; index < Punches.Count - 1; index += 2)
			{
                var diff = Punches[index + 1].Time.Subtract(Punches[index].Time);
				periods.Add(diff);
			}

			var timeWorked = new TimeSpan(periods[0].Hours, periods[0].Minutes, periods[0].Seconds);
			for (var index = 1; index < periods.Count; index++)
				timeWorked += periods[index];

			WorkedHours = timeWorked.Duration();

            if (Punches.Count >= 3)
                IntervalDid = Punches.Last().Time.Subtract(Punches.First().Time).Subtract(WorkedHours);
        }

        public TimeSpan GetExpectedLeavingTime()
        {
            if (this.Punches == null || this.Punches.Count == 0)
                return TimeSpan.Zero;

            var interval = IntervalDid;
            if (interval == TimeSpan.Zero)
                interval = DayHiredInterval;

            return Punches.First().Time + DayHiredHours + interval;
        }

        public static DayReport Zero()
        {
            return new DayReport { 
                Day = DateTime.Today, 
                WorkedHours = TimeSpan.Zero, 
                IntervalDid = TimeSpan.Zero, 
                PunchesStr = "--:--" 
            };
        }

		[JsonIgnore]
		public string PunchesStr { get; set; }
		[JsonIgnore]
		public string ResumeStr { get; set; }
		[JsonIgnore]
		public TimeSpan WorkedHours { get; set; }
		[JsonIgnore]
		public TimeSpan IntervalDid { get; set; }
		[JsonIgnore]
        public TimeSpan DayHiredHours { get; set; }
        [JsonIgnore]
        public TimeSpan DayHiredInterval { get; set; }
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
                var hired = new DateTime();
                if (Person != null)
                    hired = Person.HireDate;
                
                Today = DayReport.Zero();
                days = (from day in value where day.Day <= DateTime.Today && day.Day >= hired 
                        orderby day.Day descending select day).ToList();
                foreach (DayReport day in days)
                {
                    if (day.Day == DateTime.Today)
                        Today = day;
                }
            }
        }

        [JsonIgnore]
        public DayReport Today { get; set; }

        public bool ShouldReadNextMonth()
        {
            if (Company != null)
            {
                return Company.StartMonthBefore && DateTime.Today.Day >= Company.StartDay;
            }
            return false;
        }

        public void AddDays(IList<DayReport> days)
        {
            var allDays = Days.Concat(days);
            Days = allDays.GroupBy(day => day.Day).Select(day => day.First()).ToList();
        }
    }
}
