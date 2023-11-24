using System;

namespace Api.Config.Util
{
    public class TimeHelper
    {
        /// <summary>
        /// 获取当前时间的毫秒数
        /// </summary>
        /// <returns></returns>
        public static long GetTimeInSecond(DateTime time)
        {
            DateTime startTime = DateTime.Parse("1970-1-1");
            long seconds = (time.Ticks - startTime.Ticks) / 10000000;
            return seconds;
        }
        /// <summary>
        /// 获取相对时间的秒数(相对)
        /// </summary>
        /// <returns></returns>
        public static long GetTimeInUtcSecond()
        {
            DateTime startTime = DateTime.Parse("1970-1-1");
            long seconds = (DateTime.Now.Ticks - startTime.Ticks) / 10000000;
            return seconds;
        }
        /// <summary>
        /// 根据Utc分钟数获取时间
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        public static DateTime UtcSecondToTime(long millis)
        {
            DateTime startTime = DateTime.Parse("1970-1-1");
            millis = startTime.Ticks + millis * 10000000;
            try
            {
                return DateTime.FromBinary(millis);
            }
            catch
            {
                return DateTime.Now;
            }
        }
        /// <summary>
        /// 根据Utc分钟数获取时间
        /// </summary>
        /// <param name="millisStr"></param>
        /// <returns></returns>
        public static DateTime UtcSecondToTime(string millisStr)
        {
            if (millisStr.Length == 0)
            {
                return DateTime.MinValue;
            }
            long millis = 0;
            if (!long.TryParse(millisStr, out millis))
            {
                DateTime startTime = DateTime.Parse("1970-1-1");
                millis = DateTime.Now.Ticks - startTime.Ticks;
            }
            return UtcSecondToTime(millis);
        }
        /// <summary>
        /// 根据Utc分钟数获取时间
        /// </summary>
        /// <param name="millisStr"></param>
        /// <returns></returns>
        public static string StrUtcSecondToTime(string millisStr)
        {
            if (millisStr.Length == 0)
            {
                return "";
            }
            return UtcSecondToTime(millisStr).ToString("yyyy-MM-dd HH:mm");
        }
        /// <summary>
        /// 获取当前月第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFirstDayOfCurrentMonth()
        {
            DateTime dt = DateTime.Now;
            DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);
            return dt1;
        }
        /// <summary>
        /// 获取某月第一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(DateTime time)
        {
            DateTime dt = new DateTime(time.Year, time.Month, 1);
            return dt;
        }
        /// <summary>
        /// 获取当前月最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastDayOfCurrentMonth()
        {
            DateTime dt1 = GetFirstDayOfCurrentMonth();
            DateTime dt2 = dt1.AddMonths(1).AddDays(-1);
            return dt2;
        }
        /// <summary>
        /// 获取当前周第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFirstDayOfCurrentWeek()
        {
            DateTime time = DateTime.Now;
            int day = int.Parse(GetShortWeek(time));
            return time.AddDays(1 - day); ;
        }
        /// <summary>
        /// 获取当前周第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFirstDayOfCurrentWeek(DateTime time)
        {
            int day = int.Parse(GetShortWeek(time));
            return time.AddDays(1 - day); ;
        }
        /// <summary>
        /// 获取某月最后一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(DateTime time)
        {
            DateTime dt1 = GetFirstDayOfMonth(time);
            DateTime dt2 = dt1.AddMonths(1).AddDays(-1);
            return dt2;
        }
        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GetAge(DateTime birthday)
        {
            if (birthday == null)
            {
                return "未知";
            }

            //（4*时间天数差）/(365*4+1)
            TimeSpan ts1 = new TimeSpan(birthday.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            // 格式化年龄输出
            if (ts.TotalDays >= 365)// 年份输出
            {
                return Math.Floor(ts.TotalDays / 365.00).ToString() + "岁";
            }
            else if (ts.TotalDays > 30)//输出月数
            {
                return Math.Floor(ts.TotalDays / 30.00).ToString() + "个月";
            }
            else if (ts.TotalHours > 24)//输出天数
            {
                return Math.Floor(ts.TotalDays) + "天";
            }
            else//小时
            {
                return Math.Ceiling(ts.TotalHours) + "小时";
            }
        }
        /// <summary>
        /// 获取时分中文
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static string GetUperWord(int hour, int minute)
        {
            string word = string.Empty;
            word = ConvertToUpper(hour) + "时" + ConvertToUpper(minute) + "分";
            return word;
        }
        /// <summary>
        /// 获取时分秒中文
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static string GetUperWord(int hour, int minute, int second)
        {
            string word = string.Empty;
            if (hour > 0)
            {
                word += ConvertToUpper(hour);
            }
            word += ConvertToUpper(minute) + "分" + ConvertToUpper(minute) + "秒";
            return word;
        }
        /// <summary>
        /// 获取时分秒中文
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static string GetWord(int hour, int minute, int second)
        {
            string word = string.Empty;
            if (hour != 0)
            {
                word += hour + "时" + Math.Abs(minute) + "分" + Math.Abs(second) + "秒";
            }
            else
            {
                if (minute != 0)
                {
                    word += minute + "分" + Math.Abs(second) + "秒";
                }
                else
                {
                    word += second + "秒";
                }
            }
            return word;
        }
        /// <summary>
        /// 转换为大写
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static string ConvertToUpper(int num)
        {
            if (num.ToString().Length == 1)
            {
                if (num == 0)
                    return "零";
                if (num == 1)
                    return "一";
                if (num == 2)
                    return "二";
                if (num == 3)
                    return "三";
                if (num == 4)
                    return "四";
                if (num == 5)
                    return "五";
                if (num == 6)
                    return "六";
                if (num == 7)
                    return "七";
                if (num == 8)
                    return "八";
                if (num == 9)
                    return "九";
            }
            if (num.ToString().Length == 2)
            {
                if (num < 20)
                {
                    if (num == 10)
                    {
                        return "十";
                    }
                    else
                    {
                        return "十" + ConvertToUpper(num % 10);
                    }
                }
                else
                {
                    return ConvertToUpper(num / 10) + "十" + ConvertToUpper(num % 10);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取星期
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetWeek(DateTime time)
        {
            string week = string.Empty;
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week = "星期一";
                    break;
                case DayOfWeek.Tuesday:
                    week = "星期二";
                    break;
                case DayOfWeek.Wednesday:
                    week = "星期三";
                    break;
                case DayOfWeek.Thursday:
                    week = "星期四";
                    break;
                case DayOfWeek.Friday:
                    week = "星期五";
                    break;
                case DayOfWeek.Saturday:
                    week = "星期六";
                    break;
                case DayOfWeek.Sunday:
                    week = "星期日";
                    break;
            }
            return week;
        }
        /// <summary>
        /// 获取星期
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetShortWeek(DateTime time)
        {
            string week = string.Empty;
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week = "1";
                    break;
                case DayOfWeek.Tuesday:
                    week = "2";
                    break;
                case DayOfWeek.Wednesday:
                    week = "3";
                    break;
                case DayOfWeek.Thursday:
                    week = "4";
                    break;
                case DayOfWeek.Friday:
                    week = "5";
                    break;
                case DayOfWeek.Saturday:
                    week = "6";
                    break;
                case DayOfWeek.Sunday:
                    week = "7";
                    break;
            }
            return week;
        }
        /// <summary>
        /// 获取星期
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetWeekDay(DateTime time)
        {
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                case DayOfWeek.Sunday:
                    return 7;
            }
            return 0;
        }
        /// <summary>
        /// 获取星期
        /// </summary>
        /// <returns></returns>
        public static int GetWeekDay()
        {
            return GetWeekDay(DateTime.Now);
        }
        public static int GetYear(DateTime bTime, DateTime stime)
        {
            return (bTime - stime).Days / 365;
        }
        /// <summary>
        /// 获取分钟差
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetSpanMinutes(DateTime startTime, DateTime endTime)
        {
            int minute = GetSpanSeconds(startTime, endTime);
            return minute / 60;
        }
        /// <summary>
        /// 获取秒钟差
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetSpanSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan span;
            if (endTime > startTime)
            {
                span = endTime - startTime;
            }
            else
            {
                span = startTime - endTime;
            }
            int second = 0;
            second = span.Days * 24 * 60 * 60;
            second += span.Hours * 60 * 60;
            second += span.Minutes * 60;
            second += span.Seconds;
            return second;
        }
    }
}
