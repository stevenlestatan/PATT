using System;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

namespace PATT_Simulator {
    public class EPos {
        private string file;
        private Task<string[]> read;

        public List<char> tableList;
        public List<string> waiterId;

        private XDocument doc;

        public EPos(string file) {
            if (string.IsNullOrEmpty(file)) {
                throw new Exception("File path not found.");
            } else {
                this.file = file;
                read = File.ReadAllLinesAsync(this.file);
                read.Wait();
            }
        }

        public void GetAllTableList() {
            if (read.Status == TaskStatus.RanToCompletion) {
                tableList = new List<char>();
                foreach (var i in read.Result) {
                    if (i.Contains("00")) {
                        var seperate = i.Split('|');
                        foreach (char c in seperate[1]) {
                            tableList.Add(c);
                        }
                        break;
                    }
                }

                XMLTableListResponse(tableList);

                Utilities.Log("--");
                Utilities.Log("All Tables: ");
                tableList.ForEach(s => Utilities.Log(s.ToString()));
            } else {
                throw new Exception("Can't generate table list.");
            }
        }

        public void GetTablesPerWaiterId(string waiterId) {
            if (read.Status == TaskStatus.RanToCompletion) {
                tableList = new List<char>();
                foreach (var i in read.Result) {
                    if (i.Contains(waiterId)) {
                        var seperate = i.Split('|');
                        foreach (char c in seperate[1]) {
                            tableList.Add(c);
                        }
                    }
                }

                XMLTableListResponse(tableList);

                Utilities.Log(string.Empty);
                Utilities.Log("--");
                Utilities.Log("Waiter's available table(s): ");
                tableList.ForEach(s => Utilities.Log(s.ToString()));
            } else {
                throw new Exception("Can't generate table list.");
            }
        }

        public int GetWaiterIdByTableId(string tableId) {
            int result = 0;
            if (read.Status == TaskStatus.RanToCompletion) {
                foreach (var i in read.Result) {
                    var seperate = i.Split('|');
                    if (seperate[1].Contains(tableId.Remove(0, 3))) {
                        result = int.Parse(seperate[0]);
                    }
                }
            } else {
                throw new Exception("Can't generate waiter list.");
            }

            return result;
        }

        public void GetAllWaiterId() {
            if (read.Status == TaskStatus.RanToCompletion) {
                waiterId = new List<string>();
                foreach (var i in read.Result) {
                    if (!i.Contains("00")) {
                        var seperate = i.Split('|');
                        waiterId.Add(seperate[0]);
                    }
                }

                Utilities.Log("--");
                Utilities.Log("Waiter IDs: ");
                waiterId.ForEach(s => Utilities.Log(s.ToString()));
            } else {
                throw new Exception("Can't generate table list.");
            }
        }

        public void GrantResponse(string paymentMode = "0") {
            StringBuilder sb = new StringBuilder();
            sb.Append("00")
            .Append("0")
            .Append("00000618")
            .Append(paymentMode)
            .Append("826")
            .Append("0")
            .Append("CONF_OK"); // OR CONF_NOK

            Utilities.Log("--");
            Utilities.Log("Response Message packet: ");
            Utilities.Log(sb.ToString());
        }

        public void XMLReceiptResponse(string tableId) {
            string companyLogo = "Qk2OAAAAAAAAAD4AAAAoAAAAFAAAABQAAAABAAEAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAA AAAP///wAAAAAAf8AAAH/AAAB/wAAAcccAAHHHAABxxwAAf8AAAH/AAAB/wAAAAD/gAAA/4AAAP+AADjjgAA444A AOOOAAAD/gAAA/4AAAP+AAAAAAAA==</";
            string companyName = "GlobalPayments";
            string companyLocation = "Makati City";
            string dateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string table = string.Format("Table: {0}", tableId);
            string check = string.Format("Check: {0}", new Random().Next(0, 99999));
            string poundASCII = string.Format(string.Empty.PadRight(19, ' '), (char)0xA3);

            doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", "ISO-8859-1", null);

            XElement xRootElement = new XElement("EPOS_RECEIPT");
            XElement xChildElement = new XElement("DT");

            // CONTENT OF DT
            xChildElement.Add(new XElement("LO", new XAttribute("LEN", companyLogo.Length), companyLogo));
            xChildElement.Add(new XElement("OB", "CR"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", companyName.Length), companyName));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", companyLocation.Length), companyLocation));
            xChildElement.Add(new XElement("OB", "LS"));
            xChildElement.Add(new XElement("OB", "CR"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", table.Length), table));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", check.Length), check));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", dateTime.Length), dateTime));
            xChildElement.Add(new XElement("OB", "CR"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", poundASCII.Length), string.Format(string.Empty.PadRight(19, ' '), '£')));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Starter             6.95"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Starter             4.95"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Main Course        17.95"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Main Course        24.95"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Side                4.95"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 5), "Larger"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), " 3 @ 6.95          20.85"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 16), "White Wine (Btl)"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), " 2 @ 13.95         27.90"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Dessert             4.95"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Dessert             4.95"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "Whiskey             4.95"));
            xChildElement.Add(new XElement("OB", "DLS"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "SUB-TOTAL         123.45"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), new XAttribute("H", "YES"), "TOTAL DUE         123.45"));
            xChildElement.Add(new XElement("OB", "DLS"));
            xChildElement.Add(new XElement("OB", "CR"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 20), "THANK YOU FOR DINING"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 22), "WITH US AND WE HOPE TO"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 22), "WELCOME YOU AGAIN SOON"));
            xChildElement.Add(new XElement("OB", "CR"));
            xChildElement.Add(new XElement("BC", new XAttribute("TYPE", "CODE39"), "CHK10402"));
            xChildElement.Add(new XElement("OB", "CR"));
            xChildElement.Add(new XElement("LN", new XAttribute("LEN", 24), "01 1005   5 10402 010111"));

            xRootElement.Add(xChildElement);
            doc.Add(xRootElement);

            string temporaryPath = Utilities.CreateTemporaryPath(".xml");
            doc.Save(temporaryPath);

            string xmlContent = File.ReadAllText(temporaryPath);
            File.Delete(temporaryPath);

            Utilities.Log("--");
            Utilities.Log("XML Receipt Response: ");
            Utilities.Log(xmlContent);
        }

        public void XMLAdditionalMsgListResponse() {
            doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", "ISO-8859-9", null);
            XElement xRoot = new XElement("ADDITIONAL_DATA", "RESPONSE");
            xRoot.Add(new XElement("TYPE", "SPLITSALE_REPORT"));
            xRoot.Add(new XElement("TYPE", "FINAL_REPORT"));
            xRoot.Add(new XElement("TYPE", "TICKET"));
            xRoot.Add(new XElement("TYPE", "END"));
            doc.Add(xRoot);

            string temporaryPath = Utilities.CreateTemporaryPath(".xml");
            doc.Save(temporaryPath);

            string xmlContent = File.ReadAllText(temporaryPath);
            File.Delete(temporaryPath);

            Utilities.Log("--");
            Utilities.Log("XML Additional Message List Response: ");
            Utilities.Log(xmlContent);
        }

        private void XMLTableListResponse(List<char> tableList) {
            doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", "ISO-8859-1", string.Empty);

            XElement xElement = new XElement("EPOS_TABLE_LIST");
            foreach (char c in tableList) {
                // CONTENT OF EPOS_TABLE_LIST
                xElement.Add(new XElement("TABLE", c));
            }
            doc.Add(xElement);

            string temporaryPath = Utilities.CreateTemporaryPath(".xml");
            doc.Save(temporaryPath);

            string xmlContent = File.ReadAllText(temporaryPath);
            File.Delete(temporaryPath);

            Utilities.Log("--");
            Utilities.Log("XML Table List Response: ");
            Utilities.Log(xmlContent);
        }
    }
}
