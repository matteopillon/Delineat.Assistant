using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delineat.Assistan.Exports
{
    public class MothlyHoursExport
    {
        private readonly DAAssistantDBContext dbContext;
        private readonly ILogger logger;

        public MothlyHoursExport(DAAssistantDBContext dbContext, ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public bool ExportToExcel(string filePath, int month, int year)
        {
            var startOfTheMonth = new DateTime(year, month, 1);
            var nextMonth = month + 1;
            var nextYear = year;
            if (nextMonth == 13)
            {
                nextMonth = 1;
                nextYear++;
            }
            var endOfTheMonth = new DateTime(nextYear, nextMonth, 1, 0, 0, 0).AddSeconds(-1);

            var workLogs = dbContext.DayWorkLogs.Include(d => d.User).ThenInclude(u => u.WeekWork).Include(d => d.Job)
                           .Where(d => d.Date >= startOfTheMonth && d.Date <= endOfTheMonth);
            logger.LogInformation($"Export found {workLogs?.Count()}");
            return ExportToExcel(filePath, workLogs.ToList().GroupBy(d => d.User).ToList(), startOfTheMonth, endOfTheMonth);

        }


        private List<HolidayDate> holidayDateCache = null;
        private HolidayDate GetHolidayDate(DateTime date)
        {
            if (holidayDateCache == null) holidayDateCache = dbContext.HolidayDates.ToList();
            var holidayDate = holidayDateCache.FirstOrDefault(d => (d.Day == 0 || d.Day == date.Day)
                                                       && (d.Month == 0 || d.Month == date.Month)
                                                       && (d.Year == 0 || d.Year == date.Year)
                                                       && string.IsNullOrEmpty(d.FormulaId));

            if (holidayDate == null)
            {
                holidayDate = ResolveFormula(date, holidayDateCache.Where(d => !string.IsNullOrEmpty(d.FormulaId)));
            }

            return holidayDate;
        }

        private HolidayDate ResolveFormula(DateTime date, IEnumerable<HolidayDate> formulas)
        {
            foreach (var holidayDate in formulas)
            {
                var formulaResult = DateTime.MinValue;
                switch (holidayDate.FormulaId.ToLower())
                {
                    case "easter":
                        formulaResult = GetEaster(date);
                        break;
                    case "dayaftereaster":
                        formulaResult = GetEaster(date).AddDays(1);
                        break;
                }
                if (formulaResult == date)
                {
                    return holidayDate;
                }
            }

            return null;
        }
        private Dictionary<int, DateTime> cacheEaster = new Dictionary<int, DateTime>();
        private DateTime GetEaster(DateTime date)
        {
            if (cacheEaster.ContainsKey(date.Year)) return cacheEaster[date.Year];
            int day = 0;
            int month = 0;
            int year = date.Year;
            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            var easterDate = new DateTime(year, month, day);
            cacheEaster.Add(year, easterDate);
            return easterDate;
        }

        private const string STYLE_NAME_USER = "userName";
        private const string STYLE_NAME_MONTH = "month";
        private const string STYLE_NAME_HEADER = "header";
        private const string STYLE_NAME_NORMAL = "Normal";

        private const string STYLE_NAME_DAY = "Day";
        private const string STYLE_NAME_EMPTY_HOUR = "Hour";
        private const string STYLE_NAME_NOT_WORKING_DAY = "NotWorking";
        private const string STYLE_NAME_HOLIDAY = "Holiday";


        private bool ExportToExcel(string filePath, List<IGrouping<User, DayWorkLog>> userLogs, DateTime startOfTheMonth, DateTime endOfTheMonth)
        {
            bool completed = false;
            Microsoft.Office.Interop.Excel.Application excelApplication = null;
            Workbook workBook = null;
            try
            {

                logger?.LogInformation("Start Excel");
                excelApplication = new Microsoft.Office.Interop.Excel.Application();
                logger?.LogInformation("Start Excel completed");
                workBook = excelApplication.Workbooks.Add();


                // User Style
                var style = workBook.Styles.Add(STYLE_NAME_USER);
                style.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Orange);
                style.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                // Month Style
                style = workBook.Styles.Add(STYLE_NAME_MONTH);
                style.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);
                style.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGreen);

                style = workBook.Styles.Add(STYLE_NAME_HEADER);
                style.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(91, 155, 213));
                style.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                style = workBook.Styles.Add(STYLE_NAME_DAY);
                style.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);

                style = workBook.Styles.Add(STYLE_NAME_NOT_WORKING_DAY);
                style.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 192, 0));

                style = workBook.Styles.Add(STYLE_NAME_HOLIDAY);
                style.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkOrange);

                style = workBook.Styles.Add(STYLE_NAME_EMPTY_HOUR);
                style.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);

                bool useFirst = true;
                foreach (var g in userLogs)
                {
                    if (g.Key.WeekWork != null)
                    {
                        Worksheet worksheet = null;
                        worksheet = useFirst ? workBook.Worksheets[1] : workBook.Worksheets.Add();
                        useFirst = false;
                        worksheet.Name = $"{g.Key.FirstName} {g.Key.LastName}".Trim();
                        ExportToSheet(worksheet, g.Key, g.ToList(), startOfTheMonth, endOfTheMonth);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    }
                }

                workBook.SaveAs2(filePath);
                workBook.Close();
                completed = true;
            }
            catch (Exception ex)
            {
                logger?.LogError("Errore in fase di generazione dell'excel", ex);
                logger?.LogInformation(ex.Message);
            }
            finally
            {
                try
                {
                    if (workBook != null)
                    {

                        workBook.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
                    }
                }
                catch
                {
                }
                try
                {

                    if (excelApplication != null)
                    {
                        excelApplication.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                    }
                }
                catch
                {
                }
            }

            return completed;
        }



        private double MinutesToHour(int value)
        {
            var minutePart = value % 60.0;
            var hours = (value - minutePart) / 60.0;

            return hours + minutePart / 60.0;
        }

        private void ExportToSheet(Worksheet worksheet, User user, List<DayWorkLog> dayWorkLogs, DateTime startOfTheMonth, DateTime endOfTheMonth)
        {
            var ordinaryDate = new Dictionary<DayOfWeek, double> { { DayOfWeek.Monday, user.WeekWork.OnMonday },
                                                                { DayOfWeek.Tuesday, user.WeekWork.OnTuesday },
                                                                { DayOfWeek.Wednesday, user.WeekWork.OnWednesday },
                                                                { DayOfWeek.Thursday, user.WeekWork.OnThursday },
                                                                { DayOfWeek.Friday, user.WeekWork.OnFriday },
                                                                { DayOfWeek.Saturday, user.WeekWork.OnSaturday },
                                                                { DayOfWeek.Sunday, user.WeekWork.OnSunday }};

            //Larghezza colonne
            worksheet.Columns[1].ColumnWidth = 36.43;
            worksheet.Columns[2].ColumnWidth = 15.86;
            worksheet.Columns[3].ColumnWidth = 15.29;
            worksheet.Columns[4].ColumnWidth = 11.86;
            worksheet.Columns[5].ColumnWidth = 8.43;
            worksheet.Columns[5].ColumnWidth = 8.43;
            worksheet.Columns[7].ColumnWidth = 11.29;
            // Nome e cognome
            Microsoft.Office.Interop.Excel.Range range = worksheet.Cells[1, 1];
            range.Value2 = $"{user.LastName ?? ""} {user.FirstName ?? ""}".Trim();
            range.Style = STYLE_NAME_USER;
            //mese
            range = worksheet.Cells[1, 3];
            range.Value2 = "mese di";
            range = worksheet.Cells[1, 4];

            range.Style = STYLE_NAME_MONTH;
            range.NumberFormatLocal = "mmm-aa";
            range.Value2 = startOfTheMonth;


            range = worksheet.Cells[2, 1];
            range.Value2 = "ore ordinarie";

            range = worksheet.Cells[4, 1];
            range.Value2 = "data";
            range.Style = STYLE_NAME_HEADER;
            range = worksheet.Cells[4, 2];
            range.Value2 = "ore ordinarie";
            range.Style = STYLE_NAME_HEADER;
            range = worksheet.Cells[4, 3];
            range.Value2 = "ore straordinarie";
            range.Style = STYLE_NAME_HEADER;
            range = worksheet.Cells[4, 4];
            range.Value2 = "ore permessi";
            range.Style = STYLE_NAME_HEADER;
            range = worksheet.Cells[4, 5];
            range.Value2 = "ore ferie";
            range.Style = STYLE_NAME_HEADER;
            range = worksheet.Cells[4, 6];
            range.Value2 = "altro ore";
            range.Style = STYLE_NAME_HEADER;
            range = worksheet.Cells[4, 7];
            range.Value2 = "(specificare)";
            range.Style = STYLE_NAME_HEADER;

            var date = startOfTheMonth;
            var i = 0;
            while (date <= endOfTheMonth)
            {

                range = worksheet.Cells[5 + i, 1];
                var holidayDate = GetHolidayDate(date);
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        range.Style = STYLE_NAME_NOT_WORKING_DAY;
                        break;
                    default:
                        range.Style = holidayDate == null ? STYLE_NAME_DAY : STYLE_NAME_HOLIDAY;
                        break;
                }



                range.NumberFormatLocal = "[$-x-sysdate]gggg, mmmm gg, aaaa";
                range.Value2 = date.ToOADate();


                var maxOrdinaryHours = ordinaryDate[date.DayOfWeek];
                if (holidayDate != null) maxOrdinaryHours = MinutesToHour(holidayDate.Minutes);


                var w = dayWorkLogs.Where(d => d.Date == date && (!d.Job?.IsAbsence ?? false)).Sum(d => d.Minutes);
                // Ore ordinarie
                range = worksheet.Cells[5 + i, 2];
                if (w == 0) range.Style = STYLE_NAME_EMPTY_HOUR;
                var workingHours = MinutesToHour(w);
                range.Value2 = Math.Min(workingHours, maxOrdinaryHours);

                // Ore straordinarie
                var ex = Math.Max(workingHours - maxOrdinaryHours, 0);
                range = worksheet.Cells[5 + i, 3];
                if (ex == 0) range.Style = STYLE_NAME_EMPTY_HOUR;
                range.Value2 = ex;

                // Permessi
                var p = dayWorkLogs.Where(d => d.Date == date && (d.Job?.IsAbsence ?? false)).Sum(d => d.Minutes);
                range = worksheet.Cells[5 + i, 4];
                if (p == 0) range.Style = STYLE_NAME_EMPTY_HOUR;
                range.Value2 = MinutesToHour(p);


                // Ferie = Ore mancati senza considerare i permessi
                var vacancy = Math.Max(maxOrdinaryHours - p - workingHours, 0);
                range = worksheet.Cells[5 + i, 5];
                if (vacancy == 0) range.Style = STYLE_NAME_EMPTY_HOUR;

                range.Value2 = vacancy;

                i++;
                date = date.AddDays(1);
            }


            range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[4 + i, 7]];
            range.Borders.ColorIndex = 1;
            range.Borders.LineStyle = XlLineStyle.xlContinuous;


        }
    }
}
