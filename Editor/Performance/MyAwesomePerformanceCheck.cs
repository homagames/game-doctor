using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    public class MyAwesomePerformanceCheck : FixableCheck
    {
        public override string Description { get; }
        public override List<string> CompatibleProjectTypes { get; }

        public override CheckInstance Execute()
        {
            var result = new CheckInstance();
            MyAsyncProcess(result);
            return result;
        }

        private async Task MyAsyncProcess(CheckInstance instance)
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(100);
                instance.Progress += 0.1f;
            }
        }

        public override void Fix(CheckInstance checkInstance)
        {
            checkInstance.Passed = true;
        }
    }
}