using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatrixAICommon;
using Serilog;

namespace com.Brainsoft.NeurPsy.Experiments
{
    public class Attention
    {
        public ulong Index { get; set; }
        public string SubjectName { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime TimeLastChar { get; set; }
        public double[] MatchTimes { get; set; }
        public double[] latency_times { get; set; }
        public double[] FalsePositives { get; set; }
        public bool[] detectSuccess { get; set; }

        public class DB : BackendConnection
        {
            public static async Task<Attention> Insert(Attention tuple)
            {
                if (tuple.SubjectName == null || tuple.SubjectName.Length == 0)
                    throw new BadRequestException("`SubjectName` must be non-NULL and non-empty.\n");

                var body = new JSONBuilder();
                body.Add("subject_name", tuple.SubjectName);
                CommonJSONEncoding(body, tuple);

                //Log.Debug(body.ToString());
                byte[] res = await (await Fetch("/neuropsy/attention/insert", body)).Content.ReadAsByteArrayAsync();
                tuple.Index = BitConverter.ToUInt64(res, 0);
                return tuple;
            } // end Insert()

            //---------------------------------------------------------------------
            public static async Task<ulong> Update(Attention tuple)
            {
                if (tuple.Index == 0)
                    throw new BadRequestException("`Index` must be non-zero.\n");

                var body = new JSONBuilder();
                body.Add("index", tuple.Index);
                body.Add("subject_name", tuple.SubjectName);
                CommonJSONEncoding(body, tuple);

                byte[] res = await (await Fetch("/neuropsy/attention/update", body)).Content.ReadAsByteArrayAsync();
                return BitConverter.ToUInt64(res, 0);
            } // end Update()

            //---------------------------------------------------------------------
            private static void CommonJSONEncoding(JSONBuilder body, Attention fields)
            {
                if (fields == null)
                    return;
                body.Add("timestamp", fields.Timestamp);
                body.Add("match_times", fields.MatchTimes);
                body.Add("latency_times", fields.latency_times);
                body.Add("false_positives", fields.FalsePositives);
                body.Add("detectSuccess", fields.detectSuccess);
            } // end CommonJSONEncoding()
        } // end class DB()
    } // end class Attention

}
