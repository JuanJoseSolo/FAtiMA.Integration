using System;
using System.Collections.Generic;
using System.Text;
using EmotionalAppraisal.DTOs;
using WellFormedNames;
using IntegratedAuthoringTool;

namespace EmotionRegulation.Components
{

    public class RequiredData
    {
        public List<AppraisalRuleDTO> EventsToAvoid { set; get; }
        public List<ActionsforEvent> ActionsForEvent { set; get; } /// <summary>
        /// Pienso que debe debe de ser una lista de este tipo de datos.
        /// </summary>
        public List<Name> EventsToReappraisal { set; get; }
        public IntegratedAuthoringToolAsset IAT_FAtiMA { get; set; }
    }
}
