using System;
using System.Linq;
using System.Collections.Generic;
using RolePlayCharacter;
using WellFormedNames;
using EmotionalAppraisal;
using EmotionalAppraisal.OCCModel;
using KnowledgeBase;
using EmotionalDecisionMaking;
using ActionLibrary.DTOs;
using WorldModel;
using WorldModel.DTOs;
using EmotionalAppraisal.DTOs;
using EmotionRegulation.BigFiveModel;
using EmotionRegulation.Components;


namespace ExampleForThesis
{
    class Scenario2
    {
        static WorldModelAsset worldModel = new WorldModelAsset();
        
        static void Main(string[] args)
        {

            //-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            //                       scenario
            //-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

            var emotionalAppraisal = new EmotionalAppraisalAsset();
            var emotionalDecisionMaking = new EmotionalDecisionMakingAsset();
            var KnownledgeBase = new KB((Name)"Sam");

            var Sam = new RolePlayCharacterAsset()
            {
                CharacterName = (Name)"Sam",
                BodyName = "Male",
                m_emotionalAppraisalAsset = emotionalAppraisal,
                m_emotionalDecisionMakingAsset = emotionalDecisionMaking,
                m_kb = KnownledgeBase,
            };

            Sam.m_kb.Tell((Name)"Location(Work)", (Name)"True");
            Sam.m_kb.Tell((Name)"Know(News)", (Name)"False");
            Sam.m_emotionalDecisionMakingAsset.RegisterKnowledgeBase(KnownledgeBase);
            Sam.m_kb.Reason();

            var otherAgent = SetAgent();

            var event0 = Name.BuildName("Event(Action-End, Nameless, Tell-To, Sam)");

            var appRule = new AppraisalRuleDTO
            {
                AppraisalVariables = new AppraisalVariables
                {
                    appraisalVariables = new List<AppraisalVariableDTO>
                    {
                        new AppraisalVariableDTO { Name = OCCAppraisalVariables.DESIRABILITY, Value = (Name)"-6" }
                    }
                },
                EventMatchingTemplate = Name.BuildName("Event(Action-End, *, Tell-To, *)")
             };
            Sam.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(appRule);

            ActionRuleDTO GoTo = new ActionRuleDTO()
            {
                Action = (Name)"GoTo",
                Target = (Name)"Boss",
                Priority = (Name)"1",
                Conditions = new Conditions.DTOs.ConditionSetDTO {  ConditionSet = new string[] { "Know(News)=True" } },
                
            };
            Sam.m_emotionalDecisionMakingAsset.AddActionRule(GoTo);

            worldModel.addActionTemplate(event0,1);
            worldModel.AddActionEffect(event0, new EffectDTO()
            {
                PropertyName = (Name)"Know(News)",
                NewValue = (Name)"True",
                ObserverAgent = (Name)"SELF"
            });

            //-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
            //                        Emotion Regulation
            //-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
            
            var SamPersonality = new PersonalityDTO
            { 
                Extraversion = 15, 
                Agreeableness = 40, 
                Conscientiousness = 75, 
                Neuroticism = 10, 
                Openness = 45,
                MaxLevelEmotion = 4
            };
            var dataForEmotionRegulation = new RequiredData { EventsToAvoid = new List<AppraisalRuleDTO>() { appRule } };
            var newSam = new BaseAgent(Sam, SamPersonality, dataForEmotionRegulation);
            Console.WriteLine($" Personalities of agent {newSam.AgentName}:\n");
            newSam.AllPersonalities.ForEach(x => Console.WriteLine(x));
            Console.WriteLine($"\n Dominant personality: {newSam.DominantPersonality}\n ");
            Console.WriteLine($" Strategies to apply: \n");
            newSam.StrategiesToApply.ForEach(x => Console.WriteLine(x));


            //-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+End configurations
            var TellSam = otherAgent.Decide().ToList();
            
            
            var newDecision = newSam.Regulate(TellSam.First());

            var eventStart = EventHelper.ActionEnd(otherAgent.CharacterName, TellSam.First().Name, TellSam.First().Target);
            
            Sam.Perceive(eventStart);
            var firstEmotion = Sam.GetAllActiveEmotions().First();
            Console.WriteLine($" The agent percive: Event( {eventStart.GetNTerm(3) + " " + eventStart.GetNTerm(4)} ) " +
                                  $"and feels: {firstEmotion.Type} = {firstEmotion.Intensity}");
            Console.ReadLine();

            WorldEffects(eventStart, new List<RolePlayCharacterAsset>() { Sam, otherAgent });

            var SamDecision = Sam.Decide().ToList();

            var newEvent = EventHelper.ActionEnd(Sam.CharacterName, newDecision.Name, newDecision.Target);
            Sam.Perceive(newEvent);


            IEnumerable<EmotionDTO> emotions;
            EmotionDTO emotionDTO = new EmotionDTO();
            do
            {
                emotions = Sam.GetAllActiveEmotions();
                if (emotions.Any())
                {
                    emotionDTO = emotions.First();
                }
                Console.WriteLine($" The agent percive: Event( {newEvent.GetNTerm(3)+" "+ newEvent.GetNTerm(4)} ) " +
                                  $"and feels: {emotionDTO.Type} = {emotionDTO.Intensity}");
                Console.WriteLine("\r");
                Sam.Update();
            } while (emotionDTO.Intensity > 0.16);

        }

        static RolePlayCharacterAsset SetAgent()
        {
            var emotionalAppraisal = new EmotionalAppraisalAsset();
            var emotionalDecisionMaking = new EmotionalDecisionMakingAsset();
            var KnownledgeBase = new KB((Name)"Nameless");

            var otherAgent = new RolePlayCharacterAsset()
            {
                CharacterName = (Name)"Nameless",
                BodyName = "Male",
                m_emotionalAppraisalAsset = emotionalAppraisal,
                m_emotionalDecisionMakingAsset = emotionalDecisionMaking,
                m_kb = KnownledgeBase,
            };
            otherAgent.m_kb.Tell((Name)"Location(Work)", (Name)"True");
            otherAgent.m_emotionalDecisionMakingAsset.RegisterKnowledgeBase(KnownledgeBase);
            otherAgent.m_kb.Reason();
            ActionRuleDTO start = new ActionRuleDTO()
            {
                Action = (Name)"Tell-To",
                Target = (Name)"Sam",
                Priority = (Name)"1"
            };
            otherAgent.m_emotionalDecisionMakingAsset.AddActionRule(start);

            return otherAgent;
        }

        static void WorldEffects(Name _event, List<RolePlayCharacterAsset> _characters)
        {
            
            var consequences =  worldModel.Simulate(new Name[] { _event });

            // For each effect 
            foreach (var eff in consequences)
            {
                Console.WriteLine("Effect: " + eff.PropertyName + " " + eff.NewValue + " " + eff.ObserverAgent);

                // For each Role Play Character
                foreach (var rpc in _characters)
                {

                    //If the "Observer" part of the effect corresponds to the name of the agent or if it is a universal symbol
                    //if (eff.ObserverAgent != rpc.CharacterName && eff.ObserverAgent != (Name)"*") continue;
                    //Apply that consequence to the agent
                    rpc.Perceive(EventHelper.PropertyChange(eff.PropertyName, eff.NewValue, rpc.CharacterName));

                }
            }
        }
    }
}
