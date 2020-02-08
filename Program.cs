using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Library_System
{
    class Program
    {
        private static long _total = 0L;
        private static string _lastNodeAction = string.Empty;

        static void Main(string[] args)
        {
            string inputFilePath;
            string outputFileType;

            Console.WriteLine("\n============== Starting Program ===============\n");

            /*------------------ reading user input ------------------*/
            Console.Write(Resource.enterfilepath);
            inputFilePath = Console.ReadLine();
            Console.Write(Resource.chooseoutputfiletype);
            outputFileType = Console.ReadLine();

            /*------------------ reading input file ------------------*/
            List<Record> query = ReadInputFile(inputFilePath);

            /*------------------ handle statistical operations ------------------*/
            var resultJson = HandleStatisticalData(query);


            /*------------------ write output results to file ------------------*/
            HandelOutoutFile(resultJson, outputFileType);

            /*------------------ write output results to console ------------------*/
            Console.WriteLine("\n=============================\n");
            Console.WriteLine(resultJson);
            Console.WriteLine("\n=============================\n");

            Console.WriteLine("\n============== Ending Program ===============\n");
        }

        public static List<Record> ReadInputFile(string inputFilePath)
        {
            List<Record> query = new List<Record>();
            if (!string.IsNullOrEmpty(inputFilePath))
            {
                if (inputFilePath.EndsWith(".xml"))
                {
                    XElement root = XElement.Load(inputFilePath);

                    query =
                        (from el in root.Elements(nameof(Record).ToLower())
                         select new Record(el.Element(nameof(Record.Person).ToLower()).Attribute("id").Value,
                                    el.Element(nameof(Record.ISBN).ToLower()).Value,
                                    (DateTimeOffset)el.Element(nameof(Record.Timestamp).ToLower()),
                                    el.Element(nameof(Record.Action).ToLower()).Attribute("type").Value)).ToList();
                }
                else if (inputFilePath.EndsWith(".csv"))
                {
                    query = File.ReadLines(inputFilePath)
                                .Skip(1)
                                .Where(s => s != "")
                                .Select(s => s.Split(new[] { ',' }))
                                .Select(a => new Record(a[1], a[2], DateTimeOffset.Parse(a[0]), a[3]))
                                .ToList();
                }
                else
                {
                    // Console app
                    Console.WriteLine(Resource.wrongentry);
                    Console.WriteLine("\n============== Ending Program ===============\n");
                    System.Environment.Exit(1);
                }
            }
            else
            {
                // Console app
                Console.WriteLine(Resource.wrongentry);
                Console.WriteLine("\n============== Ending Program ===============\n");
                System.Environment.Exit(1);
            }

            return query;
        }

        public static object HandleStatisticalData(List<Record> query)
        {
            /*Person has the most checkouts*/
            var personWithMostCheckouts =
                (from el in query
                 where el.Action == "check-out"
                 group el by el.Person into g1
                 orderby g1.Count() descending
                 select g1.Key).First();

            /*Which book was checked out the longest time in total*/
            var groupByBook =
                from el in query
                group el by el.ISBN into g2
                select new
                {
                    key = g2.Key,
                    book_checkout_time = g2.Aggregate(0L, func: SumUpCheckoutTime, GetCheckoutTotalTime)
                };

            var mostCheckedOutBook =
                (from elm in groupByBook
                 orderby elm.book_checkout_time descending
                 select elm.key).First();

            /*How many books are checked out at this moment */
            var currentCheckedOutBookCount =
                (from el in query
                 select el).Aggregate(0,
                    (current, next) =>
                        next.Action == "check-in" ? current - 1 : current + 1,
                    res => res);

            /*Who currently has the largest number of books */
            var personHasCurrentlyMostBooks =
                ((from el in query
                  group el by el.Person into g2
                  select new
                  {
                      key = g2.Key,
                      book_count = g2.Aggregate(0,
                      (longest, next) =>
                          next.Action == "check-in" ? longest - 1 : longest + 1,
                      res => res)
                  }).OrderByDescending(order => order.book_count)).First().key;

            /*Write to result obj*/
            var resultJson = new
            {
                person_with_most_checkouts = personWithMostCheckouts,
                most_checked_out_book = mostCheckedOutBook,
                current_checked_out_book_count = currentCheckedOutBookCount,
                person_who_has_currently_most_books = personHasCurrentlyMostBooks
            };

            return resultJson;
        }

        public static void HandelOutoutFile(Object resultJson, string outputFileType)
        {
            var outputFileName = "output_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");

            if (outputFileType == "J")
            {
                outputFileName += ".json";
            }
            else
            {
                outputFileName += ".txt";
            }

            File.WriteAllText(outputFileName, resultJson.ToString());
        }

        public static long SumUpCheckoutTime(long current, Record next)
        {
            long result = next.Ticks;
            _lastNodeAction = next.Action;

            if (next.Action == "check-in")
            {
                result = next.Ticks - current;
                _total += result;
            }

            return result;
        }

        private static long GetCheckoutTotalTime(long result)
        {
            long temp = _total;

            if (_lastNodeAction == "check-out")
            {
                temp = DateTime.Now.Ticks - result;
                temp += _total;
            }

            _total = 0L;
            return temp;
        }
    }
}