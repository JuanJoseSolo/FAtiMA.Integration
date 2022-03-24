using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ActionLibrary;
using ActionLibrary.DTOs;
using EmotionalAppraisal;
using EmotionalAppraisal.DTOs;
using EmotionalAppraisal.OCCModel;
using EmotionRegulation.BigFiveModel;
using FLS;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using WellFormedNames;
using EmotionalDecisionMaking;

namespace EmotionRegulation.Components
{
    public class EmotionRegulationModel
    {

        public bool successfulStrategy { get; private set; }
        public Strategies StrategyApplied { get; internal set; }

        public const string SITUATION_SELECTION = "Situation Selection";
        public const string SITUATION_MODIFICATION = "Situation Modification";
        public const string ATTENTION_DEPLOYMENT = "Attention Deployment";
        public const string COGNITIVE_CHANGE = "Cognitive Change";
        public const string RESPONSE_MODULATION = "Response Modulation";

        internal IAction newAction;
        LoadingEmotionalRegulation internalSettings;
        RolePlayCharacterAsset character; //auxCharacter;
        PersonalityDTO personality;
        Name eventMatchingTemplate; 
        Name eventName;
        bool isSpeak;
        BaseAgent agent;
        RequiredData requiredData;
        DialogueStateActionDTO dialogOfEvent;
        string initiator;
        IAction decision;
        EmotionInformation emotionInformation;


        public enum Strategies
        {
            SituationSelection = 1,
            SituationModification = 2,
            AttentionalDeployment = 3,
            CognitiveChange = 4,
            ResponseModulation = 5
        };

        internal EmotionRegulationModel(IAction decision, LoadingEmotionalRegulation info)
        {
            this.decision = decision;
            this.initiator = info.initiator;
            requiredData = info.requiredData;
            agent = info.baseAgent;
            isSpeak = info.IsSpeak;
            internalSettings = info;
            character = info.baseAgent.FAtiMACharacter;
            eventMatchingTemplate = info.EventMatchingTemplate;
            eventName = info.EventMatchingTemplate.GetNTerm(3);
            personality = info.baseAgent.personality;
            dialogOfEvent = info.DialogOfEvent;
            emotionInformation = info.EmotionInformation;
            newAction = StartRegulation();
        }

        private IAction StartRegulation()
        {

            var appRuleOfEvent = internalSettings.AppraisalRulesOfEvent;
            StrategiesOfAntecedents(appRuleOfEvent);
            //if (!successfulStrategy)
                //StrategiesOfResponse();

            if (successfulStrategy)
            {
                var decisions = character.Decide().ToList();
                return character.Decide().FirstOrDefault(); 
            }
            else
                return null;
        }

        private void StrategiesOfAntecedents(List<AppraisalRuleDTO> RulesOfEvent)
        {
            var isAvoid = internalSettings.IsAvoided;
            var existsActions = internalSettings.ExistActionsForSituation;
            var relatedActions = internalSettings.requiredData.ActionsForEvent; //cuando no se carga es núlo, lo cual esta bien....
            var existsEventsForAttention = internalSettings.ExistEventsForAttention;
            var pastEvents = internalSettings.PastEvents;
            var agentStrategies = agent.StrategiesToApply;
            int detectedStrategy = 0;

            foreach (var strategy in agentStrategies)
            {
                detectedStrategy++;
                if (strategy == SITUATION_SELECTION)
                { successfulStrategy = SituationSelection(isAvoid, RulesOfEvent); if (successfulStrategy) break; continue; }
                
                else
                if (strategy == SITUATION_MODIFICATION && !successfulStrategy && existsActions)
                { detectedStrategy++; successfulStrategy = SituationModification(relatedActions.First(), RulesOfEvent); if (successfulStrategy) break; continue; }
                
                else
                if (strategy == ATTENTION_DEPLOYMENT && !successfulStrategy && existsEventsForAttention)
                { detectedStrategy++; successfulStrategy = AttentionDeployment(RulesOfEvent, pastEvents); if (successfulStrategy) break; continue; }
               /*
                else
                if (strategy == COGNITIVE_CHANGE && !successfulStrategy && existsEvts)
                { successfulStrategy = CognitiveChange(emotions, appRuleOfEvent, eventsForReappraisal); if (successfulStrategy) break; continue; }
                */
            }
            if (successfulStrategy)
                StrategyApplied = (Strategies)detectedStrategy;
        }

        private void StrategiesOfResponse()
        {
            character.Perceive(eventMatchingTemplate);
            //successfulStrategy = ResponseModulation();
            StrategyApplied = (Strategies)5;
        }

        #region Estrategies of Gross Model

        //Situation Selection 
        private bool SituationSelection(bool isAvoid, List<AppraisalRuleDTO> RulesOfEvent)
        {
            /// El parámetro RulesOfEvents contiene las reglas del evento actual, pero el 
            /// EventMatchingTemplate original de la forma {Event(Action-End, SELF, Speak(*, *, *, sth), sth), 
            /// es decir, si el usuario declaró el evento con parametros universales, es este el EMT que tiene este arreglo de 
            /// AppraisalRules.
            /// 
            var appliedStrategy = false;

            if (isAvoid)
            {
                if (isSpeak)
                {
                    /// Get the parameters of the current decision name and changes its style
                    var decisionName = decision.Name; 
                    var oldStyle = decisionName.GetNTerm(4);
                    var newStyle = "Neutral";
                    var newDecisionName = decisionName.SwapTerms(oldStyle, (Name)newStyle);

                    /// Get the action rule for this event/action or decision, and create new one. Currently, I am assuming there is 
                    /// only one rule.
                    

                    var oldActionRule = character.m_emotionalDecisionMakingAsset.GetAllActionRules().FirstOrDefault(
                        act => act.Action.GetNTerm(4) == oldStyle);

                    var newPriority = Name.BuildName(float.Parse(oldActionRule.Priority.ToString()) + 1);

                    ActionRuleDTO newActionRule = new ActionRuleDTO()
                    {
                        Action = newDecisionName, ///the actions is the third element of the EventMatchingTemplate.
                        Conditions = oldActionRule.Conditions,
                        Layer = oldActionRule.Layer,
                        Priority = newPriority,
                        Target = oldActionRule.Target
                    };
                    character.m_emotionalDecisionMakingAsset.AddActionRule(newActionRule);

                    ///get the complete event matching template, and create new one.
                    var eventList = eventMatchingTemplate.GetLiterals().ToList();
                    var thisStyle = eventList[7];
                    var subject = eventList[2];

                    var newSpeakName = Name.BuildName(String.Concat(
                        eventList[3].ToString() + '(' + eventList[4] + ',' + eventList[5] + ',' + eventList[6] + ',' + newStyle + ')'));

                    var newEventName = Name.BuildName(String.Concat(
                        eventList[0].ToString() + '(' + eventList[1] + ',' + subject + ',' + newSpeakName + ',' + eventList[8]+')'));


                    ///Update de appraisals rules and remove the old one
                    ///
                    foreach (var appRule in RulesOfEvent)
                    {
                        var newAppRule = internalSettings.SetAppRules(appRule, (Name)"0", newEventName);
                        character.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(newAppRule);
                    }
                    character.m_emotionalAppraisalAsset.RemoveAppraisalRules(RulesOfEvent);

                    /// Get all information about the current decision.
                    /// I am assuming that there is only one dialog for the current decicsion.
                    var currentState = dialogOfEvent.CurrentState;
                    var nextState = dialogOfEvent.NextState;
                    var meaning = dialogOfEvent.Meaning;
                    var style = dialogOfEvent.Style;
                    var utterance = dialogOfEvent.Utterance;
                    var id = dialogOfEvent.Id;
                    var utteranceId = dialogOfEvent.UtteranceId;

                    /// Update the new dialogAction
                    var newUtterance = String.Concat($"Thinking('{utterance}')");
                    DialogueStateActionDTO newDialogStateAction = new DialogueStateActionDTO {
                        CurrentState = currentState,
                        NextState = nextState,
                        Meaning = meaning,
                        Style = newStyle,
                        Utterance = newUtterance,
                        UtteranceId = utteranceId
                    };
                    requiredData.IAT_FAtiMA.AddDialogAction(newDialogStateAction);

                    /// Mark the strategy as succesful
                    appliedStrategy = true;
                }
                else /// If action/event is not a dialog, this block will change the name of the current event.
                {
                    var eventList = eventMatchingTemplate.GetLiterals().ToList();

                    ///Building new event and adding word NOT at the event
                    eventList.RemoveAt(3);
                    var newEventName = Name.BuildName("Not-" + eventMatchingTemplate.GetNTerm(3));
                    eventList.Insert(3, newEventName);;

                    var NewEvent = Name.BuildName(eventList);

                    ///Update de appraisals rules

                    if (initiator == agent.AgentName.ToString())
                    {
                        foreach (var appRule in RulesOfEvent)
                        {
                            // Revisar si realmente se nececita del método SetAppRules (Revisar como se actualizan en las otras estrategias).
                            var newAppRule = internalSettings.SetAppRules(appRule, (Name)"0", NewEvent);
                            character.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(newAppRule);
                        }
                    }
                    else ///cuando la acción no fue realizada por el mismo agente
                    {
                        List<AppraisalRuleDTO> oldApp = new List<AppraisalRuleDTO>();
                        foreach (var appRule in RulesOfEvent)
                        {

                           oldApp.Add(character.m_emotionalAppraisalAsset.GetAllAppraisalRules().FirstOrDefault(x =>
                            x.EventMatchingTemplate == appRule.EventMatchingTemplate));
                        }

                        foreach(var app in oldApp)
                        {
                            app.AppraisalVariables.appraisalVariables.ForEach(x => x.Value = (Name)"0");
                        }
                    };



                    /// En esta parte se construye la acción que provocó que el agente perciviera un intensidad menor del evento actual.
                    /// Por el momento se construye aquí, aunque considero que podría construirse desde la clase ActionsForEvents
                    /// 
                    
                    if(initiator == agent.AgentName.ToString())
                    {
                        ///change the action of agent
                        var oldActionRule = character.m_emotionalDecisionMakingAsset.GetAllActionRules().FirstOrDefault(
                            act => act.Action == eventName);
                        var newPriority = Name.BuildName(float.Parse(oldActionRule.Priority.ToString()) + 1);



                        ActionRuleDTO action = new ActionRuleDTO()
                        {
                            Action = newEventName,
                            Conditions = oldActionRule.Conditions,
                            Layer = oldActionRule.Layer,
                            Priority = newPriority,
                            Target = oldActionRule.Target
                        };

                        character.m_emotionalDecisionMakingAsset.AddActionRule(action);
                    }
                    appliedStrategy = true;
                }

                
            }

            return appliedStrategy;

        }

        //Situation Modification 
        private bool SituationModification(ActionsforEvent reactions, List<AppraisalRuleDTO> RulesOfEvent)
        {
            bool AppliedStrategy = false;

            float UnfitPersonality = (float)((personality.Neuroticism + personality.Agreeableness) / 2);
            float FitPersonality = (float)((personality.Conscientiousness + personality.Extraversion + personality.Openness) / 3);

            //var auxEA = new EmotionalAppraisalAsset();
            //auxCharacter = new RolePlayCharacterAsset() { m_emotionalAppraisalAsset = auxEA};
            //auxCharacter.Mood = character.Mood;
            

            agent.auxCharacter.Mood = character.Mood;

            var _appraisalVariables = emotionInformation.SpecificAppVariables;
            var _appraisalVariablesCopy = emotionInformation.SpecificAppVariablesCopy;
            var _intensity = emotionInformation.EmotionDTO.Intensity;
            var _valance = emotionInformation.OCCEmotionType.Valence;
            var _OCCemotions = new List<OCCEmotionType>();
            var _newEmotions = new List<EmotionDTO>();
            var _Threshold = false;
            var _auxEvent = Name.NIL_SYMBOL;
            string newEventName = string.Empty;
            var unionOfAppVariables = _appraisalVariablesCopy.Union(RulesOfEvent.First().AppraisalVariables.appraisalVariables);
            Name newActionName = Name.NIL_SYMBOL;
            Name newPriority = Name.NIL_SYMBOL;
            ActionRuleDTO oldActionRule = new ActionRuleDTO();
            AppraisalRuleDTO newAppRule = new AppraisalRuleDTO();


            foreach (var reaction in reactions.ActionNameValue)
            {

                if (isSpeak)
                {
                    Debug.Print($"Reaction : {reaction.Key} weigth {reaction.Value}");

                    var decisionName = decision.Name;
                    var oldStyle = decisionName.GetNTerm(4);
                    var newStyle = String.Concat(reaction.Key + "TO" + oldStyle);
                    newActionName = decisionName.SwapTerms(oldStyle, (Name)newStyle);

       
                    //////////////////////////////////////////// NUEVAS REGLAS DE VALORACIÓN
                    foreach (var oldAppraisal in _appraisalVariablesCopy)
                    {
                        /// no está dando un valor correcto la función. Revisar.
                        /// Creo que la decision entre que acción es la que se va ejucutar iría aquí, en relación con la personalida.
                        var UnfitTanh = internalSettings.AppraisalFunction(_intensity, UnfitPersonality, 100, -(int)_valance);
                        var FitTanh = internalSettings.AppraisalFunction(_intensity, FitPersonality, 100, (int)_valance);
                        var tanh = (UnfitTanh + FitTanh) + 2;//quite la división entre 2
                        oldAppraisal.Value = Name.BuildName(tanh);
                    }
                    var NewVariables = unionOfAppVariables.DistinctBy(name => name.Name).ToList();
                    ///AQUÍ ES DIFERENTE PARA LAS ACCIONES SPK
                    ///

                    var oldActionName = eventMatchingTemplate.GetNTerm(3);
                    var newActionName2 = eventMatchingTemplate.GetNTerm(3).SwapTerms(oldStyle, (Name)newStyle);///es el mismo????
                    var emtVariables = eventMatchingTemplate.GetTerms().ToList();//No me deja hacer el swap.....
                    emtVariables.RemoveAt(3); emtVariables.Insert(3, newActionName);
                    var newEventMatchingTemplate = Name.BuildName(emtVariables);

                    var emtList = eventMatchingTemplate.GetTerms().ToList();
                    emtList.RemoveAt(3);
                    emtList.Insert(3, newActionName2);
                    var newEMTAS = Name.BuildName(emtList);



                    newAppRule = new AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = newEMTAS,
                        AppraisalVariables = new AppraisalVariables(NewVariables)
                    };
                    agent.auxCharacter.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(newAppRule);

                    _auxEvent = EventHelper.ActionEnd(agent.auxCharacter.CharacterName, newActionName, decision.Target);

                    agent.auxCharacter.Perceive(_auxEvent);

                    _newEmotions = agent.auxCharacter.GetAllActiveEmotions().ToList();
                    agent.auxCharacter.ResetEmotionalState();
                    agent.auxCharacter.ForgetEvent(0); //revisar si es correcto olvidar el evento con el id 0.
                    agent.auxCharacter.m_emotionalAppraisalAsset.RemoveAppraisalRules(new List<AppraisalRuleDTO>() { newAppRule });

                    foreach (var _emo in _newEmotions)
                    {
                        _OCCemotions.Add(OCCEmotionType.Parse(_emo.Type));
                    }
                    if (_OCCemotions.Any(e => e.Valence == EmotionValence.Positive))
                    {
                        _Threshold = true;
                    }
                    else _Threshold = _newEmotions.FirstOrDefault().Intensity <= personality.MaxLevelEmotion;

                    if (_Threshold)
                    {
                        oldActionRule = character.m_emotionalDecisionMakingAsset.GetAllActionRules().FirstOrDefault(
                                                        act => act.Action.GetNTerm(4) == oldStyle);
                        newPriority = Name.BuildName(float.Parse(oldActionRule.Priority.ToString()) + 1);

                        for (int i = 0; i < _appraisalVariables.Count(); i++)
                        {
                            _appraisalVariables[i].Value = _appraisalVariablesCopy[i].Value;
                        }

                        var currentState = dialogOfEvent.CurrentState;
                        var nextState = dialogOfEvent.NextState;
                        var meaning = dialogOfEvent.Meaning;
                        var style = dialogOfEvent.Style;
                        var utterance = dialogOfEvent.Utterance;
                        var id = dialogOfEvent.Id;
                        var utteranceId = dialogOfEvent.UtteranceId;

                        /// Update the new dialogAction
                        var newUtterance = String.Concat($"{reaction.Key}:I don't think so dude !");
                        DialogueStateActionDTO newDialogStateAction = new DialogueStateActionDTO
                        {
                            CurrentState = currentState,
                            NextState = nextState,
                            Meaning = meaning,
                            Style = newStyle,
                            Utterance = newUtterance,
                            UtteranceId = utteranceId
                        };
                        requiredData.IAT_FAtiMA.AddDialogAction(newDialogStateAction);

                        AppliedStrategy = true;
                        break;

                    }

                    break;

                }
                else
                {

                    newActionName = Name.BuildName(String.Concat(reaction.Key + "-TO-" + eventName));
                    foreach (var oldAppraisal in _appraisalVariablesCopy)
                    {
                        Debug.Print($"Reaction : {reaction.Key} weigth {reaction.Value}");
                        /// Creo que la decision entre que acción es la que se va ejucutar iría aquí, en relación con la personalida.
                        var UnfitTanh = internalSettings.AppraisalFunction(_intensity, UnfitPersonality, 100, -(int)_valance);
                        var FitTanh = internalSettings.AppraisalFunction(_intensity, FitPersonality, 100, (int)_valance);
                        var tanh = (UnfitTanh + FitTanh) / 2;
                        oldAppraisal.Value = Name.BuildName(tanh);
                    }
                    var NewVariables = unionOfAppVariables.DistinctBy(name => name.Name).ToList();
                    var oldEventName = eventMatchingTemplate.GetNTerm(3);
                    var newEMT = eventMatchingTemplate.SwapTerms(oldEventName, (Name)newActionName);
                    newAppRule = new AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = newEMT, //no es correcto el EMT, revisar.
                        AppraisalVariables = new AppraisalVariables(NewVariables)
                    };
                    //auxCharacter.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(newAppRule);
                    agent.auxCharacter.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(newAppRule);

                    _auxEvent = EventHelper.ActionEnd(agent.auxCharacter.CharacterName,newActionName, decision.Target);
                    
                    //auxCharacter.Perceive(_auxEvent);
                    agent.auxCharacter.Perceive(_auxEvent);
                    //_newEmotions = auxCharacter.GetAllActiveEmotions().ToList();

                    _newEmotions = agent.auxCharacter.GetAllActiveEmotions().ToList();
                    agent.auxCharacter.ResetEmotionalState();
                    agent.auxCharacter.ForgetEvent(0); //Revisar si es correcto olvidar el evento con el id 0.
                    agent.auxCharacter.m_emotionalAppraisalAsset.RemoveAppraisalRules(new List<AppraisalRuleDTO>() { newAppRule });


                    foreach (var _emo in _newEmotions)
                    {
                        _OCCemotions.Add(OCCEmotionType.Parse(_emo.Type));
                    }
                    if (_OCCemotions.Any(e => e.Valence == EmotionValence.Positive))
                    {
                        _Threshold = true;
                    }
                    else _Threshold = _newEmotions.FirstOrDefault().Intensity <= personality.MaxLevelEmotion;
                    if (_Threshold)
                    {
                        oldActionRule = character.m_emotionalDecisionMakingAsset.GetAllActionRules().FirstOrDefault(
                                                                                    act => act.Action == eventName);
                        newPriority = Name.BuildName(float.Parse(oldActionRule.Priority.ToString()) + 1);
                        for (int i = 0; i < _appraisalVariables.Count(); i++)
                        {
                            _appraisalVariables[i].Value = _appraisalVariablesCopy[i].Value;
                        }
                        AppliedStrategy = true;
                        break;
                    }

                }
                break; //solo se está considerando como que la primer reacción funcionó.

            }

            if (_Threshold)
            {
                ActionRuleDTO newActionRule = new ActionRuleDTO()
                {
                    Action = newActionName, 
                    Conditions = oldActionRule.Conditions,
                    Layer = oldActionRule.Layer,
                    Priority = newPriority,
                    Target = oldActionRule.Target
                };
                character.m_emotionalDecisionMakingAsset.AddActionRule(newActionRule);
                character.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(newAppRule); //actualizar o crear una nueva regla ?
            }

            return AppliedStrategy;
        }
 
        //Attention Deployment 
        private bool AttentionDeployment(List<AppraisalRuleDTO> RulesOfEvent, List<Name> pastEvents)
        {
            ///Past events, whose target matches with the target of the current event. (T->T)/(4-4)
            ///Esta estrategia podrá ser aplicada cuando existan eventos (positivos) en la memoria del agente, que  
            ///involucren a los participantes del evento actual.
            agent.auxCharacter.ResetEmotionalState();
            var modd = agent.auxCharacter.Mood;
            var AppliedStrategy = false;
            var positiveIntensity = new List<float>();

            for (int i = 0; i < pastEvents.Count; i++)
            {
                var allActiveEmotions = agent.auxCharacter.GetAllActiveEmotions();

                int count = allActiveEmotions.Count();
                agent.auxCharacter.Perceive(pastEvents[i]);
                int count2 = allActiveEmotions.Count();

                var emo = allActiveEmotions.LastOrDefault();
                var OCCemotion = OCCEmotionType.Parse(emo.Type);

                if (OCCemotion.Valence == EmotionValence.Positive && (count != count2) )
                {
                    /// al parecer la emoción que tiene una variable de valoración con el valor de 5, resulta con una intensidad de 3.8
                    positiveIntensity.Add(emo.Intensity);
                } 
            }
           var emotionsProm = positiveIntensity.Average();
            if (isSpeak)
            {





            }

            /*
            /// FAtiMA no guarda el historial de las emociones por lo tanto es necesario volver a calcular las emciones.
            if (PastEvents.Any())
            {
                var AllRulesOfScenario = character.m_emotionalAppraisalAsset.GetAllAppraisalRules().ToList();
                List<AppraisalRuleDTO> OldappraisalRules = new List<AppraisalRuleDTO>();
                var emotionsOfPastEvt = new List<SetDataForEmotionRegulation.LikelyEmotion>();
                foreach (var RulesOfpastEvents in AllRulesOfScenario)
                {

                    foreach (var Event in PastEvents)
                    {
                        var pastEvt = (Name)Event.Event;
                        
                        if (pastEvt.GetNTerm(3).Equals(RulesOfpastEvents.EventMatchingTemplate.GetNTerm(3)))
                        {
                            OldappraisalRules.Add(RulesOfpastEvents);
                        }
                        else
                            break;
                        emotionsOfPastEvt = internalSettings.EmotionsDerivator(character.m_emotionalAppraisalAsset, pastEvt, character.Mood);
                    }   
                }
                if (!emotionsOfPastEvt.Any() || emotionsOfPastEvt.Where(e => (float)e.EmotionType.Valence == 1) == null)
                {
                    return AppliedStrategy;
                }


                var AuxEA = new EmotionalAppraisalAsset();
                var avg = emotionsOfPastEvt.Select(emo => emo.Intensity).Average(); ///avg de intensidad positiva de los eventos pasados
                foreach (var NegativeEmotion in NegativeEmotions)
                {
                    var AppraisalVariablesInEmotion = NegativeEmotion.AppraisalVariables;
                    var Newrules = new List<AppraisalRuleDTO>();

                    foreach (var old in AppraisalVariablesInEmotion)
                    {
                        var tanh = internalSettings.AppraisalFunction(old.Value.value, avg, 10, (int)old.Value.valance);
                        var _target = string.Empty;
                        var _eventMatchingTemplate = string.Empty;

                        RulesOfEvent.ToList().ForEach(r2 =>
                        {
                            var appraisalVariables = r2.AppraisalVariables.appraisalVariables;
                            _target = appraisalVariables.FirstOrDefault(v => v.Name == old.Key).Target.ToString();
                            _eventMatchingTemplate = r2.EventMatchingTemplate.ToString();
                        });
                        var appraisalVariableDTO = new List<AppraisalVariableDTO>()
                        {
                        new AppraisalVariableDTO ()
                        {
                            Name = old.Key,
                            Value = Name.BuildName(tanh),
                            Target = (Name)_target
                        }
                        };
                        var rule = new AppraisalRuleDTO()
                        {
                            EventMatchingTemplate = (Name)_eventMatchingTemplate,
                            AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                        };
                        Newrules.Add(rule);
                        AuxEA.AddOrUpdateAppraisalRule(rule);
                    }
                    var potentialemotions = internalSettings.EmotionsDerivator(AuxEA, currentEvent, character.Mood);
                    var Threshold = potentialemotions.LastOrDefault().Intensity <= personality.MaxLevelEmotion;

                    if (Threshold)
                    {
                        foreach (var New in potentialemotions.FirstOrDefault().AppraisalVariables)
                        {
    
                            RulesOfEvent.ToList().ForEach(r => r.AppraisalVariables.appraisalVariables.FirstOrDefault(
                                v => v.Name == New.Key).Value = Name.BuildName(New.Value.value));
                        }

                        possibleEmotions = potentialemotions;
                        foreach(var NewAppRule in Newrules)
                            character.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(NewAppRule);

                        var currentAction = character.m_emotionalDecisionMakingAsset.GetAllActionRules().FirstOrDefault(
                                    act => act.Action == currentEventName);
                        var NewPriority = Name.BuildName(float.Parse(currentAction.Priority.ToString()) * 2);

                        foreach(var pastEvt in PastEvents)
                        {
                            var nameOfEvent = (Name)pastEvt.Event;
                            var avtionName = nameOfEvent.GetNTerm(3);
                            
                            ActionRuleDTO action = new ActionRuleDTO()
                            {
                                Action = Name.BuildName("FocusOn"+avtionName),
                                Conditions = currentAction.Conditions,
                                Layer = currentAction.Layer,
                                Priority = NewPriority,
                                Target = currentAction.Target
                            };

                            character.m_emotionalDecisionMakingAsset.AddActionRule(action);
                        }
                        AppliedStrategy = true;
                    }

                }
            
            }
            */
            return AppliedStrategy;

        }
        /*
        // Cognitive Change
        private bool CognitiveChange(List<SetDataForEmotionRegulation.LikelyEmotion> NegativeEmotions, List<AppraisalRuleDTO> RulesOfEvent, List<Name> evtsForReappraisal)
        {
            Console.WriteLine("\n\n---------------------Cognitive Change------------------------");
            var AppliedStrategy = false;
            
            var AlternativeEvents  = evtsForReappraisal.Where(Event => Event.GetNTerm(4) == currentEventName);

            if (!AlternativeEvents.Any())
            {
                return AppliedStrategy;
            }
            var AllAppRules = character.m_emotionalAppraisalAsset.GetAllAppraisalRules().ToList();
            var EmotionsOfAlternativeEvt = new List<List<SetDataForEmotionRegulation.LikelyEmotion>>();
            var IntensitySum = new List<float>();

            foreach (var alternativeEvent in AlternativeEvents)
            {
                var RulesOfAlternatveEvt = AllAppRules.Where(
                r => r.EventMatchingTemplate.GetNTerm(3) == alternativeEvent.GetNTerm(3)).ToList();
    
                var EmotionsFelt = internalSettings.EmotionsDerivator(character, RulesOfAlternatveEvt);
                EmotionsOfAlternativeEvt.Add(EmotionsFelt);
                //Console.WriteLine("\n '" + alternativeEvent.GetNTerm(3) + "'");
                //EmotionsFelt.ForEach(e => Console.WriteLine($@"     Feeling emotion ---> {e.EmotionType.EmotionType} : {e.Intensity}"));
            }
            EmotionsOfAlternativeEvt.ForEach(ea =>
            { ea.Select(emo => emo.Intensity).ForEach(e => IntensitySum.Add(e)); });

            if (IntensitySum.Any())
            {
                var UnfitPersonality = (float)((personality.Neuroticism + personality.Agreeableness) / 2);
                var FitPersonality = (float)((personality.Conscientiousness + personality.Extraversion + personality.Openness) / 3);
                var ReinterpretationAvg = IntensitySum.Average();

                var AuxEA = new EmotionalAppraisalAsset();
                foreach (var NegativeEmotion in NegativeEmotions)
                {
                    var AppraisalVariablesInEmotion = NegativeEmotion.AppraisalVariables;
                    var rules = new List<AppraisalRuleDTO>();
                    foreach (var old in AppraisalVariablesInEmotion)
                    {
                        var Valoration = (character.Mood + ReinterpretationAvg);
                        var UnfitTanh = internalSettings.AppraisalFunction(old.Value.value, UnfitPersonality, 100, (int)old.Value.valance);
                        var FitTanh =  internalSettings.AppraisalFunction(old.Value.value, FitPersonality, 100, (int)old.Value.valance);
                        var MoodIntensity = internalSettings.AppraisalFunction(old.Value.value, Valoration, 10, (int)old.Value.valance);
                        var tanh = (float)((UnfitTanh + MoodIntensity + FitTanh) / 3);

                        var _target = string.Empty;
                        var _eventMatchingTemplate = string.Empty;

                        RulesOfEvent.ToList().ForEach(r3 =>
                        {
                            var appraisalVariables = r3.AppraisalVariables.appraisalVariables;
                            _target = appraisalVariables.FirstOrDefault(v => v.Name == old.Key).Target.ToString();
                            _eventMatchingTemplate = r3.EventMatchingTemplate.ToString();
                        });

                        var appraisalVariableDTO = new List<AppraisalVariableDTO>()
                        {
                        new AppraisalVariableDTO ()
                            {
                            Name = old.Key,
                            Value = Name.BuildName(tanh),
                            Target = (Name)_target
                            }
                        };
                        var rule = new AppraisalRuleDTO()
                        {
                            EventMatchingTemplate = (Name)_eventMatchingTemplate,
                            AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                        };
                        rules.Add(rule);
                        AuxEA.AddOrUpdateAppraisalRule(rule);
                    }
                    var potentialemotions = internalSettings.EmotionsDerivator(AuxEA, currentEvent, character.Mood); ///emociones potentiales

                    Console.WriteLine(" \n Must generate --> "
                        + potentialemotions.FirstOrDefault().EmotionType.EmotionType + " : "
                        + potentialemotions.LastOrDefault().Intensity
                        + " \n                   Mood : " + potentialemotions.LastOrDefault().Mood);

                    var Threshold = false;
                    if (potentialemotions.Where(e => (float)e.EmotionType.Valence == 1).Any())
                        Threshold = true;

                    Threshold = potentialemotions.LastOrDefault().Intensity <= personality.MaxLevelEmotion;
                    if (Threshold)
                    {
                        foreach (var New in potentialemotions.FirstOrDefault().AppraisalVariables)
                        {
                            ///Aquí se están ya actualizando las variables del valoración
                            RulesOfEvent.ToList().ForEach(r => r.AppraisalVariables.appraisalVariables.FirstOrDefault(
                                v => v.Name == New.Key).Value = Name.BuildName(New.Value.value));
                        }
                        possibleEmotions = potentialemotions;

                        var currentAction = character.m_emotionalDecisionMakingAsset.GetAllActionRules().FirstOrDefault(
                                    act => act.Action == currentEventName);
                        var NewPriority = Name.BuildName(float.Parse(currentAction.Priority.ToString()) * 2);

                        foreach (var evt in AlternativeEvents)
                        {
                            
                            var avtionName = evt.GetNTerm(3);

                            ActionRuleDTO action = new ActionRuleDTO()
                            {
                                Action = Name.BuildName("ThinkingIn" + avtionName),
                                Conditions = currentAction.Conditions,
                                Layer = currentAction.Layer,
                                Priority = NewPriority,
                                Target = currentAction.Target
                            };

                            character.m_emotionalDecisionMakingAsset.AddActionRule(action);
                        }

                        AppliedStrategy = true;
                    }
                }
            }
            return AppliedStrategy;
        }
    
        //Response Modulation
        private bool ResponseModulation()
        {
            var AppliedStrategy = false;
            var FAtiMAConfiguration = new EmotionalAppraisalConfiguration();

            var ActiveEmotion = character.GetAllActiveEmotions().LastOrDefault();
            ///Se supone que se está evaluando una emoción negativa, por lo qué el valance simpre será negativo
            int valenceOfemotion = -1;

            var _personalitiesValues = new float[] 
            {
                personality.Openness, 
                personality.Conscientiousness, 
                personality.Extraversion, 
                personality.Agreeableness,
                personality.Neuroticism
            };

            var allTraits = _personalitiesValues.Average();
            var PersonalityBasedEmotion = internalSettings.AppraisalFunction(allTraits, ActiveEmotion.Intensity, 5, 1) + allTraits;
            var newIntensity = (float)Math.Round(internalSettings.AppraisalFunction(ActiveEmotion.Intensity, PersonalityBasedEmotion, 100, -1));

            var Threshold = newIntensity <= personality.MaxLevelEmotion;

            if (Threshold)
            {
                var MoodDueToEvent = valenceOfemotion * (ActiveEmotion.Intensity * FAtiMAConfiguration.MoodInfluenceOnEmotionFactor);
                var valueMoodDueToEvent = MoodDueToEvent < -10 ? -10 : (MoodDueToEvent > 10 ? 10 : MoodDueToEvent);
                var CurrentMoodDueToEvent = character.Mood;
                var MoodWithoutEventValue = CurrentMoodDueToEvent - MoodDueToEvent;

                //To create and update the emotion
                var NewEmotion = new EmotionDTO
                {
                    CauseEventId = ActiveEmotion.CauseEventId,
                    Type = ActiveEmotion.Type,
                    Intensity = newIntensity,
                    CauseEventName = ActiveEmotion.CauseEventName,
                    Target = ActiveEmotion.Target,
                };

                var AuxConcreteEmotion = new ConcreteEmotionalState();
                character.RemoveEmotion(ActiveEmotion); ///Remove the emotion from agent
                character.AddActiveEmotion(NewEmotion);

                var newEmotions = character.GetAllActiveEmotions().ToList();
  
                //Update the Mood
                var NewEmoValence = valenceOfemotion;
                var NewEmoIntencity = newEmotions.LastOrDefault().Intensity;
                var moodDouToNewIntensity = NewEmoValence * (NewEmoIntencity * FAtiMAConfiguration.MoodInfluenceOnEmotionFactor);
                var MoodDouToNewIntensity = moodDouToNewIntensity < -10 ? -10 : (moodDouToNewIntensity > 10 ? 10 : moodDouToNewIntensity);
                var NewMood = MoodWithoutEventValue + MoodDouToNewIntensity;
                character.Mood = NewMood;

                AppliedStrategy = true;
            }

            return AppliedStrategy;
        }
        */
        #endregion
    }

}
