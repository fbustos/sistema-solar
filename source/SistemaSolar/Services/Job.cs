using System;

namespace Services
{
    public class Job
    {
        public Job() { }

        public void Run()
        {
            var tenYears = (DateTime.Today.AddYears(10) - DateTime.Today).TotalDays;

            for (var i = 0; i <= tenYears; i++)
            {

            }
        }
    }
}
