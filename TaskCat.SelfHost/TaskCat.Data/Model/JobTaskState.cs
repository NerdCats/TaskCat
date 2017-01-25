namespace TaskCat.Data.Model
{
    using System;
    using System.Collections.Generic;

    public class JobTaskState
    {
        public const string PENDING = "PENDING";
        public const string IN_PROGRESS = "IN_PROGRESS";
        public const string COMPLETED = "COMPLETED";
        public const string CANCELLED = "CANCELLED";

        static Dictionary<string, int> stateIndexDict = new Dictionary<string, int>();

        static JobTaskState()
        {
            stateIndexDict.Add(PENDING, 0);
            stateIndexDict.Add(IN_PROGRESS, 1);
            stateIndexDict.Add(COMPLETED, 2);
            stateIndexDict.Add(CANCELLED, 3);
        }

        private static void CheckInvalidState(string stateA, string stateB)
        {
            if (!stateIndexDict.ContainsKey(stateA))
                throw new ArgumentException($"Invalid state {stateA} encountered");

            if (!stateIndexDict.ContainsKey(stateB))
                throw new ArgumentException($"Invalid state {stateB} encountered");
        }

        public static bool GreaterThan(string stateA, string stateB)
        {
            CheckInvalidState(stateA, stateB);
            return stateIndexDict[stateA] > stateIndexDict[stateB];
        }

        public static bool GreaterThanOrEqualTo(string stateA, string stateB)
        {
            CheckInvalidState(stateA, stateB);
            return stateIndexDict[stateA] >= stateIndexDict[stateB];
        }

        public static bool LesserThan(string stateA, string stateB)
        {
            CheckInvalidState(stateA, stateB);
            return stateIndexDict[stateA] < stateIndexDict[stateB];
        }
  
        public static bool LesserThanOrEqualTo(string stateA, string stateB)
        {
            CheckInvalidState(stateA, stateB);
            return stateIndexDict[stateA] <= stateIndexDict[stateB];
        }
    }
}
