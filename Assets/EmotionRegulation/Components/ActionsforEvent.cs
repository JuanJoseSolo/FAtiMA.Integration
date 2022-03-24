using System.Collections.Generic;
using WellFormedNames;
using ActionLibrary.DTOs;
using ActionLibrary;
using EmotionalAppraisal.DTOs;
using FLS.Rules;
using System.Linq;

namespace EmotionRegulation.Components
{
    public class ActionsforEvent
    {
        private List<KeyValuePair<string, float>> NameActions;
        private AppraisalRuleDTO nameEventToReact;
        private ActionRuleDTO setAction;
        private string actionName;
    


        public List<AppraisalRuleDTO> AppraisalRulesOfEvent { get; set; }
        public List<KeyValuePair<string, float>> ActionNameValue { get; set; }
        public Name EventName { get; set; }

        //public string ActionName { get => actionName; set => SetAction(value); }

        void SetAction(string actionName)
        {
            this.actionName = actionName;
            var oldAction = nameEventToReact.EventMatchingTemplate.GetNTerm(3);
            var ruleDTO = new ActionRuleDTO
            {
                Action = Name.BuildName(actionName + " in " + oldAction),

            };
            setAction = ruleDTO;
        }
    }
}
