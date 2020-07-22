
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PATT_Simulator {
    class Program {
        static void Main(string[] args) {
            Utilities.Log("*****ASSUME THAT THIS CONSOLE IS YOUR INGENICO TERMINAL*****");
            Utilities.Log(string.Empty);

            while (true) {
                BusinessLogic.ExtendedTableLockEnable();
                BusinessLogic.EODProcessEnable();
                BusinessLogic.AdditionalMessageFlowEnable();
                BusinessLogic.GenerateTableList();
                BusinessLogic.TableLock();

                bool isToIdle = BusinessLogic.TerminalToIdle();
                if (!isToIdle) {
                    BusinessLogic.GetReceipt();
                }

                bool transactionRequest = BusinessLogic.TransactionOutcome();
                if (!transactionRequest) {
                    throw new Exception("Transaction Outcome request failed");
                }

                if (BusinessLogic.isAdditionalMsgFlowEnable == "1") {
                    BusinessLogic.AdditionalMessageFlow();
                } 

                if (BusinessLogic.isEODEnable) {
                    BusinessLogic.EODProcess();
                }

                Utilities.Log(string.Empty);
                Thread.Sleep(3000);
            }
        }
    }
}
