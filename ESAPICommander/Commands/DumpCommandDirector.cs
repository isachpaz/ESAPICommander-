﻿using System;
using System.Linq;
using ESAPICommander.ArgumentConfig;
using ESAPICommander.Interfaces;
using ESAPICommander.Logger;
using ESAPICommander.Proxies;
using VMS.TPS.Common.Model;
using VMS.TPS.Common.Model.API;

namespace ESAPICommander.Commands
{
    public class DumpCommandDirector : BaseCommandDirector
    {
        private DumpArgOptions _options;

        public DumpCommandDirector(DumpArgOptions options, IEsapiCalls esapi, ILog log) : base(esapi, log)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public override int Run()
        {
            if (!IsPIZAvailable(_options.PIZ))
            {
                _log.AddInfo($"Patient with PIZ={_options.PIZ} cannot be found...");
                _log.AddInfo("Process stopped.");
                return -1;
            }
            else
            {
                _log.AddInfo($"Patient with PIZ={_options.PIZ} found.");
            }

            //var patient = Esapi.OpenPatient(_options.PIZ);
            Esapi.OpenPatient(_options.PIZ);
            var courses = Esapi.GetCourses();

            //foreach (Course course in patient.Courses)
            foreach (ICourse course in courses)
            {
                foreach (IPlanSetup planSetup in Esapi.GetPlanSetupsFor(course.Id))
                    //foreach (PlanSetup plan in course.PlanSetups)
                {
                    _log.AddInfo($"Course: {course.Id}");
                    _log.AddInfo($"Plan: {planSetup?.Id} -> Number of fractions: {planSetup?.NumberOfFractions}, " +
                                 $"Prescription dose: {planSetup?.TotalDose}");
                    _log.AddInfo($"StructureSet: {planSetup?.StructureSet?.Id}");
                    //_log.AddInfo($"Structures: {string.Join(", ", planSetup?.StructureSet?.Structures ?? Array.Empty<Structure>())}");
                    if (planSetup?.StructureSet != null)
                        _log.AddInfo($"Structures: {string.Join(", ", planSetup?.StructureSet?.Structures)}");
                    _log.AddInfo("");
                }

                //foreach (PlanSum plan in course.PlanSums)
                //{
                //    _log.AddInfo($"Course: {course.Id}");
                //    _log.AddInfo($"Summed Plan: {plan?.Id} -> {string.Join(", ", plan?.PlanSetups?.Select(x => x.Id) ?? Array.Empty<string>())}");
                //    _log.AddInfo($"StructureSet: {plan?.StructureSet?.Id}");
                //    _log.AddInfo($"Structures: {string.Join(", ", plan?.StructureSet?.Structures ?? Array.Empty<Structure>())}");
                //    _log.AddInfo("");
                //}
            }

            return 0;
        }

    }
}