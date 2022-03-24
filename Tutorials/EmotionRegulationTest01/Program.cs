using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RolePlayCharacter;
using EmotionRegulation;
using WellFormedNames;
using EmotionalAppraisal.DTOs;
using EmotionalAppraisal;
using EmotionalAppraisal.OCCModel;
using ActionLibrary.DTOs;
using EmotionalDecisionMaking;
using IntegratedAuthoringTool;
using GAIPS.Rage;
using System.IO;
using EmotionRegulation.Components;
using ActionLibrary;

namespace EmotionRegulationTest01
{
    class Program
    {
        static IntegratedAuthoringToolAsset iatTC;
        static RolePlayCharacterAsset _RPC;
        static EmotionRegulationAsset _ER;
        static PersonalityTraitsAsset _TraitsOfAgent;
        static EmotionalAppraisalAsset _appraisalAsset;
        static EmotionalDecisionMakingAsset _edm;

        static void Main(string[] args)
        {

            _appraisalAsset = new EmotionalAppraisalAsset();
            _edm = new EmotionalDecisionMakingAsset();
            _RPC = new RolePlayCharacterAsset() { BodyName = "Male", CharacterName = (Name)"Carlos" };
            _RPC.m_emotionalAppraisalAsset = _appraisalAsset;
            _RPC.m_emotionalDecisionMakingAsset = _edm;
            _RPC.m_emotionalDecisionMakingAsset.RegisterKnowledgeBase(_RPC.m_kb);
            _RPC.ActivateIdentity(new Identity((Name)"Portuguese", (Name)"Culture", 1));


            _RPC.m_kb.Tell((Name)"Love(HisCar)", (Name)"True", (Name)"SELF");
            _RPC.m_kb.Tell((Name)"Driving(Car)", (Name)"False", (Name)"SELF");
            _RPC.m_kb.Tell((Name)"Has(Car)", (Name)"1", (Name)"SELF");


            var Drive = Name.BuildName("Event(Action-End, Pedro, Drive, Car)");
            var Crash = Name.BuildName("Event(Action-End, Pedro, Crash, Car)");

            var TalktoBoss = Name.BuildName("Event(Action-End, Pedro, TalkToBoss, Boss)");
            var Hello = Name.BuildName("Event(Action-End, Pedro, Hello, Sarah)");
            var Conversation = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
            var Hug = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
            var Discussion = Name.BuildName("Event(Action-End, Pedro, Discussion, Others)");
            var Congrat = Name.BuildName("Event(Action-End, Pedro, Congrat, Sarah)");
            var Bye = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah)");
            var Fired = Name.BuildName("Event(Action-End, Pedro, Fired, Boss)");
            var Crash2 = Name.BuildName("Event(Action-End, Pedro, Crash, Car)");
            var Profits = Name.BuildName("Event(Action-End, Pedro, Profits, Pedro)");
            var CatDied = Name.BuildName("Event(Action-End, Pedro, CatDied, Cat)");

            //Lectura de los archivos json
            ///pruba para saber en que parte estan los datos necesarios para el modelo de regulación emocional.
            var pathRoot = @"C:\Users\JuanJoseAsus\source\repos\FAtiMA-Toolkit-version02\Tutorials\EmotionRegulationTest01\bin\Scenarios\";
            var pathScenarioSimple = pathRoot + "SimpleScenarioIAT.json";
            var pathStorageSimple = pathRoot + "SimpleScenarioStorage.json";

            var storageSimple = AssetStorage.FromJson(File.ReadAllText(pathStorageSimple));
            var iatSimple = IntegratedAuthoringToolAsset.FromJson(File.ReadAllText(pathScenarioSimple), storageSimple);

            ///info: dentro iat estan los agentes, dentro de cada agente está un campo para cada asset, dentro del campo m_emotional
            ///appraisal están las appraisals rules, las cuales son las posibles acciones o eventos que el agente puede persivir,
            ///realizar (para simular que una accíon o evento se lleva a cabo se necesita del asset: emotional decition making).
            //END

            //suponiendo que del asset iat, se recupera la info de un agente específico. en este caso, el agente que estamos
            //creando via código.


            var AppRuleCrash = SetAppRules(OCCAppraisalVariables.DESIRABILITY, (Name)"-5", (Name)"-", Crash);
            var AppRuleDrive = SetAppRules(OCCAppraisalVariables.DESIRABILITY, (Name)"5", (Name)"-", Drive);
            _RPC.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(AppRuleCrash);
            _RPC.m_emotionalAppraisalAsset.AddOrUpdateAppraisalRule(AppRuleDrive);

            //var beliefs = _RPC.GetAllBeliefs().ToList();
            //Console.WriteLine($"Beliefs : \n");
            //beliefs.ForEach(b => { Console.WriteLine(b.Name + " = "+ b.Value); });



            var actionDrive = new ActionRuleDTO() { Action = (Name)"Drive", Priority = (Name)"1", Target = (Name)"Car" };
            var id_actionDrive = _RPC.m_emotionalDecisionMakingAsset.AddActionRule(actionDrive);
            _RPC.m_emotionalDecisionMakingAsset.AddRuleCondition(id_actionDrive, "Has(Car) != 1");

            var actionCrash = new ActionRuleDTO() { Action = (Name)"Crash", Priority = (Name)"1", Target = (Name)"Car" };
            var id_actionCrash = _RPC.m_emotionalDecisionMakingAsset.AddActionRule(actionCrash);
            _RPC.m_emotionalDecisionMakingAsset.AddRuleCondition(id_actionCrash, "Driving(Car) = False");


            //Botón para decir que si se va a configurar el asset Emotional Regulation
            ///Parte de Emotional Regulation Configuration

            //Show Agents into the scene:
            var AgentsInScene = iatSimple.Characters;
            Console.WriteLine("SELECTION AGENT");
            foreach (var agnt in AgentsInScene)
            {
                Console.WriteLine($"Agents in scene: {agnt}");
            }
            ///Asumption: select one
            var agent = AgentsInScene.FirstOrDefault(a => a.CharacterName == (Name)"Carlos");
            //Show Events
            var appRuleAgnt = agent.m_emotionalAppraisalAsset.GetAllAppraisalRules();
            //selection of events to avoid. "Se podria seleccionar y mostrar solo aquellas que tienen relación con el agente,
            //pero no estoy seguro de que sea facil, que valga la pena o de que funcione..jeje. Todos los agentes tienen guardados
            //los mismos eventos en su propio emotionalAppraisal."
            Console.WriteLine("\nSELECTION EVENT TO AVOID");
            foreach (var apprules in appRuleAgnt)
            {
                Console.WriteLine($"Events: {apprules.EventMatchingTemplate}");
            }
            //Assumption: select one
            var evtAvoid = appRuleAgnt.FirstOrDefault(r => r.EventMatchingTemplate.GetNTerm(3) == (Name)"Crash");
            var eventsToAvoid = new List<AppraisalRuleDTO>() { evtAvoid };
            //Cambiamos al agente que se creo desde código para seguir con la simulación.



            //Se carga la configuración para emotionRegulation

            

            var AgentER = new BaseAgent() { 
                AgentFAtiMA = agent, PersonalityOfAgent = new Personality() { Agreeableness = } };

            var dataIforamation = new RequiredData() { EventsToAvoid = eventsToAvoid, };

            var decitions = agent.Decide().FirstOrDefault();
            var emotionR = new EmotionRegulationModel(dataIforamation, )

            var ERAconfigs = new OLDEmotionalRegulationSettings()
            {
                CurrentCharacter = agent,
                //EventsToAvoid = evtER,
                ExpectedLevelEmotion = 5f,
                //Personalities = new Personality() { Conscientiousness = 100}
                 
            };
            ERAconfigs.CreatePersonality();

            //Check information
            //Console.WriteLine($"Agent and personality: {ERAconfigs.CurrentCharacter.CharacterName} --> {ERAconfigs.NewCharacter.DominantPersonality}");
            ERAconfigs.EventsToAvoid.ForEach(e1 => Console.WriteLine(e1.EventMatchingTemplate.GetNTerm(3)));


            
            //show configuration the Actions for Situation selection:
            /// El usuario debe de crear un conjunto de acciones qué el egente podrá aplicar un evento específico
            Console.WriteLine($"Crea nuevas acciones que el agente pueda utilizar para modificar su entorno en un evento específico");

            ///suponiendo que el usuario selecciona un EventMatchingTemplate, este se pasa como parametro a la clase ERactions
            ///y se crea una lista con aciones para ejecutar.
            List<ActionRuleDTO> listOfActionsER = new List<ActionRuleDTO>()
            {
                new ActionRuleDTO()
                {
                    Action = (Name)"DrivingCarefully",
                    Conditions = null,
                    Layer = (Name)"-",
                    Priority = (Name)"1",
                    Target = (Name)"SELF"
                },
                new ActionRuleDTO()
                {
                    Action = (Name)"NotListeningMusic",
                    Conditions = null,
                    Layer = (Name)"-",
                    Priority = (Name)"1",
                    Target = (Name)"SELF"
                }


            };
            ActionsforEvent actionsForEvenet = new ActionsforEvent(listOfActionsER, (Name)"Crash");//Revisar cambiar el nombre del evento
            ERAconfigs.SetActions(actionsForEvenet);

            //Show configuration events to reappraisal
            //..
            //..
            //..

            //show configuration for events to cognitive change.
            //..
            //..
            //..

            //Emotion Regulation creo que forma parte de la simulacion, para cada evento y agente.




            var decitions = agent.Decide().FirstOrDefault();
            //Entra a regular la emoción
            var cl1 = new EmotionRegulationModel(ERAconfigs, decitions);


            Console.WriteLine("Load....");


            agent = ERAconfigs.CurrentCharacter;

            /*
            //recuperar la información de los eventos declarados por el usuario.
            var appRules = _RPC.m_emotionalAppraisalAsset.GetAllAppraisalRules();
            ///MENÚ PARA EL ASSET DE EMOTION REGULATION
            Console.WriteLine("Event to avoid: [key -3 = out]");
            for (int i = 0; i < appRules.Count(); i++)
            {
                Console.WriteLine(i + " - " + appRules.ElementAt(i).EventMatchingTemplate);
            }
            List<AppraisalRuleDTO> ListEvtFatima = new List<AppraisalRuleDTO>();
            int pos = -1;

            do
            {
                Console.Write("Select Option: ");
                if (pos != -1) { ListEvtFatima.Add(appRules.ElementAt(pos));pos = -1; }
                    
            } while (!Int32.TryParse(Console.ReadLine(), out pos) || pos != -3);
            var evtER = new List<Name>();
            foreach (var evtFatima in ListEvtFatima)
            {
                evtER.Add((Name)string.Concat(evtFatima.EventMatchingTemplate.ToString().Replace(")", string.Empty) + ", true)"));
      
            }
            */



            //SIMULATION
            ///En la simulación primero es el método de RPC.Decide y despues el de RPC.Perceive, el método perceive es para
            ///cada agente en la escena.



            _RPC.Perceive(Drive);


        }















        static AppraisalRuleDTO SetAppRules(string OCCvar, Name value, Name target, Name evt)
        {
            var _AppraisalVariableDTO = new AppraisalVariableDTO() { Name = OCCvar, Target = target, Value = value };
            var rule = new AppraisalRuleDTO() { EventMatchingTemplate = evt, AppraisalVariables = new AppraisalVariables(
                new[] { _AppraisalVariableDTO }.ToList()) };
            return rule;
        }
    }
}
