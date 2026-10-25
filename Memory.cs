
using System;
using System.Threading.Tasks;
using MatrixAICommon;
using Serilog;

namespace com.Brainsoft.NeurPsy.Experiments
{
    public class Memory
    {
        //======================================================================
        //----------------------------------------------------------------------
        // PUBLIC PROPERTIES
        //----------------------------------------------------------------------
        //======================================================================
        public ulong Index { get; set; } = 0;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime? Timestamp { get; set; } = null;
        public double InputTimeLimit { get; set; } = 0.5;
        public double[] MatchDisplayTimes { get; set; } = null;
        public double[] LatencyTimes { get; set; } = null;
        public double[] FalsePosTimes { get; set; } = null;
        public bool[] DetectSuccess { get; set; } = null;
        //======================================================================
        //----------------------------------------------------------------------
        // REMOTE INTERFACE
        //----------------------------------------------------------------------
        //======================================================================
        public class DB : BackendConnection
        {
            public static async Task<Memory> Insert(Memory tuple)
            {
                try
                {
                    if (tuple.SubjectName.Length == 0)
                        throw new BadRequestException("`SubjectName` cannot be empty.");

                    var req = new JSONBuilder();
                    req.Add("subject_name", tuple.SubjectName);
                    req.Add("timestamp", tuple.Timestamp);
                    req.Add("input_time_limit", tuple.InputTimeLimit);
                    req.Add("match_times", tuple.MatchDisplayTimes);
                    req.Add("latency_times", tuple.LatencyTimes);
                    req.Add("falsePos_times", tuple.FalsePosTimes);
                    req.Add("detectSuccess", tuple.DetectSuccess);
                    byte[] res = await (await Fetch("/neuropsy/memory/insert", req)).Content.ReadAsByteArrayAsync();
                    tuple.Index = BitConverter.ToUInt64(res, 0);
                    return tuple;
                }
                catch (Exception ex)
                {
                    Log.Error("{Cognitive load - insert error {0}}", ex.Message);
                    return null;
                }
            } // end Insert()
        } // end class DB
    } // end class Memory

}
