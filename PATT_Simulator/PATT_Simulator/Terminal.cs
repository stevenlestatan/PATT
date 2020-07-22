using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PATT_Simulator {
    public class Terminal {
        private int? paymentMode;

        public Terminal(int? paymentMode) {
            this.paymentMode = paymentMode;
        }

        public bool GrantRequest(string privateData, bool isExtendedTableLock = false, EPos epos = null) {
            bool result = true;
            try {
                if (isExtendedTableLock) {
                    int waiterId = epos.GetWaiterIdByTableId(privateData);
                    privateData = string.Format("O0{0}{1}", waiterId.ToString().Length, waiterId) + privateData;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("00")
                .Append("00000000")
                .Append("0")
                .Append(paymentMode)
                .Append("0")
                .Append("000")
                .Append(privateData);

                Utilities.Log(string.Empty);
                Utilities.Log("--");
                Utilities.Log("Request Message packet: ");
                Utilities.Log(sb.ToString());
            } catch (Exception e) {
                result = false;
            }

            return result;
        }

        public bool GrantTransactionRequest() {
            bool result = true;
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append("00")
                .Append("0")
                .Append("00000618")
                .Append("0")
                .Append("C06007104M0B00000011100P0130000000000000000000000000000")
                .Append("826")
                .Append("0000000000");

                Utilities.Log(string.Empty);
                Utilities.Log("--");
                Utilities.Log("Request Message packet: ");
                Utilities.Log(sb.ToString());
            } catch (Exception e) {
                result = false;
            }

            return result;
        }

        public bool GrantAdditionalMsgRequest() {
            bool result = true;
            try {
                XDocument doc = new XDocument();
                doc.Declaration = new XDeclaration("1.0", "ISO-8859-1", null);
                XElement xElem = new XElement("ADDITIONAL_DATA", "REQUEST");
                doc.Add(xElem);

                string temporaryPath = Utilities.CreateTemporaryPath(".xml");
                doc.Save(temporaryPath);

                string xml = File.ReadAllText(temporaryPath);
                File.Delete(temporaryPath);

                string serialMsg = string.Empty;
                string ipMsg = string.Empty;

                serialMsg = Utilities.BuildSerialRequest(xml);

                List<string> ip = new List<string>();
                char[] c = xml.Length.ToString().ToCharArray();
                ip.Add(c[0].ToString());
                ip.Add(c[1].ToString());
                ipMsg = Utilities.BuildIPRequest(ip.ToArray(), xml);

                Utilities.Log(string.Empty);
                Utilities.Log("--");
                Utilities.Log("SERIAL -- XML Additional Message List Request packet: ");
                Utilities.Log(serialMsg);

                Utilities.Log(string.Empty);
                Utilities.Log("--");
                Utilities.Log("IP -- XML Additional Message List Request packet: ");
                Utilities.Log(ipMsg);
            } catch (Exception e) {
                result = false;
            }

            return result;
        }

        public bool GrantTransferDataRequest() {
            bool result = true;
            try {
                XDocument doc = new XDocument();
                doc.Declaration = new XDeclaration("1.0", "ISO-8859-1", null);
                XElement xElem = new XElement("DATA_TRANSFER", "REQUEST");
                xElem.Add(new XElement("TYPE", "EOD_REPORT"));
                doc.Add(xElem);

                string temporaryPath = Utilities.CreateTemporaryPath(".xml");
                doc.Save(temporaryPath);

                string xml = File.ReadAllText(temporaryPath);
                File.Delete(temporaryPath);

                string serialMsg = string.Empty;
                string ipMsg = string.Empty;

                serialMsg = Utilities.BuildSerialRequest(xml);

                List<string> ip = new List<string>();
                char[] c = xml.Length.ToString().ToCharArray();
                ip.Add(c[0].ToString());
                ip.Add(c[1].ToString());
                ipMsg = Utilities.BuildIPRequest(ip.ToArray(), xml);

                Utilities.Log(string.Empty);
                Utilities.Log("--");
                Utilities.Log("SERIAL -- XML Transfer Data Request packet: ");
                Utilities.Log(serialMsg);

                Utilities.Log(string.Empty);
                Utilities.Log("--");
                Utilities.Log("IP -- XML Transfer Data Request packet: ");
                Utilities.Log(ipMsg);
            } catch (Exception e) {
                result = false;
            }

            return result;
        }

        public void SplitsaleReportRequest() {
            XDocument doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", "ISO-8859-1", null);

            XElement xRoot = new XElement("CREDIT_CARD_RECEIPT");
            xRoot.Add(new XElement("RECEIPT",
                new XAttribute("STYPE", "SPLITSALE REPORT"),
                new XElement("LF",
                new XAttribute("LEN", 7),
                new XAttribute("ID", 0),
                new XAttribute("ID_NAME", "unknowns"),
                "NO DATA")));
            doc.Add(xRoot);

            string temporaryPath = Utilities.CreateTemporaryPath(".xml");
            doc.Save(temporaryPath);

            string xml = File.ReadAllText(temporaryPath);
            File.Delete(temporaryPath);

            Utilities.Log(string.Empty);
            Utilities.Log("--");
            Utilities.Log("Split Sale Report Request packet: ");
            Utilities.Log(xml);
        }

        public void TicketReportRequest() {
            string xml = "<?xml version=\"1.0\" encoding =\"ISO-8859-1\"?><CREDIT_CARD_RECEIPT><RECEIPT STYPE=\"MERCHANT\" >< LF LEN =\"24\" H=\"YES\" B =\"YES\" ID =\"1\" ID_NAME =\"MERCHANT NAME\">Global Payments     </LF> <LF  LEN=\"24\"  ID =\"3\"  ID_NAME =\"MERCHANT ID\" >M:79807901</LF> <LF  LEN=\"24\"  ID=\"4\"  ID_NAME=\"TID\" >TID:22160390         </LF> <LF  LEN=\"24\"  ID=\"20\"  ID_NAME=\"SEQUENCE NUMBER\" >S30</LF> <LF  LEN=\"24\"  ID=\"24\"  ID_NAME=\"HANDSET\" >HANDSET:1</LF> <LF  LEN=\"24\"  ID=\"7\"  ID_NAME=\"ISSUER NAME\" >AMERICAN EXPRESS</LF> <LF  LEN=\"24\"  ID=\"6\"  ID_NAME=\"AID\" >AID: A000000025010403</LF> <LF  LEN=\"24\"  H=\"YES\"  B=\"YES\"  ID=\"5\"  ID_NAME=\"APPLICATION LABEL\" >American Express</LF> <LF  LEN=\"24\"  ID=\"8\"  ID_NAME=\"PAN\" >***********0001</LF> <LF  LEN=\"24\"  ID=\"9\"  ID_NAME=\"EXPIRY DATE\" >EXP 11/16</LF> <LF  LEN=\"24\"  ID=\"10\"  ID_NAME=\"PAN SEQUENCE NUMBER\" >PAN SEQ NO. 00</LF> <LF  LEN=\"24\"  ID=\"11\"  ID_NAME=\"START DATE\" >STT 11/12</LF> <LF  LEN=\"24\"  ID=\"13\"  ID_NAME=\"DATA SOURCE\" >ICC</LF> <LF  LEN=\"24\"  ID=\"14\"  ID_NAME=\"TRANSACTION TYPE\" >SALE COMPLETION</LF> <LF  LEN=\"24\"  ID=\"15\"  ID_NAME=\"AMOUNT\" >AMOUNT            ?30.00</LF> <LF  LEN=\"24\"  H=\"YES\"  B=\"YES\"  ID=\"39\"  ID_NAME=\"TOTAL AMOUNT\" >TOTAL             ?30.00</LF> <LF  LEN=\"24\"  ID=\"31\"  ID_NAME=\"SIGNATURE BOX\" >SIGN BELOW</LF> <LF  LEN=\"24\"  ID=\"31\"  ID_NAME=\"SIGNATURE BOX\" >........................</LF> <LF  LEN=\"24\"  ID=\"31\"  ID_NAME=\"SIGNATURE BOX\" >PLEASE DEBIT MY ACCOUNT</LF> <LF  LEN=\"24\"  ID=\"17\"  ID_NAME=\"DATE / TIME\" >07/07/20 08:39</LF> <LF  LEN=\"24\"  ID=\"18\"  ID_NAME=\"MESSAGE\" >AUTH CODE:080808</LF> <LF  LEN=\"24\"  ID=\"19\"  ID_NAME=\"TRANSACTION NUMBER\" >TXN 0243</LF> <LF  LEN=\"24\"  ID=\"94\"  ID_NAME=\"FINAL MESSAGE\" >     MERCHANT COPY      </LF> <LF  LEN=\"24\"  ID=\"94\"  ID_NAME=\"FINAL MESSAGE\" > PLEASE RETAIN RECEIPT</LF> </RECEIPT> <RECEIPT  STYPE=\"CUSTOMER\" ><LF  LEN=\"24\"  H=\"YES\"  B=\"YES\"  ID=\"1\"  ID_NAME=\"MERCHANT NAME\" >Global Payments         </LF> <LF  LEN=\"24\"  ID=\"3\"  ID_NAME=\"MERCHANT ID\" >M:***07901</LF> <LF  LEN=\"24\"  ID=\"4\"  ID_NAME=\"TID\" >TID:****0390         </LF> <LF  LEN=\"24\"  ID=\"20\"  ID_NAME=\"SEQUENCE NUMBER\" >S30</LF> <LF  LEN=\"24\"  ID=\"24\"  ID_NAME=\"HANDSET\" >HANDSET:1</LF> <LF  LEN=\"24\"  ID=\"7\"  ID_NAME=\"ISSUER NAME\" >AMERICAN EXPRESS</LF> <LF  LEN=\"24\"  ID=\"6\"  ID_NAME=\"AID\" >AID: A000000025010403</LF> <LF  LEN=\"24\"  H=\"YES\"  B=\"YES\"  ID=\"5\"  ID_NAME=\"APPLICATION LABEL\" >American Express</LF> <LF  LEN=\"24\"  ID=\"8\"  ID_NAME=\"PAN\" >***********0001</LF> <LF  LEN=\"24\"  ID=\"10\"  ID_NAME=\"PAN SEQUENCE NUMBER\" >PAN SEQ NO. 00</LF> <LF  LEN=\"24\"  ID=\"13\"  ID_NAME=\"DATA SOURCE\" >ICC</LF> <LF  LEN=\"24\"  ID=\"14\"  ID_NAME=\"TRANSACTION TYPE\" >SALE COMPLETION</LF> <LF  LEN=\"24\"  ID=\"15\"  ID_NAME=\"AMOUNT\" >AMOUNT            ?30.00</LF> <LF  LEN=\"24\"  H=\"YES\"  B=\"YES\"  ID=\"39\"  ID_NAME=\"TOTAL AMOUNT\" >TOTAL             ?30.00</LF> <LF  LEN=\"23\"  W=\"YES\"  B=\"YES\"  ID=\"16\"  ID_NAME=\"CVM RESULT\" >SIGNATURE VERIFIED</LF> <LF  LEN=\"24\"  ID=\"21\"  ID_NAME=\"COURTESY MESSAGE\" >Thank You</LF> <LF  LEN=\"24\"  ID=\"17\"  ID_NAME=\"DATE / TIME\" >07/07/20 08:39</LF> <LF  LEN=\"24\"  ID=\"18\"  ID_NAME=\"MESSAGE\" >AUTH CODE:080808</LF> <LF  LEN=\"24\"  ID=\"94\"  ID_NAME=\"FINAL MESSAGE\" >     CUSTOMER COPY      </LF> <LF  LEN=\"24\"  ID=\"94\"  ID_NAME=\"FINAL MESSAGE\" > PLEASE RETAIN RECEIPT</LF> </RECEIPT> </CREDIT_CARD_RECEIPT>";

            Utilities.Log(string.Empty);
            Utilities.Log("--");
            Utilities.Log("Split Sale Report Request packet: ");
            Utilities.Log(xml);
        }

        public void ReportRequest() {
            string xml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>< CREDIT_CARD_RECEIPT >< RECEIPT STYPE =\"AUXILIARY\" ><LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"240\" ID_NAME=\"Ticket Name\" >END OF DAY</LF> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"240\" ID_NAME=\"Ticket Name\" >Z BALANCES</LF> <LF LEN=\"24\" ID=\"240\" ID_NAME=\"Ticket Name\" >Totals reset</LF> <LF LEN=\"24\" H=\"YES\" B=\"YES\" ID=\"1\" ID_NAME=\"MERCHANT NAME\" >Global Payments </LF> <LF LEN=\"24\" ID=\"4\" ID_NAME=\"TID\" >TID:22160390</LF> <LF LEN=\"24\" ID=\"17\" ID_NAME=\"DATE/TIME\" >08/07/20 03:50</LF> <LF LEN=\"24\" ID=\"24\" ID_NAME=\"HANDSET\" >HANDSET:01</LF> <LO ID=\"24\" >DLS</LO> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"252\" ID_NAME=\"Field Name\" >GRAND TOTALS</LF> <LF LEN=\"24\" ID=\"253\" ID_NAME=\"Since\" >Since 08/07/20 03:49</LF> <LF LEN=\"24\" H=\"YES\" B=\"YES\" ID=\"246\" ID_NAME=\"ISS TOTALS\" >No Business</LF> <LO ID=\"246\" >DLS</LO> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"94\" ID_NAME=\"FINAL MESSAGE\" >REPORT COMPLETE</LF> <LO ID=\"254\" >TF</LO> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"240\" ID_NAME=\"Ticket Name\" >END OF DAY</LF> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"240\" ID_NAME=\"Ticket Name\" >BANKING </LF> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"240\" ID_NAME=\"Ticket Name\" > </LF> <LF LEN=\"24\" H=\"YES\" B=\"YES\" ID=\"1\" ID_NAME=\"MERCHANT NAME\" >Global Payments </LF> <LF LEN=\"24\" ID=\"4\" ID_NAME=\"TID\" >TID:22160390</LF> <LF LEN=\"24\" ID=\"17\" ID_NAME=\"DATE/TIME\" >08/07/20 03:50</LF> <LF LEN=\"24\" ID=\"24\" ID_NAME=\"HANDSET\" >HANDSET:01</LF> <LO ID=\"24\" >DLS</LO> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"241\" ID_NAME=\"ACQ NAME\" > Global Payments</LF> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"242\" ID_NAME=\"ACQ RESULT\" >TOTALS CONFIRMED</LF> <LF LEN=\"24\" ID=\"17\" ID_NAME=\"DATE/TIME\" >08/07/20 03:50</LF> <LF LEN=\"24\" ID=\"3\" ID_NAME=\"MERCHANT ID\" >M:79807901</LF> <LF LEN=\"24\" ID=\"4\" ID_NAME=\"TID\" >TID:22160390</LF> <LF LEN=\"24\" ID=\"243\" ID_NAME=\"CURRENT SESSION NUMBER\" >CURRENT SESSION: 5</LF> <LF LEN=\"24\" H=\"YES\" B=\"YES\" ID=\"244\" ID_NAME=\"CURRENT SESSION TOTALS\" >No Business</LF> <LF LEN=\"24\" ID=\"247\" ID_NAME=\"PREVIOUS SESSION NUMBER\" >PREVIOUS SESSION:</LF> <LF LEN=\"24\" ID=\"248\" ID_NAME=\"TXN RANGE\" >Txn Nos 0224-0244</LF> <LF LEN=\"24\" ID=\"249\" ID_NAME=\"TXN TOTALS\" >Sales 7 � 584.45</LF> <LF LEN=\"24\" ID=\"249\" ID_NAME=\"TXN TOTALS\" >Refunds 1 � 100.00</LF> <LF LEN=\"24\" ID=\"249\" ID_NAME=\"TXN TOTALS\" >TOTAL DR � 484.45</LF> <LF LEN=\"24\" ID=\"245\" ID_NAME=\"ISS NAME\" > AMERICAN EXPRESS</LF> <LF LEN=\"24\" ID=\"246\" ID_NAME=\"ISS TOTALS\" > Sales 6 � 284.45</LF> <LF LEN=\"24\" ID=\"246\" ID_NAME=\"ISS TOTALS\" > Refunds 1 � 100.00</LF> <LF LEN=\"24\" ID=\"246\" ID_NAME=\"ISS TOTALS\" > SUB-TOTAL DR � 184.45</LF> <LF LEN=\"24\" ID=\"245\" ID_NAME=\"ISS NAME\" > MASTERCARD</LF> <LF LEN=\"24\" ID=\"246\" ID_NAME=\"ISS TOTALS\" > Sales 1 � 300.00</LF> <LF LEN=\"24\" ID=\"246\" ID_NAME=\"ISS TOTALS\" > SUB-TOTAL DR � 300.00</LF> <LF LEN=\"24\" ID=\"250\" ID_NAME=\"CURRENT TXN\" >TXN 0245</LF> <LF LEN=\"24\" ID=\"18\" ID_NAME=\"MESSAGE\" >COMPLETED</LF> <LF LEN=\"24\" ID=\"33\" ID_NAME=\"DIAG\" >DIAG 76</LF> <LF LEN=\"24\" ID=\"33\" ID_NAME=\"DIAG\" >SESSION NOW CHANGED TO 6</LF> <LO ID=\"33\" >DLS</LO> <LF LEN=\"23\" W=\"YES\" B=\"YES\" ID=\"251\" ID_NAME=\"TICKET FINAL MESSAGE\" >REPORT COMPLETE</LF> <LF LEN=\"24\" H=\"YES\" B=\"YES\" ID=\"251\" ID_NAME=\"TICKET FINAL MESSAGE\" >TAXFREE REPORT</LF> <LF LEN=\"24\" ID=\"251\" ID_NAME=\"TICKET FINAL MESSAGE\" >TID:22160390</LF> <LF LEN=\"24\" ID=\"251\" ID_NAME=\"TICKET FINAL MESSAGE\" >08/07/20 03:50</LF> <LF LEN=\"24\" ID=\"251\" ID_NAME=\"TICKET FINAL MESSAGE\" >Refund Total: 25</LF> <LF LEN=\"24\" ID=\"251\" ID_NAME=\"TICKET FINAL MESSAGE\" >Refund Uploaded: 0</LF> <LF LEN=\"24\" H=\"YES\" B=\"YES\" ID=\"251\" ID_NAME=\"TICKET FINAL MESSAGE\" >UPLOAD INCOMPLETE</LF> </RECEIPT> </CREDIT_CARD_RECEIPT>";

            Utilities.Log(string.Empty);
            Utilities.Log("--");
            Utilities.Log("Report Request packet: ");
            Utilities.Log(xml);
        }
    }
}
