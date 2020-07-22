using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PATT_Simulator {
    static class Utilities {
        public static void Log(string msg) {
            Console.WriteLine(msg);
        }

        public static bool IsObjectNullOrEmpty(this object obj) {
            bool result = false;
            if (string.IsNullOrEmpty(obj.ToString())) {
                result = true;
            }

            return result;
        }

        public static string CreateTemporaryPath(string ext) {
            String path = Path.GetTempPath();
            string fileName = Guid.NewGuid().ToString() + ext;
            string temporaryPath = Path.Combine(path, fileName);
            return temporaryPath;
        }

        public static string BuildSerialRequest(string xml) {
            return string.Format("{0}{1}{2}{3}", (char)0x02, xml, (char)0x03, byte.Parse(xml));
        }

        public static string BuildIPRequest(string[] header, string xml) {
            return string.Format("{0}{1}", HeaderLength(header), xml);
        }

        private static int HeaderLength(string[] buffer) {
            var fHex = Convert.ToInt64(buffer[0]).ToString("X2");
            var sHex = Convert.ToInt64(buffer[1]).ToString("X2");
            var _hex = fHex + sHex;

            return int.Parse(_hex, System.Globalization.NumberStyles.HexNumber);
        }

        private static byte[] CalculateLRC(string requestMessage) {
            byte[] bytes = Encoding.ASCII.GetBytes((requestMessage + (char)0x03));
            byte lrc = 0;
            for (int i = 0; i < bytes.Length; i++) {
                lrc ^= bytes[i];
            }
            bytes = new byte[] { lrc };
            return bytes;
        }
    }
}
