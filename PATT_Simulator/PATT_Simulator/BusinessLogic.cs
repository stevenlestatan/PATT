using System;
using System.Threading;

namespace PATT_Simulator {
    public static class BusinessLogic {
        private static EPos epos;
        private static Terminal terminal;
        private static string tableId;
        private static bool isExtendedTableLock;
        public static string isAdditionalMsgFlowEnable;
        public static bool isEODEnable;

        public static void ExtendedTableLockEnable() {
            Utilities.Log("You want to enable the Extended Table Lock? YES or NO");

            A:
            string read = Console.ReadLine();

            if (read.Equals("NO") || read.Equals("no")) {
                isExtendedTableLock = false;
            } else if (read.Equals("YES") || read.Equals("yes")) {
                isExtendedTableLock = true;
            } else {
                Utilities.Log("Please respond with appropriate answer; YES or NO only");
                goto A;
            }
        }

        public static void EODProcessEnable() {
            Utilities.Log(string.Empty);
            Utilities.Log("You want to enable the End of Day Process? YES or NO");

        A:
            string read = Console.ReadLine();

            if (read.Equals("NO") || read.Equals("no")) {
                isEODEnable = false;
            } else if (read.Equals("YES") || read.Equals("yes")) {
                isEODEnable = true;
            } else {
                Utilities.Log("Please respond with appropriate answer; YES or NO only");
                goto A;
            }
        }

        public static void AdditionalMessageFlowEnable() {
            Utilities.Log(string.Empty);
            Utilities.Log("You want to enable the Additional Message Flow List? YES or NO");

        A:
            string read = Console.ReadLine();

            if (read.Equals("NO") || read.Equals("no")) {
                isAdditionalMsgFlowEnable = "0";
            } else if (read.Equals("YES") || read.Equals("yes")) {
                isAdditionalMsgFlowEnable = "1";
            } else {
                Utilities.Log("Please respond with appropriate answer; YES or NO only");
                goto A;
            }
        }

        public static bool TerminalToIdle() {
            bool result = false;
            Utilities.Log("You want to proceed or unlock the table already? PROCEED or UNLOCK");

        A:
            string read = Console.ReadLine();

            if (read.Equals("PROCEED") || read.Equals("proceed")) {
                Utilities.Log(string.Empty);
                Utilities.Log("Proceeding...");
            } else if (read.Equals("UNLOCK") || read.Equals("unlock")) {
                TableUnlock();
                result = true;
            } else {
                Utilities.Log("Please respond with appropriate answer; PROCEED or UNLOCK only");
                goto A;
            }

            Utilities.Log(string.Empty);
            Thread.Sleep(1500);

            return result;
        }

        public static void GenerateTableList() {
            terminal = new Terminal(4);
            epos = new EPos("tableList.txt");

            Utilities.Log(string.Empty);
            Utilities.Log("Would like to list down all the tables available? YES or NO");

        A:
            string readLine = Console.ReadLine();
            if (readLine.Equals("NO") || readLine.Equals("no")) {
                Utilities.Log(string.Empty);
                epos.GetAllWaiterId();
                string waiterId = string.Empty;

                while (true) {
                    Thread.Sleep(1500);
                    Utilities.Log(string.Empty);
                    Utilities.Log("To generate available table(s) per Waiter ID, please enter your Waiter's ID: ");
                    waiterId = Console.ReadLine();

                    if (epos.waiterId.Contains(waiterId)) {
                        break;
                    }
                }

                while (true) {
                    bool isRequestGranted = terminal.GrantRequest(waiterId);
                    if (isRequestGranted) {
                        Utilities.Log(string.Empty);
                        epos.GetTablesPerWaiterId(waiterId);
                        break;
                    }
                }
            } else if (readLine.Equals("YES") || readLine.Equals("yes")) {
                while (true) {
                    bool isRequestGranted = terminal.GrantRequest("00");
                    if (isRequestGranted) {
                        epos.GetAllTableList();
                        break;
                    }
                }
            } else {
                Utilities.Log("Please respond with appropriate answer; YES or NO only");
                goto A;
            }

            Thread.Sleep(1500);
            Utilities.Log(string.Empty);
        }

        public static void TableLock() {
            terminal = new Terminal(1);
            Utilities.Log("To lock your table, please enter the table id where you are in: ");

        A:
            string read = Console.ReadLine();
            tableId = string.Format("L0{0}{1}", read.Length, read);
            char[] cArr = read.ToCharArray();

            char cTableId = '\0';
            foreach (char c in cArr) {
                cTableId += c;
            }

            if (epos.tableList.Contains(cTableId)) {
                while (true) {
                    bool isTableLock = terminal.GrantRequest(tableId, isExtendedTableLock, epos);
                    if (isTableLock) {
                        epos.GrantResponse(isAdditionalMsgFlowEnable);
                        break;
                    }
                }
            } else {
                Utilities.Log("Please respond with appropriate table ids above: ");
                goto A;
            }

            Thread.Sleep(1500);
            Utilities.Log(string.Empty);
        }

        public static void TableUnlock() {
            terminal = new Terminal(2);
            Utilities.Log(string.Empty);
            Utilities.Log(string.Format("Unlocking table {0}...", tableId));
            Thread.Sleep(1500);

            while (true) {
                bool isTableUnlock = terminal.GrantRequest(tableId);
                if (isTableUnlock) {
                    Thread.Sleep(1500);
                    Utilities.Log(string.Empty);
                    Utilities.Log("Ingenico Terminal returned to idle screen");
                    break;
                }
            }

            Thread.Sleep(1500);
        }

        public static void GetReceipt() {
            terminal = new Terminal(3);
            Utilities.Log("Would you like to request a receipt?: YES or NO");

        A:
            string read = Console.ReadLine();

            if (read.Equals("NO") || read.Equals("no")) {
                Utilities.Log(string.Empty);
                Utilities.Log("Proceeding...");
                Thread.Sleep(1500);
            } else if (read.Equals("YES") || read.Equals("yes")) {
                while (true) {
                    bool isRequestGranted = terminal.GrantRequest("0000000000");
                    if (isRequestGranted) {
                        Utilities.Log(string.Empty);
                        epos.XMLReceiptResponse(tableId);
                        break;
                    }
                }
            } else {
                Utilities.Log("Please respond with appropriate answer; YES or NO only");
                goto A;
            }

            Utilities.Log(string.Empty);
            Thread.Sleep(1500);
        }

        public static bool TransactionOutcome() {
            terminal = new Terminal(null);

            Utilities.Log("Transaction Outcome: ");
            bool transactionRequest = terminal.GrantTransactionRequest();

            if (transactionRequest) {
                Utilities.Log(string.Empty);
                epos.GrantResponse(isAdditionalMsgFlowEnable);
            }

            return transactionRequest;
        }

        public static void AdditionalMessageFlow() {
            Utilities.Log(string.Empty);
            Utilities.Log("Additional Message List: ");
            bool isRequestGranted = terminal.GrantAdditionalMsgRequest();
            if (isRequestGranted) {
                Utilities.Log(string.Empty);
                epos.XMLAdditionalMsgListResponse();
            }

            Utilities.Log(string.Empty);
            Utilities.Log("Generating all additional data...");
            Thread.Sleep(3000);

            terminal.SplitsaleReportRequest();
            terminal = new Terminal(3);
            bool finalReceipt = terminal.GrantRequest("0000000000");
            if (!finalReceipt) {
                throw new Exception("Failed to generate Final Receipt");
            }
            terminal.TicketReportRequest();
        }

        public static void EODProcess() {
            Utilities.Log("End of Day Process: ");
            bool isRequestGranted = terminal.GrantTransferDataRequest();
            if (isRequestGranted) {
                Utilities.Log(string.Empty);
                epos.GrantResponse(isAdditionalMsgFlowEnable);
            }

            terminal.ReportRequest();
            epos.GrantResponse(isAdditionalMsgFlowEnable);
        }
    }
}
