using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Models;
using System.Web.Http.Cors;
using System.Xml;
using System.Xml.Linq;
using System.Data;

namespace Service.Controllers
{
    [EnableCors("http://localhost:14341", "*", "*")]
    public class LogsController : ApiController
    {
        private string log4j = "http://jakarta.apache.org/log4j";
        private List<Log> logs = new List<Log>();

        [HttpGet]
        [Route("api/Logs")]
        public IEnumerable<Log> GetAllLogs()
        {
            logs = ReadFileXml();
            return logs;
        }

        [HttpGet]
        [Route("api/RefreshedLogs/{maxTimestamp}")]
        public IEnumerable<Log> GetRefreshedLogs(long maxTimestamp)
        {
            //logs.Add(new Log { Level = "new item", Timestamp= DateTimeOffset.Now.Millisecond, Logger = "HeartbeatsManager.Services.HeartbeatsListener",Thread ="5" });
            var logsAdded = ReadAddedLogs(maxTimestamp);
            return logsAdded;
        }

        [HttpGet]
        [Route("api/SortedLogs")]
        public IEnumerable<Log> GetSortedLogs()
        {
            var sortedLogs = ReadFileXml();
            sortedLogs.Sort();
            return sortedLogs;
        }

        [HttpPost]
        [Route("api/FilterLogs")]
        public IEnumerable<Log> FilterLogs(FilterData filterData)
        {

            logs = ReadFileXml();
            IEnumerable<Log> filtredLogs = logs;
            if (filterData.Column == Constants.ColumnName.LEVEL_COLUMN_NAME)
            {
                filtredLogs = logs.Where(x => x.Level.StartsWith(filterData.Text.ToUpper()));
                filtredLogs.Concat(logs.Where(x => x.Level.StartsWith(filterData.Text.ToLower())));
            }
            else if(filterData.Column == Constants.ColumnName.LOGGER_COLUMN_NAME)
            {
                filtredLogs = logs.Where(x => x.Logger.StartsWith(filterData.Text));
            }else if(filterData.Column == Constants.ColumnName.THREAD_COLUMN_NAME && filterData.ThreadRange == "")
            {
                filtredLogs = logs.Where(x => x.Thread.SequenceEqual(filterData.Text));
            }
            if (filterData.ThreadRange != "empty")
            {
                    int number;
                    List<Log> fLogs = new List<Log>();
                    foreach (Log l in filtredLogs)
                    {
                        bool orNumber = int.TryParse(l.Thread, out number);
                        if (orNumber)
                        {
                            int numberThread = Int32.Parse(l.Thread);
                            if(filterData.ThreadRange == "<=10" && numberThread <= 10)
                            {
                                fLogs.Add(l);
                            }else if(filterData.ThreadRange == "10-40" && (numberThread >10 && numberThread <= 40))
                            {
                                fLogs.Add(l);
                            }
                            else if (filterData.ThreadRange == "40-80" && (numberThread > 40 && numberThread <= 80))
                            {
                                fLogs.Add(l);
                            }
                            else if (filterData.ThreadRange == "80-110" && (numberThread > 80 && numberThread <= 110))
                            {
                                fLogs.Add(l);
                            }
                            else if (filterData.ThreadRange == ">110" && numberThread > 110)
                            {
                                fLogs.Add(l);
                            }
                        }
                    }
                filtredLogs = fLogs;
            }
            logs = filtredLogs.ToList();
            return filtredLogs;
        }

        [HttpGet]
        [Route("api/FilteredLogs")]
        public IEnumerable<Log> GetFilteredLogs()
        {
            return logs;
        }

        public List<Log> ReadAddedLogs(long maxTimestamp)
        {
            List<Log> addedLogs = new List<Log>();
            NameTable nt = new NameTable();
            XmlNamespaceManager mgr = new XmlNamespaceManager(nt);
            mgr.AddNamespace("log4j", log4j);

            long currentMaxTimestamp = maxTimestamp;

            XmlParserContext pc = new XmlParserContext(nt, mgr, "", XmlSpace.Default);

            XNamespace log4jNs = log4j;

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (XmlReader readerXml = XmlReader.Create(@"C:\\Users\\Win10\\Desktop\\HeartbeatsManager.xml", settings, pc))
            {
                while (readerXml.Read())
                {
                    if (readerXml.LocalName == "event")
                    {
                        using (XmlReader eventReader = readerXml.ReadSubtree())
                        {
                            eventReader.Read();

                            XElement eventEl = XNode.ReadFrom(eventReader) as XElement;
                            if ((long)eventEl.Attribute("timestamp") > maxTimestamp)
                            {
                                addedLogs.Add(new Log
                                {
                                    Logger = (string)eventEl.Attribute("logger"),
                                    Timestamp = (long)eventEl.Attribute("timestamp"),
                                    Level = (string)eventEl.Attribute("level"),
                                    Thread = (string)eventEl.Attribute("thread"),
                                    Message = (string)eventEl.Element(log4jNs + "message")
                                }
                                  );
                                if ((long)eventEl.Attribute("timestamp") > currentMaxTimestamp)
                                {
                                    currentMaxTimestamp = (long)eventEl.Attribute("timestamp");
                                }
                            }
                            eventReader.Close();
                        }
                    }
                }
            }

            return addedLogs;
        }

        public List<Log> ReadFileXml()
        {
            logs = new List<Log>();

            NameTable nt = new NameTable();
            XmlNamespaceManager mgr = new XmlNamespaceManager(nt);
            mgr.AddNamespace("log4j", log4j);

            XmlParserContext pc = new XmlParserContext(nt, mgr, "", XmlSpace.Default);

            XNamespace log4jNs = log4j;

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (XmlReader readerXml = XmlReader.Create(@"C:\\Users\\Win10\\Desktop\\HeartbeatsManager.xml", settings, pc))
            {
                while (readerXml.Read())
                {
                    if (readerXml.LocalName == "event")
                    {
                        using (XmlReader eventReader = readerXml.ReadSubtree())
                        {
                            eventReader.Read();
                            XElement eventEl = XNode.ReadFrom(eventReader) as XElement;
                            logs.Add(new Log {
                                Logger =(string)eventEl.Attribute("logger"),
                                Timestamp =(long)eventEl.Attribute("timestamp"),
                                Level = (string)eventEl.Attribute("level"),
                                Thread = (string)eventEl.Attribute("thread"),
                                Message = (string)eventEl.Element(log4jNs + "message")
                              }
                              );
                            eventReader.Close();
                        }
                    }
                }
            }
            return logs;
        }

    }
}
